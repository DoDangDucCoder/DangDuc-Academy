using HDNXUdemyData.Entities;
using HDNXUdemyData.GenericRepository;
using HDNXUdemyData.IRepository;
using Microsoft.AspNetCore.Http;

namespace HDNXUdemyData.Repository
{
    public class PasswordResetTokenRepository : GenericRepository<PasswordResetTokenEntities>, IPasswordResetTokenRepository
    {
        public PasswordResetTokenRepository(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
    }
}
