using HDNXUdemyData.EntitiesContext;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HDNXUdemyServices.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly ProjectContext _context;

        public AdminDashboardService(IHttpContextAccessor httpContextAccessor)
        {
            _context = new ProjectContext(httpContextAccessor);
        }

        public async Task<AdminDashboardModel> GetDashboardAsync()
        {
            var model = new AdminDashboardModel();

            // Total Students
            model.TotalStudents = await _context.Users!
                .Where(x => x.Status == (int)EStatus.Active && x.RoleId == (int)ERoles.Student)
                .CountAsync();

            // Total Teachers
            model.TotalTeachers = await _context.Users!
                .Where(x => x.Status == (int)EStatus.Active && x.RoleId == (int)ERoles.Teacher)
                .CountAsync();

            // Total Courses
            model.TotalCourses = await _context.Courses!
                .Where(x => x.Status == (int)EStatus.Active)
                .CountAsync();

            // Total Revenue
            var successfulPurchases = await _context.PurcharseCourses!
                .Where(x => x.Status == (int)EStatus.Active && x.PurcharseStatus == (int)ETypeOfStatusOrder.Payment)
                .ToListAsync();

            model.TotalRevenue = successfulPurchases.Sum(x => x.TotalPrice ?? 0);

            // Recent Orders
            var recentPurchases = successfulPurchases
                .OrderByDescending(x => x.Id)
                .Take(10)
                .ToList();

            foreach (var purchase in recentPurchases)
            {
                var student = await _context.Users!.FirstOrDefaultAsync(x => x.Id == purchase.IdStudent);
                
                var purchaseDetail = await _context.PurcharseCourseDetails!
                    .FirstOrDefaultAsync(x => x.IdPurchaseOrder == purchase.Id);
                
                string courseName = "Unknown Course";
                if (purchaseDetail != null)
                {
                    var course = await _context.Courses!.FirstOrDefaultAsync(x => x.Id == purchaseDetail.IdCourse);
                    if (course != null)
                    {
                        courseName = course.Title ?? "Unknown Course";
                    }
                }

                model.RecentOrders.Add(new RecentOrderModel
                {
                    OrderId = purchase.Id,
                    CourseName = courseName,
                    StudentName = student?.Name ?? "Unknown Student",
                    Amount = purchase.TotalPrice ?? 0,
                    OrderDate = purchase.PurchaseDate?.ToDateTimeUnspecified() ?? purchase.CreateDate.ToDateTimeUnspecified(),
                    Status = "Completed"
                });
            }

            return model;
        }
    }
}
