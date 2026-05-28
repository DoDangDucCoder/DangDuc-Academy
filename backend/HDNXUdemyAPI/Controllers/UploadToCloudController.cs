using Asp.Versioning;
using CloudinaryDotNet.Actions;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.Exception;
using HDNXUdemyModel.Model;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyModel.SystemExceptions;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HDNXUdemyAPI.Controllers
{
    /// <summary>
    /// UploadToCloudController
    /// </summary>
    [ApiVersion(VersionApi.Version1)]
    [Route(RouterControllerName.UploadToCloud)]
    [Authorize(Roles = "Admin")]
    public class UploadToCloudController : BaseController
    {
        private readonly IUploadDataToCloud _uploadDataToCloud;
        private readonly IUploadFileVideoToServer _uploadFileVideoToServer;

        /// <summary>
        /// UploadToCloudController
        /// </summary>
        public UploadToCloudController(IUploadDataToCloud uploadDataToCloud, IUploadFileVideoToServer uploadFileVideoToServer)
        {
            _uploadDataToCloud = uploadDataToCloud ?? throw new ProjectException(nameof(_uploadDataToCloud));
            _uploadFileVideoToServer = uploadFileVideoToServer ?? throw new ProjectException(nameof(_uploadFileVideoToServer));
        }

        /// <summary>
        /// AddPhotoTocloudinaryAsync
        /// </summary>
        /// <param name="filePicture"></param>
        /// <returns></returns>
        [HttpPost("cloudinary")]
        public async Task<RepositoryModel<ResponseUploadWithCloudinary>> AddPhotoToCloudinaryAsync(IFormFile filePicture)
        {
            ValidateImageFile(filePicture);
            RepositoryModel<ResponseUploadWithCloudinary> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new ResponseUploadWithCloudinary(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _uploadDataToCloud.AddPhotoToCloudAsync(filePicture);
            return result;
        }

        /// <summary>
        /// DeletePhotoToCloudinarAsync
        /// </summary>
        /// <param name="publicId"></param>
        /// <returns></returns>
        [HttpDelete("cloudinary/{publicId}")]
        public async Task<RepositoryModel<DeletionResult>> DeletePhotoToCloudinarAsync(string publicId)
        {
            RepositoryModel<DeletionResult> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new DeletionResult(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _uploadDataToCloud.DeletePhotoToCloudAsync(publicId);
            return result;
        }

        /// <summary>
        /// AddPhotoToCloudinaryRequestAsync
        /// </summary>
        /// <returns></returns>
        [HttpPost("cloudinary-request")]
        public async Task<RepositoryModel<ResponseUploadWithCloudinary>> AddPhotoToCloudinaryRequestAsync()
        {
            RepositoryModel<ResponseUploadWithCloudinary> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new ResponseUploadWithCloudinary(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };
            var files = Request.Form.Files;
            if (files == null || files.Count == 0)
            {
                throw new ProjectBadRequestException("File is required.");
            }

            ValidateImageFile(files[0]);
            result.Data = await _uploadDataToCloud.AddPhotoToCloudAsync(files[0]);
            return result;
        }

        /// <summary>
        /// CreateCourse
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("file-software")]
        public async Task<IActionResult> CreateFileSoftware(FileManagerModel model)
        {
            ValidateFileManagerModel(model, requireId: false);
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _uploadFileVideoToServer.CreateFileSoftware(model);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>
        /// UpdateStatusCourse
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("file-software/status/{id}")]
        public async Task<IActionResult> UpdateStatusSoftware(long id, FileManagerModel model)
        {
            ValidateId(id);
            ValidateStatus(model.Status);
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _uploadFileVideoToServer.UpdateStatusSoftware(id, model);
            return Ok(result);
        }

        /// <summary>
        /// UpdateInformationCourse
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("file-software/{id}")]
        public async Task<IActionResult> UpdateInformationSoftware(long id, FileManagerModel model)
        {
            ValidateId(id);
            ValidateFileManagerModel(model, requireId: false);
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _uploadFileVideoToServer.UpdateInformationSoftware(id, model);
            return Ok(result);
        }

        /// <summary>
        /// GetListStudentProcess
        /// </summary>
        /// <returns></returns>
        [HttpGet("file-software")]
        public async Task<IActionResult> GetListFileSoftware()
        {
            RepositoryModel<List<FileManagerModel>> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new List<FileManagerModel>(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _uploadFileVideoToServer.GetListFileSoftware(Request);
            return Ok(result);
        }

        /// <summary>
        /// GetStudentProcess
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("file-software/{id}")]
        public async Task<IActionResult> GetFileSoftware(long id)
        {
            ValidateId(id);
            RepositoryModel<FileManagerModel> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new FileManagerModel(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _uploadFileVideoToServer.GetFileSoftware(id);
            return Ok(result);
        }

        private static void ValidateImageFile(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                throw new ProjectBadRequestException("File is required.");
            }

            const long maxBytes = 10 * 1024 * 1024;
            if (file.Length > maxBytes)
            {
                throw new ProjectBadRequestException("File size exceeds 10 MB.");
            }

            var fileName = Path.GetFileName(file.FileName);
            if (string.IsNullOrWhiteSpace(fileName) || fileName != file.FileName)
            {
                throw new ProjectBadRequestException("Invalid file name.");
            }

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedExtensions.Contains(extension) || !allowedContentTypes.Contains(file.ContentType?.ToLowerInvariant()))
            {
                throw new ProjectBadRequestException("Invalid file type.");
            }
        }

        private static void ValidateFileManagerModel(FileManagerModel model, bool requireId)
        {
            if (model == null)
            {
                throw new ProjectBadRequestException("Payload is required.");
            }

            if (requireId && (!model.Id.HasValue || model.Id.Value <= 0))
            {
                throw new ProjectBadRequestException("Id is invalid.");
            }

            ValidateRequiredString(model.FileName, nameof(model.FileName), 255);
            ValidateRequiredString(model.ActualNameFile, nameof(model.ActualNameFile), 255);
            ValidateRequiredString(model.FileUrl, nameof(model.FileUrl), 1000);
            ValidateRequiredString(model.ExtendFile, nameof(model.ExtendFile), 20);
            ValidateStatus(model.Status);
        }

        private static void ValidateId(long id)
        {
            if (id <= 0)
            {
                throw new ProjectBadRequestException("Id is invalid.");
            }
        }

        private static void ValidateStatus(int status)
        {
            if (!Enum.IsDefined(typeof(EStatus), status))
            {
                throw new ProjectBadRequestException("Status is invalid.");
            }
        }

        private static void ValidateRequiredString(string? value, string fieldName, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ProjectBadRequestException($"{fieldName} is required.");
            }

            if (value.Length > maxLength)
            {
                throw new ProjectBadRequestException($"{fieldName} is too long.");
            }

            if (value.Contains("..") || value.Contains("<script", StringComparison.OrdinalIgnoreCase))
            {
                throw new ProjectBadRequestException($"{fieldName} is invalid.");
            }
        }
    }
}
