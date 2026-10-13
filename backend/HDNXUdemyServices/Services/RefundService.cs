using AutoMapper;
using HDNXUdemyData.Entities;
using HDNXUdemyData.IRepository;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.Model;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyServices.IServices;
using NodaTime;

namespace HDNXUdemyServices.Services
{
    public class RefundService : IRefundService
    {
        private readonly IRRefundRequest _refundRepository;
        private readonly IPurcharseCourseRepository _purchaseCourseRepository;
        private readonly IRPPurcharseCourseDetailsRepository _purchaseDetailsRepository;
        private readonly IStudentProcessRepository _studentProcessRepository;
        private readonly IUserRepository _userRepository;
        private readonly IStripeServices _stripeServices;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public RefundService(
            IRRefundRequest refundRepository,
            IPurcharseCourseRepository purchaseCourseRepository,
            IRPPurcharseCourseDetailsRepository purchaseDetailsRepository,
            IStudentProcessRepository studentProcessRepository,
            IUserRepository userRepository,
            IStripeServices stripeServices,
            IAuditLogRepository auditLogRepository,
            INotificationRepository notificationRepository,
            IMapper mapper)
        {
            _refundRepository = refundRepository;
            _purchaseCourseRepository = purchaseCourseRepository;
            _purchaseDetailsRepository = purchaseDetailsRepository;
            _studentProcessRepository = studentProcessRepository;
            _userRepository = userRepository;
            _stripeServices = stripeServices;
            _auditLogRepository = auditLogRepository;
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public async Task<RepositoryModel<RefundRequestModel>> RequestRefund(long idStudent, long idPurchaseCourse, string reason)
        {
            var purchase = await _purchaseCourseRepository.GetByIdAsync(idPurchaseCourse);
            if (purchase == null || purchase.IdStudent != idStudent)
            {
                return new RepositoryModel<RefundRequestModel> { StatusCode = 404, SystemMessage = "Purchase not found" };
            }

            if (purchase.PurcharseStatus != (int)ETypeOfStatusOrder.Payment)
            {
                return new RepositoryModel<RefundRequestModel> { StatusCode = 400, SystemMessage = "Purchase not eligible for refund" };
            }

            if (purchase.PurchaseDate.HasValue)
            {
                var daysSincePurchase = (SystemClock.Instance.GetCurrentInstant().InUtc().LocalDateTime - purchase.PurchaseDate.Value).Days;
                if (daysSincePurchase > 30)
                {
                    return new RepositoryModel<RefundRequestModel> { StatusCode = 400, SystemMessage = "Refund period expired (30 days)" };
                }
            }

            var existingRequest = await _refundRepository.GetObjectAsync(x => x.IdPurchaseCourse == idPurchaseCourse);
            if (existingRequest != null)
            {
                return new RepositoryModel<RefundRequestModel> { StatusCode = 400, SystemMessage = "Refund already requested" };
            }

            var request = new RefundRequestEntities
            {
                IdStudent = idStudent,
                IdPurchaseCourse = idPurchaseCourse,
                Reason = reason,
                RefundStatus = 0, // Pending
                RequestDate = SystemClock.Instance.GetCurrentInstant().InUtc().LocalDateTime
            };

            await _refundRepository.AddAsync(request);

            return new RepositoryModel<RefundRequestModel>
            {
                StatusCode = 200,
                Data = _mapper.Map<RefundRequestModel>(request)
            };
        }

        public async Task<RepositoryModel<bool>> ProcessRefund(long idRefundRequest, bool isApproved, string adminNote)
        {
            var request = await _refundRepository.GetByIdAsync(idRefundRequest);
            if (request == null || request.RefundStatus != 0)
            {
                return new RepositoryModel<bool> { StatusCode = 404, SystemMessage = "Valid refund request not found" };
            }

            request.AdminNote = adminNote;
            request.ProcessDate = SystemClock.Instance.GetCurrentInstant().InUtc().LocalDateTime;

            if (isApproved)
            {
                var purchase = await _purchaseCourseRepository.GetByIdAsync(request.IdPurchaseCourse);
                if (purchase != null && !string.IsNullOrEmpty(purchase.PaymentIntent))
                {
                    var refundSuccess = await _stripeServices.RefundPayment(purchase.PaymentIntent);
                    if (!refundSuccess)
                    {
                        return new RepositoryModel<bool> { StatusCode = 500, SystemMessage = "Stripe refund failed" };
                    }

                    // Update purchase status to Rejected/Refunded
                    purchase.PurcharseStatus = (int)ETypeOfStatusOrder.Reject;
                    await _purchaseCourseRepository.UpdateAsync(purchase);

                    // Revoke access
                    var details = await _purchaseDetailsRepository.GetAsync(x => x.IdPurchaseOrder == request.IdPurchaseCourse);
                    var courseIds = details.Select(x => x.IdCourse).ToList();
                    var processes = await _studentProcessRepository.GetAsync(x => x.CreateBy == purchase.IdStudent && courseIds.Contains(x.IdCourse));
                    
                    foreach (var process in processes)
                    {
                        process.Status = (int)EStatus.Inactive; // Set to inactive
                        await _studentProcessRepository.UpdateAsync(process);
                    }
                    
                    request.RefundStatus = 1; // Approved
                }
                else
                {
                    return new RepositoryModel<bool> { StatusCode = 400, SystemMessage = "Payment Intent not found" };
                }
            }
            else
            {
                request.RefundStatus = 2; // Rejected
            }

            await _refundRepository.UpdateAsync(request);

            // Audit Log
            await _auditLogRepository.AddAsync(new AuditLogEntities
            {
                Action = isApproved ? "RefundApproved" : "RefundRejected",
                EntityName = "RefundRequest",
                EntityId = idRefundRequest.ToString(),
                NewValue = $"Status: {(isApproved ? "Approved" : "Rejected")}, Note: {adminNote}",
                CreatedAt = SystemClock.Instance.GetCurrentInstant().InUtc().LocalDateTime
            });

            // Notification to student
            await _notificationRepository.AddAsync(new NotificationEntities
            {
                IdStudent = request.IdStudent,
                IdCourse = 0,
                TypeNotification = 3, // Refund notification type
                ShortComment = isApproved
                    ? "Your refund request has been approved. The amount will be returned to your payment method."
                    : $"Your refund request has been rejected. Reason: {adminNote}",
                IsRead = false
            });

            return new RepositoryModel<bool> { StatusCode = 200, Data = true };
        }

        public async Task<RepositoryModel<List<RefundRequestModel>>> GetPendingRefunds()
        {
            var requests = await _refundRepository.GetAsync(x => x.RefundStatus == 0);
            var requestList = requests.ToList();
            var models = _mapper.Map<List<RefundRequestModel>>(requestList);

            if (models.Any())
            {
                // Batch load all related students and purchases in 2 queries instead of 2*N
                var studentIds = models.Select(m => m.IdStudent).Distinct().ToList();
                var purchaseIds = models.Select(m => m.IdPurchaseCourse).Distinct().ToList();

                var students = await _userRepository.GetAsync(u => studentIds.Contains(u.Id));
                var purchases = await _purchaseCourseRepository.GetAsync(p => purchaseIds.Contains(p.Id));

                var studentDict = students.ToDictionary(s => s.Id);
                var purchaseDict = purchases.ToDictionary(p => p.Id);

                foreach (var m in models)
                {
                    if (studentDict.TryGetValue(m.IdStudent, out var student))
                        m.Student = _mapper.Map<UserModel>(student);
                    if (purchaseDict.TryGetValue(m.IdPurchaseCourse, out var purchase))
                        m.PurchaseCourse = _mapper.Map<PurcharseCourseModel>(purchase);
                }
            }

            return new RepositoryModel<List<RefundRequestModel>> { StatusCode = 200, Data = models };
        }

        public async Task<RepositoryModel<List<RefundRequestModel>>> GetRefundsByStudent(long idStudent)
        {
            var requests = await _refundRepository.GetAsync(x => x.IdStudent == idStudent);
            var requestList = requests.ToList();
            var models = _mapper.Map<List<RefundRequestModel>>(requestList);

            if (models.Any())
            {
                // Batch load all related purchases in 1 query instead of N
                var purchaseIds = models.Select(m => m.IdPurchaseCourse).Distinct().ToList();
                var purchases = await _purchaseCourseRepository.GetAsync(p => purchaseIds.Contains(p.Id));
                var purchaseDict = purchases.ToDictionary(p => p.Id);

                foreach (var m in models)
                {
                    if (purchaseDict.TryGetValue(m.IdPurchaseCourse, out var purchase))
                        m.PurchaseCourse = _mapper.Map<PurcharseCourseModel>(purchase);
                }
            }

            return new RepositoryModel<List<RefundRequestModel>> { StatusCode = 200, Data = models };
        }
    }
}
