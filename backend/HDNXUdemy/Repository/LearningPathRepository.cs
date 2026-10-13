using HDNXUdemyData.Entities;
using HDNXUdemyData.GenericRepository;
using HDNXUdemyData.IRepository;
using Microsoft.AspNetCore.Http;

namespace HDNXUdemyData.Repository
{
    public class LearningPathRepository : GenericRepository<LearningPathEntities>, ILearningPathRepository
    {
        public LearningPathRepository(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor) { }
    }
}
