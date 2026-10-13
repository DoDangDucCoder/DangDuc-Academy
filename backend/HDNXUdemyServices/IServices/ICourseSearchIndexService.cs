using HDNXUdemyModel.Base;
using HDNXUdemyModel.Model;
using HDNXUdemyModel.RequestModel;

namespace HDNXUdemyServices.IServices
{
    public interface ICourseSearchIndexService
    {
        Task IndexCourse(CourseModel model);
        Task DeleteCourse(long idCourse);
        Task<PagedResult<CourseModel>?> SearchCourses(SearchCoursesRequest request, int pageIndex, int pageSize);
        Task<int> ReindexAllCourses();
    }
}
