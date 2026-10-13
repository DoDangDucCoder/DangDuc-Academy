using HDNXUdemyModel.ResponModel;
using System.Threading.Tasks;

namespace HDNXUdemyServices.IServices
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardModel> GetDashboardAsync();
    }
}
