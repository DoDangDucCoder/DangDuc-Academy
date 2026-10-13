using HDNXUdemyData.Entities;
using HDNXUdemyData.IRepository;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.RequestModel;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HDNXUdemyServices.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ArticleService(IArticleRepository articleRepository, IHttpContextAccessor httpContextAccessor)
        {
            _articleRepository = articleRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        private long GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == "user-id");
            if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long userId))
            {
                return userId;
            }
            return 0;
        }

        private string GenerateSlug(string title)
        {
            string slug = title.ToLower();
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", " ").Trim();
            slug = slug.Substring(0, slug.Length <= 45 ? slug.Length : 45).Trim();
            slug = Regex.Replace(slug, @"\s", "-");
            return slug;
        }

        public async Task<ArticleResponse> CreateArticle(ArticleRequest request)
        {
            var userId = GetCurrentUserId();
            var slug = GenerateSlug(request.Title) + "-" + DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var article = new Article
            {
                Title = request.Title ?? "",
                Slug = slug,
                Description = request.Description ?? "",
                Content = request.Content ?? "",
                ThumbnailUrl = request.ThumbnailUrl ?? "",
                AuthorId = userId,
                ViewCount = 0,
                Status = (int)EStatus.Active
            };

            await _articleRepository.AddAsync(article);

            return await GetArticleById(article.Id);
        }

        public async Task<bool> DeleteArticle(long id)
        {
            var article = await _articleRepository.GetByIdAsync(id);
            if (article == null) return false;

            article.Status = (int)EStatus.Inactive;
            await _articleRepository.UpdateAsync(article);
            return true;
        }

        public async Task<ArticleResponse> GetArticleById(long id)
        {
            var article = await _articleRepository.GetByIdAsync(id);
            if (article == null || article.Status == (int)EStatus.Inactive) return null!;

            return new ArticleResponse
            {
                Id = article.Id,
                Title = article.Title,
                Slug = article.Slug,
                Description = article.Description,
                Content = article.Content,
                ThumbnailUrl = article.ThumbnailUrl,
                AuthorId = article.AuthorId,
                ViewCount = article.ViewCount,
                CreateDate = article.CreateDate.ToDateTimeUnspecified(),
                UpdateDate = article.UpdateDate.ToDateTimeUnspecified()
            };
        }

        public async Task<ArticleResponse> GetArticleBySlug(string slug)
        {
            var list = await _articleRepository.GetAsync(x => x.Slug == slug && x.Status == (int)EStatus.Active);
            var article = list.FirstOrDefault();
            if (article == null) return null!;

            return new ArticleResponse
            {
                Id = article.Id,
                Title = article.Title,
                Slug = article.Slug,
                Description = article.Description,
                Content = article.Content,
                ThumbnailUrl = article.ThumbnailUrl,
                AuthorId = article.AuthorId,
                ViewCount = article.ViewCount,
                CreateDate = article.CreateDate.ToDateTimeUnspecified(),
                UpdateDate = article.UpdateDate.ToDateTimeUnspecified()
            };
        }

        public async Task<PagedResult<ArticleResponse>> GetPagedArticles(int pageIndex, int pageSize, string search = null)
        {
            var query = await _articleRepository.GetAsync(x => x.Status == (int)EStatus.Active);
            
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Title.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var totalItems = query.Count();
            var items = query.OrderByDescending(x => x.CreateDate)
                             .Skip((pageIndex - 1) * pageSize)
                             .Take(pageSize)
                             .Select(article => new ArticleResponse
                             {
                                 Id = article.Id,
                                 Title = article.Title,
                                 Slug = article.Slug,
                                 Description = article.Description,
                                 Content = article.Content,
                                 ThumbnailUrl = article.ThumbnailUrl,
                                 AuthorId = article.AuthorId,
                                 ViewCount = article.ViewCount,
                                 CreateDate = article.CreateDate.ToDateTimeUnspecified(),
                                 UpdateDate = article.UpdateDate.ToDateTimeUnspecified()
                             }).ToList();

            return new PagedResult<ArticleResponse>
            {
                Results = items,
                RowCount = totalItems,
                CurrentPage = pageIndex,
                PageSize = pageSize,
                PageCount = (int)Math.Ceiling((double)totalItems / pageSize)
            };
        }

        public async Task<bool> IncrementViewCount(string slug)
        {
            var list = await _articleRepository.GetAsync(x => x.Slug == slug);
            var article = list.FirstOrDefault();
            if (article == null) return false;

            article.ViewCount++;
            await _articleRepository.UpdateAsync(article);
            return true;
        }

        public async Task<ArticleResponse> UpdateArticle(long id, ArticleRequest request)
        {
            var article = await _articleRepository.GetByIdAsync(id);
            if (article == null || article.Status == (int)EStatus.Inactive) return null!;

            article.Title = request.Title ?? "";
            article.Description = request.Description ?? "";
            article.Content = request.Content ?? "";
            article.ThumbnailUrl = request.ThumbnailUrl ?? "";

            await _articleRepository.UpdateAsync(article);

            return await GetArticleById(article.Id);
        }
    }
}
