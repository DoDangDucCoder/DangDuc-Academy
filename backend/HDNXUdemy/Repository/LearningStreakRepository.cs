using HDNXUdemyData.Entities;
using HDNXUdemyData.GenericRepository;
using HDNXUdemyData.IRepository;
using Microsoft.AspNetCore.Http;

namespace HDNXUdemyData.Repository
{
    public class LearningStreakRepository : GenericRepository<LearningStreakEntities>, ILearningStreakRepository
    {
        public LearningStreakRepository(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor) { }
    }
}
