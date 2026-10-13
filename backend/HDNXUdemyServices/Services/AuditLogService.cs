using HDNXUdemyData.Entities;
using HDNXUdemyData.EntitiesContext;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.Model;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using System.Text.Json;

namespace HDNXUdemyServices.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly ProjectContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = new ProjectContext(httpContextAccessor);
        }

        public async Task WriteAsync(long? userId, string action, string entityName, string? entityId, object? oldValue, object? newValue)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            _context.AuditLogs!.Add(new AuditLogEntities
            {
                UserId = userId,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                OldValue = oldValue == null ? null : JsonSerializer.Serialize(oldValue),
                NewValue = newValue == null ? null : JsonSerializer.Serialize(newValue),
                IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                UserAgent = request?.Headers.UserAgent.ToString(),
                CreatedAt = LocalDateTime.FromDateTime(DateTime.UtcNow),
                Status = (int)EStatus.Active
            });
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<AuditLogModel>> GetLogs(int pageIndex, int pageSize, string? action, string? entityName, long? userId, string? fromDate, string? toDate)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 100);
            var query = _context.AuditLogs!.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(action))
            {
                query = query.Where(x => x.Action.Contains(action));
            }
            if (!string.IsNullOrWhiteSpace(entityName))
            {
                query = query.Where(x => x.EntityName.Contains(entityName));
            }
            if (userId.HasValue && userId.Value > 0)
            {
                query = query.Where(x => x.UserId == userId.Value);
            }
            if (TryParseLocalDate(fromDate, out var parsedFrom))
            {
                query = query.Where(x => x.CreatedAt >= parsedFrom);
            }
            if (TryParseLocalDate(toDate, out var parsedTo))
            {
                query = query.Where(x => x.CreatedAt <= parsedTo.PlusDays(1));
            }

            var total = await query.CountAsync();
            var data = await query.OrderByDescending(x => x.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<AuditLogModel>
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                RowCount = total,
                PageCount = (int)Math.Ceiling(total / (double)pageSize),
                Results = data.Select(x => new AuditLogModel
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Action = x.Action,
                    EntityName = x.EntityName,
                    EntityId = x.EntityId,
                    OldValue = x.OldValue,
                    NewValue = x.NewValue,
                    IpAddress = x.IpAddress,
                    UserAgent = x.UserAgent,
                    CreatedAt = x.CreatedAt,
                    Status = x.Status ?? (int)EStatus.Active
                }).ToList()
            };
        }

        private static bool TryParseLocalDate(string? value, out LocalDateTime localDateTime)
        {
            localDateTime = default;
            if (string.IsNullOrWhiteSpace(value) || !DateTime.TryParse(value, out var dateTime))
            {
                return false;
            }

            localDateTime = LocalDateTime.FromDateTime(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc));
            return true;
        }
    }
}
