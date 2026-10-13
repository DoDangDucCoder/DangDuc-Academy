using HDNXUdemyData.Entities;
using HDNXUdemyData.GenericRepository;
using HDNXUdemyData.IRepository;
using Microsoft.AspNetCore.Http;

namespace HDNXUdemyData.Repository
{
    public class XPTransactionRepository : GenericRepository<XPTransactionEntities>, IXPTransactionRepository
    {
        public XPTransactionRepository(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor) { }
    }
}
