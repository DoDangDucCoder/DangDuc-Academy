using HDNXUdemyModel.Model;
using Microsoft.AspNetCore.Http;

namespace HDNXUdemyServices.IServices
{
    public interface ISubtitleService
    {
        Task<List<SubtitleModel>> GetSubtitlesByContentDetailAsync(long idContentDetail);
        Task<SubtitleModel> AddSubtitleAsync(long idContentDetail, string language, string label, IFormFile file);
        Task<bool> DeleteSubtitleAsync(long id);
    }
}
