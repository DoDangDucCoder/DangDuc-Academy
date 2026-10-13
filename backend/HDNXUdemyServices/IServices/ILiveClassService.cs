using HDNXUdemyModel.Model;

namespace HDNXUdemyServices.IServices
{
    public interface ILiveClassService
    {
        Task<LiveClassModel> CreateLiveClass(LiveClassModel model);
        Task<List<LiveClassModel>> GetLiveClasses(long? idCourse, long? idInstructor);
        Task<bool> UpdateLiveClass(long id, LiveClassModel model);
    }
}
