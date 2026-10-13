using HDNXUdemyModel.Base;
using HDNXUdemyModel.Model;
using HDNXUdemyModel.ResponModel;

namespace HDNXUdemyServices.IServices
{
    public interface IRefundService
    {
        Task<RepositoryModel<RefundRequestModel>> RequestRefund(long idStudent, long idPurchaseCourse, string reason);
        Task<RepositoryModel<bool>> ProcessRefund(long idRefundRequest, bool isApproved, string adminNote);
        Task<RepositoryModel<List<RefundRequestModel>>> GetPendingRefunds();
        Task<RepositoryModel<List<RefundRequestModel>>> GetRefundsByStudent(long idStudent);
    }
}
