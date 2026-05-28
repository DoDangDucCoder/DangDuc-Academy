using Asp.Versioning;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyModel.SystemExceptions;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HDNXUdemyConvertVideoAPI.Controllers
{
    /// <summary>
    /// UploadVideoDataToServerController
    /// </summary>
    [ApiVersion(VersionApi.Version1)]
    [Route(RouterControllerName.UploadVideoToClound)]
    [Authorize(Roles = "Admin")]
    public class UploadVideoDataToServerController : BaseController
    {
        private readonly IUploadFileVideoToServer _uploadFileVideoToServer;

        /// <summary>
        /// UploadVideoDataToServerController
        /// </summary>
        /// <param name="uploadFileVideoToServer"></param>
        /// <exception cref="ProjectException"></exception>
        public UploadVideoDataToServerController(IUploadFileVideoToServer uploadFileVideoToServer)
        {
            _uploadFileVideoToServer = uploadFileVideoToServer ?? throw new ProjectException(nameof(_uploadFileVideoToServer));
        }

        /// <summary>
        /// UploadVideoToServer
        /// </summary>
        /// <returns></returns>
        [HttpPost("video-course")]
        public async Task<IActionResult> UploadVideoToServer()
        {
            RepositoryModel<ReturnUploadFile> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new ReturnUploadFile(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };
            var folderUpload = ProjectConfig.StorageMainVideo ?? string.Empty;
            var files = Request.HasFormContentType ? Request.Form.Files : null;
            var invalidResult = ValidateUpload(files, VideoExtensions, VideoContentTypes, 500 * 1024 * 1024);
            if (invalidResult != null)
            {
                return BadRequest(invalidResult);
            }

            var storageUnavailable = ValidateStorage(folderUpload);
            if (storageUnavailable != null)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, storageUnavailable);
            }

            result.Data = await _uploadFileVideoToServer.UploadVideoFileToServer(files![0], folderUpload, Request);
            if (!result.Data.IsUpload)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, StorageUnavailable("Storage or video conversion is unavailable."));
            }

            result.StatusCode = (int)HttpStatusCode.OK;
            return Ok(result);
        }

        /// <summary>
        /// UploadFileDocumentToServer
        /// </summary>
        /// <returns></returns>
        [HttpPost("file-document")]
        public async Task<IActionResult> UploadFileDocumentToServer()
        {
            RepositoryModel<ReturnUploadFile> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new ReturnUploadFile(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };
            var folderUpload = ProjectConfig.UploadSoftWareAndFile ?? string.Empty;
            var files = Request.HasFormContentType ? Request.Form.Files : null;
            var invalidResult = ValidateUpload(files, DocumentExtensions, null, 100 * 1024 * 1024);
            if (invalidResult != null)
            {
                return BadRequest(invalidResult);
            }

            var storageUnavailable = ValidateStorage(folderUpload);
            if (storageUnavailable != null)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, storageUnavailable);
            }

            result.Data = await _uploadFileVideoToServer.UploadVideoFileToServer(files![0], folderUpload, Request);
            if (!result.Data.IsUpload)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, StorageUnavailable("Storage or video conversion is unavailable."));
            }

            result.StatusCode = (int)HttpStatusCode.OK;
            return Ok(result);
        }

        /// <summary>
        /// UploadVideoMp4FileToServer
        /// </summary>
        /// <returns></returns>
        [HttpPost("mp4-video-course")]
        public async Task<IActionResult> UploadVideoMp4FileToServer()
        {
            RepositoryModel<ReturnUploadFile> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new ReturnUploadFile(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };
            var folderUpload = ProjectConfig.StorageMainVideo ?? string.Empty;
            var files = Request.HasFormContentType ? Request.Form.Files : null;
            var invalidResult = ValidateUpload(files, Mp4Extensions, Mp4ContentTypes, 500 * 1024 * 1024);
            if (invalidResult != null)
            {
                return BadRequest(invalidResult);
            }

            var storageUnavailable = ValidateStorage(folderUpload);
            if (storageUnavailable != null)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, storageUnavailable);
            }

            result.Data = await _uploadFileVideoToServer.UploadVideoMp4FileToServer(files![0], folderUpload, Request);
            if (!result.Data.IsUpload)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, StorageUnavailable("Storage is unavailable."));
            }

            result.StatusCode = (int)HttpStatusCode.OK;
            return Ok(result);
        }

        private static readonly HashSet<string> VideoExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".mp4", ".mov", ".avi", ".mkv", ".webm"
        };

        private static readonly HashSet<string> Mp4Extensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".mp4"
        };

        private static readonly HashSet<string> DocumentExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".zip", ".rar", ".7z"
        };

        private static readonly HashSet<string> VideoContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "video/mp4", "video/quicktime", "video/x-msvideo", "video/x-matroska", "video/webm", "application/octet-stream"
        };

        private static readonly HashSet<string> Mp4ContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "video/mp4", "application/octet-stream"
        };

        private static RepositoryModel<ReturnUploadFile>? ValidateUpload(
            IFormFileCollection? files,
            HashSet<string> allowedExtensions,
            HashSet<string>? allowedContentTypes,
            long maxBytes)
        {
            if (files == null || files.Count == 0)
            {
                return InvalidUpload("A file is required.");
            }

            var file = files[0];
            if (file.Length <= 0)
            {
                return InvalidUpload("File is empty.");
            }

            if (file.Length > maxBytes)
            {
                return InvalidUpload("File is too large.");
            }

            string extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !allowedExtensions.Contains(extension))
            {
                return InvalidUpload("File type is not allowed.");
            }

            if (allowedContentTypes != null && !allowedContentTypes.Contains(file.ContentType))
            {
                return InvalidUpload("File content type is not allowed.");
            }

            return null;
        }

        private static RepositoryModel<ReturnUploadFile> InvalidUpload(string message)
        {
            return new RepositoryModel<ReturnUploadFile>
            {
                PartnerCode = InterCode.ErrorMiddleware,
                RetCode = ERetCode.BadRequest,
                Data = new ReturnUploadFile { IsUpload = false, FileName = string.Empty },
                StatusCode = (int)HttpStatusCode.BadRequest,
                SystemMessage = message
            };
        }

        private static RepositoryModel<ReturnUploadFile>? ValidateStorage(string folderUpload)
        {
            string basePath = ProjectConfig.DiskBaseForVideo ?? string.Empty;
            if (string.IsNullOrWhiteSpace(basePath) || string.IsNullOrWhiteSpace(folderUpload))
            {
                return StorageUnavailable("Storage path is not configured.");
            }

            string root = Path.GetPathRoot(basePath) ?? basePath;
            if (!Directory.Exists(root))
            {
                return StorageUnavailable("Storage path is unavailable.");
            }

            try
            {
                string folder = Path.Combine(basePath, folderUpload);
                Directory.CreateDirectory(folder);
                string probeFile = Path.Combine(folder, $".write-test-{Guid.NewGuid():N}.tmp");
                System.IO.File.WriteAllText(probeFile, "ok");
                System.IO.File.Delete(probeFile);
                return null;
            }
            catch
            {
                return StorageUnavailable("Storage path is not writable.");
            }
        }

        private static RepositoryModel<ReturnUploadFile> StorageUnavailable(string message)
        {
            return new RepositoryModel<ReturnUploadFile>
            {
                PartnerCode = InterCode.ErrorMiddleware,
                RetCode = ERetCode.SystemError,
                Data = new ReturnUploadFile { IsUpload = false, FileName = string.Empty },
                StatusCode = (int)HttpStatusCode.ServiceUnavailable,
                SystemMessage = message
            };
        }
    }
}
