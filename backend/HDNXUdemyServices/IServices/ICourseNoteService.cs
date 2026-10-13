using HDNXUdemyModel.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HDNXUdemyServices.IServices
{
    public interface ICourseNoteService
    {
        Task<List<CourseNoteModel>> GetNotesByCourseAsync(long studentId, long courseId);
        Task<CourseNoteModel> AddNoteAsync(CourseNoteModel model);
        Task<CourseNoteModel> UpdateNoteAsync(long noteId, CourseNoteModel model);
        Task<bool> DeleteNoteAsync(long noteId, long studentId);
    }
}
