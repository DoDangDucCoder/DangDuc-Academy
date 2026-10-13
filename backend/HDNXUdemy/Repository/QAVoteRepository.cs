using HDNXUdemyData.Entities;
using HDNXUdemyData.GenericRepository;
using HDNXUdemyData.IRepository;
using Microsoft.AspNetCore.Http;

namespace HDNXUdemyData.Repository
{
    public class QAVoteRepository : GenericRepository<QAVoteEntities>, IQAVoteRepository
    {
        public QAVoteRepository(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor) { }
    }
}
