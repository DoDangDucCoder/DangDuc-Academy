using HDNXUdemyModel.Base;
using HDNXUdemyModel.Model;

namespace HDNXUdemyServices.IServices
{
    public interface IPayoutService
    {
        Task<decimal> GetInstructorBalanceAsync(long instructorId);
        Task<bool> RequestPayoutAsync(long instructorId, decimal amount);
        Task<List<PayoutRequestModel>> GetPendingPayoutsAsync();
        Task<bool> ProcessPayoutAsync(long payoutRequestId, bool isApproved, string adminNote);
    }
}
