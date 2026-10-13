using DNXUdemyData.Entities;
using HDNXUdemyData.Entities;
using HDNXUdemyData.IRepository;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.Exception;
using HDNXUdemyModel.RequestModel;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyModel.SystemExceptions;
using HDNXUdemyServices.IServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;

namespace HDNXUdemyServices.Services
{
    public class YoutubeImportService : IYoutubeImportService
    {
        private const string YoutubeApiBaseUrl = "https://www.googleapis.com/youtube/v3";

        private readonly HttpClient _httpClient;
        private readonly ICourseRepository _courseRepository;
        private readonly IContentCourseRepository _contentCourseRepository;
        private readonly IContentCourseDetailRepository _contentCourseDetailRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;

        public YoutubeImportService(HttpClient httpClient, ICourseRepository courseRepository, IContentCourseRepository contentCourseRepository,
            IContentCourseDetailRepository contentCourseDetailRepository, ICategoryRepository categoryRepository, IUserRepository userRepository)
        {
            _httpClient = httpClient;
            _courseRepository = courseRepository;
            _contentCourseRepository = contentCourseRepository;
            _contentCourseDetailRepository = contentCourseDetailRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
        }

        public async Task<List<YoutubePlaylistPreviewResponse>> PreviewChannel(YoutubePreviewChannelRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.ChannelUrl))
            {
                throw new ProjectBadRequestException("ChannelUrl is required.");
            }

            var channelId = await ResolveChannelId(request.ChannelUrl.Trim());
            var playlists = new List<YoutubePlaylistPreviewResponse>();
            string? nextPageToken = null;

            do
            {
                var url = BuildUrl("playlists", new Dictionary<string, string?>
                {
                    ["part"] = "snippet,contentDetails",
                    ["channelId"] = channelId,
                    ["maxResults"] = "50",
                    ["pageToken"] = nextPageToken
                });

                using var document = await GetJsonDocument(url);
                nextPageToken = ReadString(document.RootElement, "nextPageToken");
                foreach (var item in document.RootElement.GetProperty("items").EnumerateArray())
                {
                    var snippet = item.GetProperty("snippet");
                    var contentDetails = item.GetProperty("contentDetails");
                    playlists.Add(new YoutubePlaylistPreviewResponse
                    {
                        PlaylistId = ReadString(item, "id"),
                        ChannelId = ReadString(snippet, "channelId"),
                        ChannelTitle = ReadString(snippet, "channelTitle"),
                        Title = ReadString(snippet, "title"),
                        Description = ReadString(snippet, "description"),
                        ThumbnailUrl = ReadThumbnailUrl(snippet),
                        VideoCount = ReadInt(contentDetails, "itemCount")
                    });
                }
            } while (!string.IsNullOrWhiteSpace(nextPageToken));

