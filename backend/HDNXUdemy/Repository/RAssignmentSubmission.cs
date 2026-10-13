using HDNXUdemyData.Entities;
using HDNXUdemyData.GenericRepository;
using HDNXUdemyData.IRepository;
using Microsoft.AspNetCore.Http;

namespace HDNXUdemyData.Repository
{
    public class AssignmentSubmissionRepository : GenericRepository<AssignmentSubmissionEntities>, IAssignmentSubmissionRepository
    {
        public AssignmentSubmissionRepository(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
    }
}
