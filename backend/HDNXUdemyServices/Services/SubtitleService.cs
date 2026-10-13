using HDNXUdemyData.Entities;
using HDNXUdemyData.EntitiesContext;
using HDNXUdemyModel.SystemExceptions;
using HDNXUdemyModel.Model;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HDNXUdemyServices.Services
{
    public class SubtitleService : ISubtitleService
    {
        private readonly ProjectContext _context;
        private readonly IUploadDataToCloud _uploadDataToCloud;

        public SubtitleService(ProjectContext context, IUploadDataToCloud uploadDataToCloud)
        {
            _context = context;
            _uploadDataToCloud = uploadDataToCloud;
        }

        public async Task<List<SubtitleModel>> GetSubtitlesByContentDetailAsync(long idContentDetail)
        {
            return await _context.Subtitles!
                .Where(s => s.IdContentDetail == idContentDetail)
                .Select(s => new SubtitleModel
                {
                    Id = s.Id,
                    IdContentDetail = s.IdContentDetail,
                    Language = s.Language,
                    Label = s.Label,
                    FileUrl = s.FileUrl,
                    PublicId = s.PublicId
                }).ToListAsync();
        }

        public async Task<SubtitleModel> AddSubtitleAsync(long idContentDetail, string language, string label, IFormFile file)
        {
            var contentDetail = await _context.ContentCourseDetails!.FindAsync(idContentDetail);
            if (contentDetail == null) throw new ProjectException("ContentDetail not found.");

            // Upload the .vtt file to Cloudinary
            var uploadResult = await _uploadDataToCloud.AddRawFileToCloudAsync(file);

            var entity = new SubtitleEntities
            {
                IdContentDetail = idContentDetail,
                Language = language,
                Label = label,
                FileUrl = uploadResult.UrlPicture,
                PublicId = uploadResult.PublicId
            };

            _context.Subtitles!.Add(entity);
            await _context.SaveChangesAsync();

            return new SubtitleModel
            {
                Id = entity.Id,
                IdContentDetail = entity.IdContentDetail,
                Language = entity.Language,
                Label = entity.Label,
                FileUrl = entity.FileUrl,
                PublicId = entity.PublicId
            };
        }

        public async Task<bool> DeleteSubtitleAsync(long id)
        {
            var subtitle = await _context.Subtitles!.FindAsync(id);
            if (subtitle == null) return false;

            if (!string.IsNullOrEmpty(subtitle.PublicId))
            {
                await _uploadDataToCloud.DeletePhotoToCloudAsync(subtitle.PublicId);
            }

            _context.Subtitles.Remove(subtitle);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