            return playlists;
        }

        public async Task<YoutubePlaylistPreviewResponse> PreviewPlaylist(YoutubePreviewPlaylistRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.PlaylistUrlOrId))
            {
                throw new ProjectBadRequestException("PlaylistUrlOrId is required.");
            }

            var playlistId = ExtractPlaylistId(request.PlaylistUrlOrId.Trim());
            var playlist = await GetPlaylist(playlistId);
            playlist.Videos = await GetPlaylistVideos(playlistId);
            playlist.VideoCount = playlist.Videos.Count;
            playlist.TotalDuration = FormatDuration(playlist.Videos.Select(x => ParseDisplayDuration(x.Duration)).Aggregate(TimeSpan.Zero, (total, item) => total + item));
            return playlist;
        }

        public async Task<YoutubeImportPlaylistResponse> ImportPlaylist(YoutubeImportPlaylistRequest request)
        {
            ValidateImportRequest(request);
            var playlistId = ExtractPlaylistId(request.PlaylistUrlOrId!.Trim());

            var duplicatedCourse = await _courseRepository.GetObjectAsync(x => x.KeyVideoUpload == playlistId && x.Status == (int)EStatus.Active);
            if (duplicatedCourse != null)
            {
                throw new ProjectConflictException("This YouTube playlist has already been imported.");
            }

            _ = await _categoryRepository.GetByIdAsync(request.IdCategory)
                ?? throw new ProjectBadRequestException("Category is invalid.");
            _ = await _userRepository.GetByIdAsync(request.IdAuthor)
                ?? throw new ProjectBadRequestException("Author is invalid.");

            var preview = await PreviewPlaylist(new YoutubePreviewPlaylistRequest { PlaylistUrlOrId = playlistId });
            if (!preview.Videos.Any())
            {
                throw new ProjectBadRequestException("Playlist has no public videos.");
            }

            var firstVideo = preview.Videos.FirstOrDefault();
            var course = await _courseRepository.AddReturnModelAsync(new CourseEntities
            {
                Title = Truncate(preview.Title, 255),
                ShortDescription = Truncate(string.IsNullOrWhiteSpace(preview.Description) ? preview.Title : preview.Description, 1000),
                IdAuthor = request.IdAuthor,
                TotalStudentRegister = 0,
                TotalChapter = preview.Videos.Count,
                Duration = preview.TotalDuration,
                PriceOfCourse = 0,
                IsDiscount = false,
                PriceOfDiscount = 0,
                IsFree = true,
                TypeOfCourse = (int)ETypeOfCourse.Normal,
                Introduce = Truncate(preview.Title, 2000),
                IdCategory = request.IdCategory,
                LevelCourse = Truncate(string.IsNullOrWhiteSpace(request.LevelCourse) ? "Beginner" : request.LevelCourse, 100),
                Languages = request.Language,
                Description = Truncate(string.IsNullOrWhiteSpace(preview.Description) ? preview.Title : preview.Description, 5000),
                PublicId = playlistId,
                PictureUrl = preview.ThumbnailUrl,
                KeyVideoUpload = playlistId,
                FileUrl = firstVideo?.VideoId,
                ProcessCourse = request.PublishNow ? (int)ProcessVideo.Public : (int)ProcessVideo.Open,
                Status = (int)EStatus.Active
            });

            var content = await _contentCourseRepository.AddReturnModelAsync(new ContentCourseEntities
            {
                IdCourse = course.Id,
                Name = Truncate(preview.Title, 255),
                Status = (int)EStatus.Active
            });

            var lessons = preview.Videos.Select(video => new ContentCourseDetailEntities
            {
                IdContent = content.Id,
                NameSubContent = Truncate(video.Title, 255),
                TimeOfContent = video.Duration,
                IsLearningFree = request.IsLearningFree,
                IdVideoUpload = video.VideoId,
                FileNameVideo = video.VideoId,
                IsFinishConvert = true,
                Status = (int)EStatus.Active
            }).ToList();

            await _contentCourseDetailRepository.AddManyAsync(lessons);

            return new YoutubeImportPlaylistResponse
            {
                CourseId = course.Id,
                ContentId = content.Id,
                PlaylistId = playlistId,
                CourseTitle = course.Title,
                ImportedLessons = lessons.Count
            };
        }

        private static void ValidateImportRequest(YoutubeImportPlaylistRequest? request)
        {
            if (request == null)
            {
                throw new ProjectBadRequestException("Payload is required.");
            }

            if (string.IsNullOrWhiteSpace(request.PlaylistUrlOrId))
            {
                throw new ProjectBadRequestException("PlaylistUrlOrId is required.");
            }

            if (request.IdCategory <= 0)
            {
                throw new ProjectBadRequestException("Category is required.");
            }

            if (request.IdAuthor <= 0)
            {
                throw new ProjectBadRequestException("Author is required.");
            }
        }

        private async Task<YoutubePlaylistPreviewResponse> GetPlaylist(string playlistId)
        {
            var url = BuildUrl("playlists", new Dictionary<string, string?>
            {
                ["part"] = "snippet,contentDetails",
                ["id"] = playlistId,
                ["maxResults"] = "1"
            });

            using var document = await GetJsonDocument(url);
            var item = document.RootElement.GetProperty("items").EnumerateArray().FirstOrDefault();
            if (item.ValueKind == JsonValueKind.Undefined)
            {
                throw new ProjectNotFoundException("YouTube playlist not found.");
            }

            var snippet = item.GetProperty("snippet");
            var contentDetails = item.GetProperty("contentDetails");
            return new YoutubePlaylistPreviewResponse
            {
                PlaylistId = ReadString(item, "id"),
                ChannelId = ReadString(snippet, "channelId"),
                ChannelTitle = ReadString(snippet, "channelTitle"),
                Title = ReadString(snippet, "title"),
                Description = ReadString(snippet, "description"),
                ThumbnailUrl = ReadThumbnailUrl(snippet),
                VideoCount = ReadInt(contentDetails, "itemCount")
            };
        }

        private async Task<List<YoutubeVideoPreviewResponse>> GetPlaylistVideos(string playlistId)
        {
            var videos = new List<YoutubeVideoPreviewResponse>();
            string? nextPageToken = null;

            do
            {
                var url = BuildUrl("playlistItems", new Dictionary<string, string?>
                {
                    ["part"] = "snippet",
                    ["playlistId"] = playlistId,
                    ["maxResults"] = "50",
                    ["pageToken"] = nextPageToken
                });

                using var document = await GetJsonDocument(url);
                nextPageToken = ReadString(document.RootElement, "nextPageToken");
                foreach (var item in document.RootElement.GetProperty("items").EnumerateArray())
                {
                    var snippet = item.GetProperty("snippet");
                    if (!snippet.TryGetProperty("resourceId", out var resourceId))
                    {
                        continue;
                    }

                    var videoId = ReadString(resourceId, "videoId");
                    var title = ReadString(snippet, "title");
                    if (string.IsNullOrWhiteSpace(videoId) || string.Equals(title, "Private video", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(title, "Deleted video", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    videos.Add(new YoutubeVideoPreviewResponse
                    {
                        VideoId = videoId,
                        Title = title,
                        Description = ReadString(snippet, "description"),
                        ThumbnailUrl = ReadThumbnailUrl(snippet),
                        Position = ReadInt(snippet, "position")
                    });
                }
            } while (!string.IsNullOrWhiteSpace(nextPageToken));

            await FillVideoDurations(videos);
            return videos.OrderBy(x => x.Position).ToList();
        }

        private async Task FillVideoDurations(List<YoutubeVideoPreviewResponse> videos)
        {
            foreach (var chunk in videos.Chunk(50))
            {
                var ids = string.Join(",", chunk.Select(x => x.VideoId));
                var url = BuildUrl("videos", new Dictionary<string, string?>
                {
                    ["part"] = "contentDetails",
                    ["id"] = ids
                });

                using var document = await GetJsonDocument(url);
                var durations = document.RootElement.GetProperty("items").EnumerateArray()
                    .ToDictionary(x => ReadString(x, "id") ?? string.Empty, x => ReadString(x.GetProperty("contentDetails"), "duration"));

                foreach (var video in chunk)
                {
                    video.Duration = video.VideoId != null && durations.TryGetValue(video.VideoId, out var duration)
                        ? FormatDuration(ParseDuration(duration))
                        : "00:00";
                }
            }
        }

        private async Task<string> ResolveChannelId(string channelUrl)
        {
            var channelIdMatch = Regex.Match(channelUrl, @"(?:youtube\.com\/channel\/|^)(UC[\w-]{20,})", RegexOptions.IgnoreCase);
            if (channelIdMatch.Success)
            {
                return channelIdMatch.Groups[1].Value;
            }

            var handleMatch = Regex.Match(channelUrl, @"youtube\.com\/@(?<handle>[\w.\-]+)", RegexOptions.IgnoreCase);
            if (!handleMatch.Success && channelUrl.StartsWith("@", StringComparison.Ordinal))
            {
                handleMatch = Regex.Match(channelUrl, @"@(?<handle>[\w.\-]+)", RegexOptions.IgnoreCase);
            }

            if (handleMatch.Success)
            {
                var handle = $"@{handleMatch.Groups["handle"].Value}";
                var url = BuildUrl("channels", new Dictionary<string, string?>
                {
                    ["part"] = "id",
                    ["forHandle"] = handle,
                    ["maxResults"] = "1"
                });

                using var document = await GetJsonDocument(url);
                var item = document.RootElement.GetProperty("items").EnumerateArray().FirstOrDefault();
                if (item.ValueKind != JsonValueKind.Undefined)
                {
                    return ReadString(item, "id") ?? throw new ProjectNotFoundException("YouTube channel not found.");
                }
            }

            throw new ProjectBadRequestException("Only YouTube channel URLs with /channel/UC... or @handle are supported.");
        }

        private async Task<JsonDocument> GetJsonDocument(string url)
        {
            EnsureApiKey();
            using var response = await _httpClient.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ProjectBadRequestException($"YouTube API request failed: {body}");
            }

            return JsonDocument.Parse(body);
        }

        private static void EnsureApiKey()
        {
            if (string.IsNullOrWhiteSpace(ProjectConfig.ApiKeyYoutubeApi) || ProjectConfig.ApiKeyYoutubeApi == "CHANGE_ME_USE_ENV")
            {
                throw new ProjectBadRequestException("YouTube API key is not configured.");
            }
        }

        private static string BuildUrl(string resource, IDictionary<string, string?> query)
        {
            EnsureApiKey();
            var items = query
                .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                .Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value!)}")
                .Append($"key={Uri.EscapeDataString(ProjectConfig.ApiKeyYoutubeApi!)}");

            return $"{YoutubeApiBaseUrl}/{resource}?{string.Join("&", items)}";
        }

        private static string ExtractPlaylistId(string value)
        {
            if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
            {
                var match = Regex.Match(uri.Query, @"(?:\?|&)list=([^&]+)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return Uri.UnescapeDataString(match.Groups[1].Value);
                }
            }

            var cleanValue = value.Trim();
            if (Regex.IsMatch(cleanValue, @"^[\w-]+$"))
            {
                return cleanValue;
            }

            throw new ProjectBadRequestException("PlaylistUrlOrId is invalid.");
        }

        private static string? ReadThumbnailUrl(JsonElement snippet)
        {
            if (!snippet.TryGetProperty("thumbnails", out var thumbnails))
            {
                return null;
            }

            foreach (var key in new[] { "maxres", "standard", "high", "medium", "default" })
            {
                if (thumbnails.TryGetProperty(key, out var thumbnail))
                {
                    return ReadString(thumbnail, "url");
                }
            }

            return null;
        }

        private static string? ReadString(JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out var value) && value.ValueKind != JsonValueKind.Null
                ? value.GetString()
                : null;
        }

        private static int ReadInt(JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out var value) && value.TryGetInt32(out var result) ? result : 0;
        }

        private static TimeSpan ParseDuration(string? duration)
        {
            if (string.IsNullOrWhiteSpace(duration))
            {
                return TimeSpan.Zero;
            }

            try
            {
                return XmlConvert.ToTimeSpan(duration);
            }
            catch
            {
                return TimeSpan.Zero;
            }
        }

        private static TimeSpan ParseDisplayDuration(string? duration)
        {
            if (string.IsNullOrWhiteSpace(duration))
            {
                return TimeSpan.Zero;
            }

            var parts = duration.Split(':').Select(x => int.TryParse(x, out var value) ? value : 0).ToArray();
            return parts.Length switch
            {
                3 => new TimeSpan(parts[0], parts[1], parts[2]),
                2 => new TimeSpan(0, parts[0], parts[1]),
                _ => TimeSpan.Zero
            };
        }

        private static string FormatDuration(TimeSpan duration)
        {
            return duration.TotalHours >= 1
                ? $"{(int)duration.TotalHours:00}:{duration.Minutes:00}:{duration.Seconds:00}"
                : $"{duration.Minutes:00}:{duration.Seconds:00}";
        }

        private static string? Truncate(string? value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            var trimmed = value.Trim();
            return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength];
        }
    }
}
