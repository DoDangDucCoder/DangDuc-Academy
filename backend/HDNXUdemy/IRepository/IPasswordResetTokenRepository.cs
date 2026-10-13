using HDNXUdemyData.Entities;
using HDNXUdemyData.GenericRepository;

namespace HDNXUdemyData.IRepository
{
    public interface IPasswordResetTokenRepository : IGenericRepository<PasswordResetTokenEntities>
    {
    }
}
