using HDNXUdemyData.Entities;
using HDNXUdemyData.GenericRepository;
using HDNXUdemyData.IRepository;
using Microsoft.AspNetCore.Http;

namespace HDNXUdemyData.Repository
{
    public class VideoEngagementRepository : GenericRepository<VideoEngagementEntities>, IVideoEngagementRepository
    {
        public VideoEngagementRepository(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
    }
}
