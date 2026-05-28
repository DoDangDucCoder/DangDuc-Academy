using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Exception;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyModel.SystemExceptions;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Http;

namespace HDNXUdemyServices.Services
{
    public class UploadDataToCloud : IUploadDataToCloud
    {
        private Cloudinary _cloudinary;

        public UploadDataToCloud()
        {
            var accountCloud = new Account(ProjectConfig.CloudName, ProjectConfig.APIKey, ProjectConfig.APISecret);
            _cloudinary = new Cloudinary(accountCloud);
        }

        public async Task<ResponseUploadWithCloudinary> AddPhotoToCloudAsync(IFormFile formFile)
        {
            EnsureCloudinaryConfigured();
            if (formFile == null || formFile.Length <= 0)
            {
                throw new ProjectBadRequestException("File is required.");
            }

            try
            {
                var accountCloud = new Account(ProjectConfig.CloudName, ProjectConfig.APIKey, ProjectConfig.APISecret);
                var uploadResult = new ImageUploadResult();
                _cloudinary = new Cloudinary(accountCloud);
                using var stream = formFile.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(formFile.FileName, stream),
                    Transformation = new Transformation().Height(500).Crop("fill").Quality("auto")
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);

                var resultData = new ResponseUploadWithCloudinary()
                {
                    UrlPicture = uploadResult.SecureUrl?.AbsoluteUri ?? string.Empty,
                    PublicId = uploadResult.PublicId
                };

                return resultData;
            }
            catch (Exception ex)
            {
                throw new ProjectAwsException("Cloud storage is unavailable.", ex);
            }
        }

        public async Task<ResponseUploadWithCloudinary> AddPhotoToCloudAsyncByBase64(string imagesBase64)
        {
            EnsureCloudinaryConfigured();
            if (string.IsNullOrWhiteSpace(imagesBase64))
            {
                throw new ProjectBadRequestException("Image is required.");
            }

            try
            {
                var accountCloud = new Account(ProjectConfig.CloudName, ProjectConfig.APIKey, ProjectConfig.APISecret);
                var uploadResult = new ImageUploadResult();
                _cloudinary = new Cloudinary(accountCloud);
                string dataUpload = $"data:image/jpeg;base64,{imagesBase64}";
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(@$"{dataUpload}"),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    Format = "png"
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);

                var resultData = new ResponseUploadWithCloudinary()
                {
                    UrlPicture = uploadResult.SecureUrl?.AbsoluteUri ?? string.Empty,
                    PublicId = uploadResult.PublicId
                };

                return resultData;
            }
            catch (Exception ex)
            {
                throw new ProjectAwsException("Cloud storage is unavailable.", ex);
            }
        }

        public async Task<DeletionResult> DeletePhotoToCloudAsync(string publicId)
        {
            EnsureCloudinaryConfigured();
            if (string.IsNullOrWhiteSpace(publicId) || publicId.Contains("..") || publicId.Contains("<script", StringComparison.OrdinalIgnoreCase))
            {
                throw new ProjectBadRequestException("Public id is invalid.");
            }

            try
            {
                var accountCloud = new Account(ProjectConfig.CloudName, ProjectConfig.APIKey, ProjectConfig.APISecret);
                _cloudinary = new Cloudinary(accountCloud);
                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);
                return result;
            }
            catch (Exception ex)
            {
                throw new ProjectAwsException("Cloud storage is unavailable.", ex);
            }
        }

        private static void EnsureCloudinaryConfigured()
        {
            if (string.IsNullOrWhiteSpace(ProjectConfig.CloudName) ||
                string.IsNullOrWhiteSpace(ProjectConfig.APIKey) ||
                string.IsNullOrWhiteSpace(ProjectConfig.APISecret))
            {
                throw new ProjectAwsException("Cloudinary configuration is missing.");
            }
        }
    }
}
