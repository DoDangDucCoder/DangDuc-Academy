using HDNXUdemyData.Entities;
using HDNXUdemyData.EntitiesContext;
using HDNXUdemyData.GenericRepository;
using HDNXUdemyData.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HDNXUdemyData.Repository
{
    public class RefundRequestRepository : GenericRepository<RefundRequestEntities>, IRRefundRequest
    {
        public RefundRequestRepository(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
    }
}
