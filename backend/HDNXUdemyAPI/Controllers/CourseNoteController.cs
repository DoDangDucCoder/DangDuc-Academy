using Asp.Versioning;
using HDNXUdemyModel.Model;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HDNXUdemyAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/CourseNote")]
    [Authorize]
    public class CourseNoteController : BaseController
    {
        private readonly ICourseNoteService _courseNoteService;

        public CourseNoteController(ICourseNoteService courseNoteService)
        {
            _courseNoteService = courseNoteService;
        }

        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetNotesByCourse(long courseId)
        {
            var notes = await _courseNoteService.GetNotesByCourseAsync(CurrentUserId(), courseId);
            return Ok(notes);
        }

        [HttpPost]
        public async Task<IActionResult> AddNote([FromBody] CourseNoteModel model)
        {
            model.IdStudent = CurrentUserId();
            var note = await _courseNoteService.AddNoteAsync(model);
            return Ok(note);
        }

        [HttpPut("{noteId}")]
        public async Task<IActionResult> UpdateNote(long noteId, [FromBody] CourseNoteModel model)
        {
            var note = await _courseNoteService.UpdateNoteAsync(noteId, model);
            return Ok(note);
        }

        [HttpDelete("{noteId}")]
        public async Task<IActionResult> DeleteNote(long noteId)
        {
            var result = await _courseNoteService.DeleteNoteAsync(noteId, CurrentUserId());
            return Ok(result);
        }
    }
}
