using HDNXUdemyData.Entities;
using HDNXUdemyData.GenericRepository;
using HDNXUdemyData.IRepository;
using Microsoft.AspNetCore.Http;

namespace HDNXUdemyData.Repository
{
    public class AchievementBadgeRepository : GenericRepository<AchievementBadgeEntities>, IAchievementBadgeRepository
    {
        public AchievementBadgeRepository(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor) { }
    }
}
