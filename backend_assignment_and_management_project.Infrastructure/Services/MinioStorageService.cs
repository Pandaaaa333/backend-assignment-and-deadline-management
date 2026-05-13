using backend_assignment_and_management_project.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;

namespace backend_assignment_and_management_project.Infrastructure.Services
{
    public class MinioStorageService : IStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly string _bucketName;
        private readonly string _endpoint;

        public MinioStorageService(IConfiguration configuration)
        {
            var minioSettings = configuration.GetSection("Minio");
            _endpoint = minioSettings["Endpoint"] ?? "localhost:9000";
            var accessKey = minioSettings["AccessKey"] ?? "minioadmin";
            var secretKey = minioSettings["SecretKey"] ?? "minioadminpassword";
            _bucketName = minioSettings["BucketName"] ?? "study-management";
            var useSSL = bool.Parse(minioSettings["UseSSL"] ?? "false");

            _minioClient = new MinioClient()
                .WithEndpoint(_endpoint)
                .WithCredentials(accessKey, secretKey)
                .WithSSL(useSSL)
                .Build();
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folderName = "uploads")
        {
            try
            {
                // Ensure bucket exists
                var beArgs = new BucketExistsArgs().WithBucket(_bucketName);
                bool found = await _minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs().WithBucket(_bucketName);
                    await _minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
                }

                var objectName = $"{folderName}/{Guid.NewGuid()}_{fileName}";
                
                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectName)
                    .WithStreamData(fileStream)
                    .WithObjectSize(fileStream.Length)
                    .WithContentType(contentType);

                await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

                return objectName;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading file to MinIO: {ex.Message}", ex);
            }
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            try
            {
                // Assuming fileUrl is just the object name (e.g., uploads/guid_name.png)
                var removeObjectArgs = new RemoveObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(fileUrl);

                await _minioClient.RemoveObjectAsync(removeObjectArgs).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting file from MinIO: {ex.Message}", ex);
            }
        }

        public async Task<string> GetPresignedUrlAsync(string fileName, int expiryInSeconds = 3600)
        {
            try
            {
                var args = new PresignedGetObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(fileName)
                    .WithExpiry(expiryInSeconds);

                return await _minioClient.PresignedGetObjectAsync(args).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating presigned URL from MinIO: {ex.Message}", ex);
            }
        }
    }
}
