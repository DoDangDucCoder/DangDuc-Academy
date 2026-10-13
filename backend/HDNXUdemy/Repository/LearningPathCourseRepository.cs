using HDNXUdemyData.Entities;
using HDNXUdemyData.GenericRepository;
using HDNXUdemyData.IRepository;
using Microsoft.AspNetCore.Http;

namespace HDNXUdemyData.Repository
{
    public class LearningPathCourseRepository : GenericRepository<LearningPathCourseEntities>, ILearningPathCourseRepository
    {
        public LearningPathCourseRepository(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor) { }
    }
}
