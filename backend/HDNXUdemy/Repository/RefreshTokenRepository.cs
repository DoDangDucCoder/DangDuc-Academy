using HDNXUdemyData.Entities;
using HDNXUdemyData.GenericRepository;
using HDNXUdemyData.IRepository;
using Microsoft.AspNetCore.Http;

namespace HDNXUdemyData.Repository
{
    public class RefreshTokenRepository : GenericRepository<RefreshTokenEntities>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
    }
}
