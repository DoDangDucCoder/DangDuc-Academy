using HDNXUdemyData.Entities;
using HDNXUdemyData.IRepository;
using HDNXUdemyModel.Model;
using HDNXUdemyServices.IServices;
using NodaTime;

namespace HDNXUdemyServices.Services
{
    public class PayoutService : IPayoutService
    {
        private readonly IPayoutRequestRepository _payoutRequestRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IRPPurcharseCourseDetailsRepository _purcharseCourseDetailsRepository;
        private readonly IPurcharseCourseRepository _purcharseCourseRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly INotificationRepository _notificationRepository;

        public PayoutService(
            IPayoutRequestRepository payoutRequestRepository,
            ICourseRepository courseRepository,
            IRPPurcharseCourseDetailsRepository purcharseCourseDetailsRepository,
            IPurcharseCourseRepository purcharseCourseRepository,
            IAuditLogRepository auditLogRepository,
            INotificationRepository notificationRepository)
        {
            _payoutRequestRepository = payoutRequestRepository;
            _courseRepository = courseRepository;
            _purcharseCourseDetailsRepository = purcharseCourseDetailsRepository;
            _purcharseCourseRepository = purcharseCourseRepository;
            _auditLogRepository = auditLogRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<decimal> GetInstructorBalanceAsync(long instructorId)
        {
            // 1. Get all courses by instructor
            var courses = await _courseRepository.GetAsync(c => c.IdAuthor == instructorId);
            var courseIds = courses.Select(c => c.Id).ToList();

            if (!courseIds.Any()) return 0;

            // 2. Get all successful purchases for these courses
            var purchaseDetails = await _purcharseCourseDetailsRepository.GetAsync(pd => courseIds.Contains(pd.IdCourse));
            var purchaseIds = purchaseDetails.Select(pd => pd.IdPurchaseOrder).Distinct().ToList();

            var successfulPurchases = await _purcharseCourseRepository.GetAsync(p => purchaseIds.Contains(p.Id) && p.PurcharseStatus == 1);
            var successfulPurchaseIds = successfulPurchases.Select(p => p.Id).ToList();

            // 3. Calculate total revenue (70% share)
            decimal totalRevenue = 0;
            foreach (var detail in purchaseDetails.Where(pd => successfulPurchaseIds.Contains(pd.IdPurchaseOrder)))
            {
                var course = courses.FirstOrDefault(c => c.Id == detail.IdCourse);
                if (course != null)
                {
                    totalRevenue += course.PriceOfCourse * 0.7m;
                }
            }

            // 4. Subtract approved and pending payouts
            var payouts = await _payoutRequestRepository.GetAsync(p => p.IdInstructor == instructorId && p.PayoutStatus != 2);
            decimal totalPaid = payouts.Sum(p => p.Amount);

            return totalRevenue - totalPaid;
        }

        public async Task<bool> RequestPayoutAsync(long instructorId, decimal amount)
        {
            // Re-check balance to prevent race conditions
            var balance = await GetInstructorBalanceAsync(instructorId);
            if (amount <= 0 || amount > balance)
            {
                return false;
            }

            var request = new PayoutRequestEntities
            {
                IdInstructor = instructorId,
                Amount = amount,
                PayoutStatus = 0,
                RequestDate = DateTime.UtcNow
            };

            return await _payoutRequestRepository.AddAsync(request);
        }

        public async Task<List<PayoutRequestModel>> GetPendingPayoutsAsync()
        {
            var pending = await _payoutRequestRepository.GetAsync(p => p.PayoutStatus == 0);
            return pending.Select(p => new PayoutRequestModel
            {
                Id = p.Id,
                IdInstructor = p.IdInstructor,
                Amount = p.Amount,
                PayoutStatus = p.PayoutStatus,
                RequestDate = p.RequestDate,
                ProcessDate = p.ProcessDate,
                AdminNote = p.AdminNote
            }).ToList();
        }

        public async Task<bool> ProcessPayoutAsync(long payoutRequestId, bool isApproved, string adminNote)
        {
            var request = await _payoutRequestRepository.GetByIdAsync(payoutRequestId);
            if (request == null || request.PayoutStatus != 0) return false;

            request.PayoutStatus = isApproved ? 1 : 2;
            request.AdminNote = adminNote;
            request.ProcessDate = DateTime.UtcNow;

            var result = await _payoutRequestRepository.UpdateAsync(request);

            // Audit Log
            await _auditLogRepository.AddAsync(new AuditLogEntities
            {
                Action = isApproved ? "PayoutApproved" : "PayoutRejected",
                EntityName = "PayoutRequest",
                EntityId = payoutRequestId.ToString(),
                NewValue = $"Amount: {request.Amount}, Status: {(isApproved ? "Approved" : "Rejected")}, Note: {adminNote}",
                CreatedAt = SystemClock.Instance.GetCurrentInstant().InUtc().LocalDateTime
            });

            // Notification to instructor
            await _notificationRepository.AddAsync(new NotificationEntities
            {
                IdStudent = request.IdInstructor,
                IdCourse = 0,
                TypeNotification = 4, // Payout notification type
                ShortComment = isApproved
                    ? $"Your payout request of ${request.Amount} has been approved. Please check your bank account."
                    : $"Your payout request of ${request.Amount} has been rejected. Reason: {adminNote}",
                IsRead = false
            });

            return result;
        }
    }
}
