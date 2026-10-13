using HDNXUdemyData.Entities;
using HDNXUdemyData.GenericRepository;
using HDNXUdemyData.IRepository;
using Microsoft.AspNetCore.Http;

namespace HDNXUdemyData.Repository
{
    public class AssignmentRepository : GenericRepository<AssignmentEntities>, IAssignmentRepository
    {
        public AssignmentRepository(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
    }
}
