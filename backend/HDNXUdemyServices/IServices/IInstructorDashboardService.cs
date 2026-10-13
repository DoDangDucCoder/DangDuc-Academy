using HDNXUdemyModel.ResponModel;

namespace HDNXUdemyServices.IServices
{
    public interface IInstructorDashboardService
    {
        Task<InstructorDashboardModel> GetDashboard(long instructorId);
    }
}
