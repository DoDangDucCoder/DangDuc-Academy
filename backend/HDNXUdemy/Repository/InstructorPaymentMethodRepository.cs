using HDNXUdemyData.Entities;
using HDNXUdemyData.GenericRepository;
using HDNXUdemyData.IRepository;
using Microsoft.AspNetCore.Http;

namespace HDNXUdemyData.Repository
{
    public class InstructorPaymentMethodRepository : GenericRepository<InstructorPaymentMethodEntities>, IInstructorPaymentMethodRepository
    {
        public InstructorPaymentMethodRepository(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
    }
}
