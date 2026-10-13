using System.Collections.Generic;
using System.Threading.Tasks;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Model;

namespace HDNXUdemyServices.IServices
{
    public interface IAssignmentService
    {
        Task<RepositoryModel<AssignmentModel>> CreateAssignment(AssignmentModel model);
        Task<RepositoryModel<AssignmentModel>> UpdateAssignment(AssignmentModel model);
        Task<RepositoryModel<bool>> DeleteAssignment(long idAssignment);
        Task<RepositoryModel<List<AssignmentModel>>> GetAssignmentsByCourse(long idCourse);
        
        Task<RepositoryModel<AssignmentSubmissionModel>> SubmitAssignment(AssignmentSubmissionModel model);
        Task<RepositoryModel<List<AssignmentSubmissionModel>>> GetSubmissionsByAssignment(long idAssignment);
        Task<RepositoryModel<AssignmentSubmissionModel>> GetSubmissionByUserAndAssignment(long idAssignment, long idUser);
        Task<RepositoryModel<AssignmentSubmissionModel>> GradeSubmission(long idSubmission, int score, string feedback);
    }
}
