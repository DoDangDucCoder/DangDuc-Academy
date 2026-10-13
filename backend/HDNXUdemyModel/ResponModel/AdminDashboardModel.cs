using System;
using System.Collections.Generic;

namespace HDNXUdemyModel.ResponModel
{
    public class AdminDashboardModel
    {
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalCourses { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<RecentOrderModel> RecentOrders { get; set; } = new List<RecentOrderModel>();
    }

    public class RecentOrderModel
    {
        public long OrderId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
