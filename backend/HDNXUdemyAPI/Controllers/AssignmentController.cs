using Asp.Versioning;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.Model;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HDNXUdemyAPI.Controllers
{
    [ApiVersion(VersionApi.Version1)]
    [Route("api/v{version:apiVersion}/assignment")]
    [Authorize]
    public class AssignmentController : BaseController
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAssignment([FromBody] AssignmentModel model)
        {
            var res = await _assignmentService.CreateAssignment(model);
            return Ok(res);
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateAssignment([FromBody] AssignmentModel model)
        {
            var res = await _assignmentService.UpdateAssignment(model);
            return Ok(res);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAssignment(long id)
        {
            var res = await _assignmentService.DeleteAssignment(id);
            return Ok(res);
        }

        [HttpGet("get-by-course/{idCourse}")]
        public async Task<IActionResult> GetAssignmentsByCourse(long idCourse)
        {
            var res = await _assignmentService.GetAssignmentsByCourse(idCourse);
            return Ok(res);
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitAssignment([FromBody] AssignmentSubmissionModel model)
        {
            var res = await _assignmentService.SubmitAssignment(model);
            return Ok(res);
        }

        [HttpGet("get-submissions/{idAssignment}")]
        public async Task<IActionResult> GetSubmissionsByAssignment(long idAssignment)
        {
            var res = await _assignmentService.GetSubmissionsByAssignment(idAssignment);
            return Ok(res);
        }

        [HttpGet("get-my-submission/{idAssignment}/{idUser}")]
        public async Task<IActionResult> GetMySubmission(long idAssignment, long idUser)
        {
            var res = await _assignmentService.GetSubmissionByUserAndAssignment(idAssignment, idUser);
            return Ok(res);
        }

        [HttpPost("grade/{idSubmission}")]
        public async Task<IActionResult> GradeSubmission(long idSubmission, [FromQuery] int score, [FromQuery] string feedback)
        {
            var res = await _assignmentService.GradeSubmission(idSubmission, score, feedback);
            return Ok(res);
        }
    }
}
