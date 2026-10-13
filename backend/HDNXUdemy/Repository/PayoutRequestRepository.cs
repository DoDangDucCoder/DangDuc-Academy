
using HDNXUdemyData.Entities;
using HDNXUdemyData.GenericRepository;
using HDNXUdemyData.IRepository;
using Microsoft.AspNetCore.Http;

namespace HDNXUdemyData.Repository
{
    public class PayoutRequestRepository : GenericRepository<PayoutRequestEntities>, IPayoutRequestRepository
    {
        public PayoutRequestRepository(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
        }
    }
}
