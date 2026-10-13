using HDNXUdemyData.Entities;
using HDNXUdemyData.GenericRepository;
using HDNXUdemyData.IRepository;
using Microsoft.AspNetCore.Http;

namespace HDNXUdemyData.Repository
{
    public class AnnouncementRepository : GenericRepository<AnnouncementEntities>, IAnnouncementRepository
    {
        public AnnouncementRepository(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor) { }
    }
}
