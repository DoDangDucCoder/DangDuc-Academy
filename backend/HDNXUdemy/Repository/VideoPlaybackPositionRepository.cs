using HDNXUdemyData.Entities;
using HDNXUdemyData.GenericRepository;
using HDNXUdemyData.IRepository;
using Microsoft.AspNetCore.Http;

namespace HDNXUdemyData.Repository
{
    public class VideoPlaybackPositionRepository : GenericRepository<VideoPlaybackPositionEntities>, IVideoPlaybackPositionRepository
    {
        public VideoPlaybackPositionRepository(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
    }
}
