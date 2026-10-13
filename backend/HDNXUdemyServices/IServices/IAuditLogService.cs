using HDNXUdemyModel.Base;
using HDNXUdemyModel.Model;

namespace HDNXUdemyServices.IServices
{
    public interface IAuditLogService
    {
        Task WriteAsync(long? userId, string action, string entityName, string? entityId, object? oldValue, object? newValue);
        Task<PagedResult<AuditLogModel>> GetLogs(int pageIndex, int pageSize, string? action, string? entityName, long? userId, string? fromDate, string? toDate);
    }
}
