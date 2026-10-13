using HDNXUdemyData.Entities;
using HDNXUdemyData.GenericRepository;
using HDNXUdemyData.IRepository;
using Microsoft.AspNetCore.Http;

namespace HDNXUdemyData.Repository
{
    public class LessonCompletionRepository : GenericRepository<LessonCompletionEntities>, ILessonCompletionRepository
    {
        public LessonCompletionRepository(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor) { }
    }
}
