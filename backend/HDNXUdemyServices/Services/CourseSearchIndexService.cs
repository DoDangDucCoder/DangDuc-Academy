using HDNXUdemyData.EntitiesContext;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.Model;
using HDNXUdemyModel.RequestModel;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

namespace HDNXUdemyServices.Services
{
    public class CourseSearchIndexService : ICourseSearchIndexService
    {
        private readonly ProjectContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _indexName;

        public CourseSearchIndexService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, HttpClient httpClient)
        {
            _context = new ProjectContext(httpContextAccessor);
            _httpClient = httpClient;
            _baseUrl = configuration.GetValue<string>("Elasticsearch:Url") ?? configuration.GetValue<string>("Elasticsearch__Url") ?? "http://localhost:9200";
            _indexName = configuration.GetValue<string>("Elasticsearch:IndexName") ?? "hdnx-courses";
        }

        public async Task IndexCourse(CourseModel model)
        {
            try
            {
                await _httpClient.PutAsJsonAsync($"{_baseUrl.TrimEnd('/')}/{_indexName}/_doc/{model.Id}", model);
            }
            catch
            {
                // Search indexing is best-effort; DB remains the source of truth.
            }
        }

        public async Task DeleteCourse(long idCourse)
        {
            try
            {
                await _httpClient.DeleteAsync($"{_baseUrl.TrimEnd('/')}/{_indexName}/_doc/{idCourse}");
            }
            catch
            {
            }
        }

        public async Task<PagedResult<CourseModel>?> SearchCourses(SearchCoursesRequest request, int pageIndex, int pageSize)
        {
            try
            {
                var must = new List<object>();
                var filter = new List<object>
                {
                    new { term = new Dictionary<string, object> { ["status"] = (int)EStatus.Active } }
                };

                if (string.IsNullOrWhiteSpace(request.Keyword))
                {
                    must.Add(new { match_all = new { } });
                }
                else
                {
                    must.Add(new
                    {
                        simple_query_string = new
                        {
                            query = request.Keyword,
                            fields = new[] { "title^3", "shortDescription", "description", "categoryName", "levelCourse" },
                            default_operator = "and"
                        }
                    });
                }

                if (request.CategoryId.HasValue && request.CategoryId > 0)
                {
                    filter.Add(new { term = new Dictionary<string, object> { ["idCategory"] = request.CategoryId.Value } });
                }

                if (!string.IsNullOrWhiteSpace(request.Level))
                {
                    filter.Add(new { term = new Dictionary<string, object> { ["levelCourse.keyword"] = request.Level.Trim() } });
                }

                if (request.PriceMin.HasValue || request.PriceMax.HasValue)
                {
                    var range = new Dictionary<string, object>();
                    if (request.PriceMin.HasValue) range["gte"] = request.PriceMin.Value;
                    if (request.PriceMax.HasValue) range["lte"] = request.PriceMax.Value;
                    filter.Add(new { range = new Dictionary<string, object> { ["priceOfCourse"] = range } });
                }

                var sort = request.SortBy?.ToLowerInvariant() switch
                {
                    "price_asc" => new object[] { new { priceOfCourse = new { order = "asc" } } },
                    "price_desc" => new object[] { new { priceOfCourse = new { order = "desc" } } },
                    "newest" => new object[] { new { id = new { order = "desc" } } },
                    _ => new object[] { new { id = new { order = "desc" } } }
                };

                var payload = new
                {
                    from = Math.Max(pageIndex - 1, 0) * pageSize,
                    size = pageSize,
                    query = new
                    {
                        @bool = new
                        {
                            must,
                            filter
                        }
                    },
                    sort
                };
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl.TrimEnd('/')}/{_indexName}/_search", payload);
                if (!response.IsSuccessStatusCode) return null;
                using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                var hits = document.RootElement.GetProperty("hits");
                var total = hits.GetProperty("total").GetProperty("value").GetInt32();
                var results = new List<CourseModel>();
                foreach (var hit in hits.GetProperty("hits").EnumerateArray())
                {
                    var source = hit.GetProperty("_source").Deserialize<CourseModel>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (source != null) results.Add(source);
                }
                return new PagedResult<CourseModel>
                {
                    CurrentPage = pageIndex,
                    PageSize = pageSize,
                    RowCount = total,
                    PageCount = (int)Math.Ceiling(total / (double)pageSize),
                    Results = results
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> ReindexAllCourses()
        {
            var courses = await _context.Courses!.AsNoTracking()
                .Where(x => x.Status == (int)EStatus.Active)
                .Select(x => new CourseModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    ShortDescription = x.ShortDescription,
                    IdAuthor = x.IdAuthor,
                    IdCategory = x.IdCategory,
                    LevelCourse = x.LevelCourse,
                    Description = x.Description,
                    PriceOfCourse = x.PriceOfCourse,
                    PriceOfDiscount = x.PriceOfDiscount,
                    IsFree = x.IsFree,
                    PictureUrl = x.PictureUrl,
                    Status = x.Status ?? (int)EStatus.Active
                })
                .ToListAsync();
            foreach (var course in courses)
            {
                await IndexCourse(course);
            }
            return courses.Count;
        }
    }
}
