using HDNXUdemyData.Entities;
using HDNXUdemyData.GenericRepository;
using HDNXUdemyData.IRepository;
using Microsoft.AspNetCore.Http;

namespace HDNXUdemyData.Repository
{
    public class UserBadgeRepository : GenericRepository<UserBadgeEntities>, IUserBadgeRepository
    {
        public UserBadgeRepository(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor) { }
    }
}
