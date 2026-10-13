using HDNXUdemyModel.Base;
using HDNXUdemyModel.RequestModel;
using HDNXUdemyModel.ResponModel;
using System.Threading.Tasks;

namespace HDNXUdemyServices.IServices
{
    public interface IArticleService
    {
        Task<PagedResult<ArticleResponse>> GetPagedArticles(int pageIndex, int pageSize, string search = null);
        Task<ArticleResponse> GetArticleById(long id);
        Task<ArticleResponse> GetArticleBySlug(string slug);
        Task<ArticleResponse> CreateArticle(ArticleRequest request);
        Task<ArticleResponse> UpdateArticle(long id, ArticleRequest request);
        Task<bool> DeleteArticle(long id);
        Task<bool> IncrementViewCount(string slug);
    }
}
