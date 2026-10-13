using HDNXUdemyData.Entities;
using HDNXUdemyData.EntitiesContext;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.Exception;
using HDNXUdemyModel.Model;
using HDNXUdemyModel.SystemExceptions;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HDNXUdemyServices.Services
{
    public class LiveClassService : ILiveClassService
    {
        private readonly ProjectContext _context;
        private readonly IAuditLogService _auditLogService;

        public LiveClassService(IHttpContextAccessor httpContextAccessor, IAuditLogService auditLogService)
        {
            _context = new ProjectContext(httpContextAccessor);
            _auditLogService = auditLogService;
        }

        public async Task<LiveClassModel> CreateLiveClass(LiveClassModel model)
        {
            Validate(model);
            var entity = new LiveClassEntities
            {
                IdCourse = model.IdCourse,
                IdInstructor = model.IdInstructor,
                Title = model.Title,
                Description = model.Description,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                MeetingUrl = model.MeetingUrl,
                Status = (int)EStatus.Active
            };
            _context.LiveClasses!.Add(entity);
            await _context.SaveChangesAsync();
            await _auditLogService.WriteAsync(model.IdInstructor, "LiveClass.Create", "LiveClass", entity.Id.ToString(), null, model);
            return ToModel(entity);
        }

        public async Task<List<LiveClassModel>> GetLiveClasses(long? idCourse, long? idInstructor)
        {
            var query = _context.LiveClasses!.AsNoTracking().AsQueryable();
            if (idCourse.HasValue) query = query.Where(x => x.IdCourse == idCourse.Value);
            if (idInstructor.HasValue) query = query.Where(x => x.IdInstructor == idInstructor.Value);
            return await query.OrderBy(x => x.StartTime).Select(x => ToModel(x)).ToListAsync();
        }

        public async Task<bool> UpdateLiveClass(long id, LiveClassModel model)
        {
            Validate(model);
            var entity = await _context.LiveClasses!.FindAsync(id) ?? throw new ProjectNotFoundException("Live class not found.");
            var oldValue = ToModel(entity);
            entity.IdCourse = model.IdCourse;
            entity.IdInstructor = model.IdInstructor;
            entity.Title = model.Title;
            entity.Description = model.Description;
            entity.StartTime = model.StartTime;
            entity.EndTime = model.EndTime;
            entity.MeetingUrl = model.MeetingUrl;
            entity.Status = model.Status;
            await _context.SaveChangesAsync();
            await _auditLogService.WriteAsync(model.IdInstructor, "LiveClass.Update", "LiveClass", id.ToString(), oldValue, model);
            return true;
        }

        private static void Validate(LiveClassModel model)
        {
            if (model.IdCourse <= 0 || model.IdInstructor <= 0) throw new ProjectBadRequestException("Course and instructor are required.");
            if (string.IsNullOrWhiteSpace(model.Title)) throw new ProjectBadRequestException("Title is required.");
            if (string.IsNullOrWhiteSpace(model.MeetingUrl)) throw new ProjectBadRequestException("MeetingUrl is required.");
            if (model.EndTime <= model.StartTime) throw new ProjectBadRequestException("EndTime must be after StartTime.");
        }

        private static LiveClassModel ToModel(LiveClassEntities entity) => new()
        {
            Id = entity.Id,
            IdCourse = entity.IdCourse,
            IdInstructor = entity.IdInstructor,
            Title = entity.Title,
            Description = entity.Description,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            MeetingUrl = entity.MeetingUrl,
            Status = entity.Status ?? (int)EStatus.Active
        };
    }
}
