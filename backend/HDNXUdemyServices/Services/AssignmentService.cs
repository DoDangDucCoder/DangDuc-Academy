using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HDNXUdemyData.Entities;
using HDNXUdemyData.IRepository;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.Model;
using HDNXUdemyServices.IServices;

namespace HDNXUdemyServices.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IAssignmentSubmissionRepository _submissionRepository;

        public AssignmentService(IAssignmentRepository assignmentRepository, IAssignmentSubmissionRepository submissionRepository)
        {
            _assignmentRepository = assignmentRepository;
            _submissionRepository = submissionRepository;
        }

        public async Task<RepositoryModel<AssignmentModel>> CreateAssignment(AssignmentModel model)
        {
            var entity = new AssignmentEntities
            {
                IdCourse = model.IdCourse,
                Title = model.Title,
                Description = model.Description,
                MaxScore = model.MaxScore,
                DueDate = model.DueDate,
                Status = (int)EStatus.Active
            };

            await _assignmentRepository.AddAsync(entity);
            model.Id = entity.Id;
            return new RepositoryModel<AssignmentModel>(ERetCode.Successfull, model) { StatusCode = 200, SystemMessage = "Success" };
        }

        public async Task<RepositoryModel<AssignmentModel>> UpdateAssignment(AssignmentModel model)
        {
            if (model.Id == null)
                return new RepositoryModel<AssignmentModel>(ERetCode.SystemError, model) { StatusCode = 400, SystemMessage = "Id is null" };

            var entity = await _assignmentRepository.GetByIdAsync(model.Id.Value);
            if (entity == null)
            {
                return new RepositoryModel<AssignmentModel>(ERetCode.SystemError, model) { StatusCode = 404, SystemMessage = "Assignment not found" };
            }

            entity.Title = model.Title;
            entity.Description = model.Description;
            entity.MaxScore = model.MaxScore;
            entity.DueDate = model.DueDate;

            await _assignmentRepository.UpdateAsync(entity);
            return new RepositoryModel<AssignmentModel>(ERetCode.Successfull, model) { StatusCode = 200, SystemMessage = "Success" };
        }

        public async Task<RepositoryModel<bool>> DeleteAssignment(long idAssignment)
        {
            var entity = await _assignmentRepository.GetByIdAsync(idAssignment);
            if (entity != null)
            {
                entity.Status = (int)EStatus.Inactive;
                await _assignmentRepository.UpdateAsync(entity);
            }
            return new RepositoryModel<bool>(ERetCode.Successfull, true) { StatusCode = 200, SystemMessage = "Success" };
        }

        public async Task<RepositoryModel<List<AssignmentModel>>> GetAssignmentsByCourse(long idCourse)
        {
            var entities = await _assignmentRepository.GetAllAsync();
            var list = entities.Where(x => x.IdCourse == idCourse && x.Status != (int)EStatus.Inactive)
                               .Select(x => new AssignmentModel
                               {
                                   Id = x.Id,
                                   IdCourse = x.IdCourse,
                                   Title = x.Title,
                                   Description = x.Description,
                                   MaxScore = x.MaxScore,
                                   DueDate = x.DueDate,
                                   Status = x.Status ?? (int)EStatus.Active
                               }).ToList();

            return new RepositoryModel<List<AssignmentModel>>(ERetCode.Successfull, list) { StatusCode = 200, SystemMessage = "Success" };
        }

        public async Task<RepositoryModel<AssignmentSubmissionModel>> SubmitAssignment(AssignmentSubmissionModel model)
        {
            var existing = (await _submissionRepository.GetAllAsync())
                            .FirstOrDefault(x => x.IdAssignment == model.IdAssignment && x.IdUser == model.IdUser && x.Status != (int)EStatus.Inactive);
            
            if (existing != null)
            {
                existing.SubmissionText = model.SubmissionText;
                existing.FileUrl = model.FileUrl;
                existing.SubmittedAt = DateTime.UtcNow;
                await _submissionRepository.UpdateAsync(existing);
                model.Id = existing.Id;
            }
            else
            {
                var entity = new AssignmentSubmissionEntities
                {
                    IdAssignment = model.IdAssignment,
                    IdUser = model.IdUser,
                    SubmissionText = model.SubmissionText,
                    FileUrl = model.FileUrl,
                    SubmittedAt = DateTime.UtcNow,
                    Status = (int)EStatus.Active
                };
                await _submissionRepository.AddAsync(entity);
                model.Id = entity.Id;
            }

            return new RepositoryModel<AssignmentSubmissionModel>(ERetCode.Successfull, model) { StatusCode = 200, SystemMessage = "Success" };
        }

        public async Task<RepositoryModel<List<AssignmentSubmissionModel>>> GetSubmissionsByAssignment(long idAssignment)
        {
            var entities = await _submissionRepository.GetAllAsync();
            var list = entities.Where(x => x.IdAssignment == idAssignment && x.Status != (int)EStatus.Inactive)
                               .Select(x => new AssignmentSubmissionModel
                               {
                                   Id = x.Id,
                                   IdAssignment = x.IdAssignment,
                                   IdUser = x.IdUser,
                                   SubmissionText = x.SubmissionText,
                                   FileUrl = x.FileUrl,
                                   SubmittedAt = x.SubmittedAt,
                                   Score = x.Score,
                                   Feedback = x.Feedback,
                                   Status = x.Status ?? (int)EStatus.Active
                               }).ToList();

            return new RepositoryModel<List<AssignmentSubmissionModel>>(ERetCode.Successfull, list) { StatusCode = 200, SystemMessage = "Success" };
        }

        public async Task<RepositoryModel<AssignmentSubmissionModel>> GetSubmissionByUserAndAssignment(long idAssignment, long idUser)
        {
            var entities = await _submissionRepository.GetAllAsync();
            var entity = entities.FirstOrDefault(x => x.IdAssignment == idAssignment && x.IdUser == idUser && x.Status != (int)EStatus.Inactive);
            if (entity == null)
            {
                return new RepositoryModel<AssignmentSubmissionModel>(ERetCode.SystemError, null) { StatusCode = 404, SystemMessage = "Not found" };
            }

            var model = new AssignmentSubmissionModel
            {
                Id = entity.Id,
                IdAssignment = entity.IdAssignment,
                IdUser = entity.IdUser,
                SubmissionText = entity.SubmissionText,
                FileUrl = entity.FileUrl,
                SubmittedAt = entity.SubmittedAt,
                Score = entity.Score,
                Feedback = entity.Feedback,
                Status = entity.Status ?? (int)EStatus.Active
            };

            return new RepositoryModel<AssignmentSubmissionModel>(ERetCode.Successfull, model) { StatusCode = 200, SystemMessage = "Success" };
        }

        public async Task<RepositoryModel<AssignmentSubmissionModel>> GradeSubmission(long idSubmission, int score, string feedback)
        {
            var entity = await _submissionRepository.GetByIdAsync(idSubmission);
            if (entity == null)
            {
                return new RepositoryModel<AssignmentSubmissionModel>(ERetCode.SystemError, null) { StatusCode = 404, SystemMessage = "Not found" };
            }

            entity.Score = score;
            entity.Feedback = feedback;

            await _submissionRepository.UpdateAsync(entity);

            var model = new AssignmentSubmissionModel
            {
                Id = entity.Id,
                IdAssignment = entity.IdAssignment,
                IdUser = entity.IdUser,
                SubmissionText = entity.SubmissionText,
                FileUrl = entity.FileUrl,
                SubmittedAt = entity.SubmittedAt,
                Score = entity.Score,
                Feedback = entity.Feedback,
                Status = entity.Status ?? (int)EStatus.Active
            };
            return new RepositoryModel<AssignmentSubmissionModel>(ERetCode.Successfull, model) { StatusCode = 200, SystemMessage = "Success" };
        }
    }
}
