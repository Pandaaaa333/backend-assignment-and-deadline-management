namespace backend_assignment_and_management_project.Application.Interfaces
{
    public interface IStorageService
    {
        /// <summary>
        /// Uploads a file to the storage and returns the public URL or file path.
        /// </summary>
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folderName = "uploads");

        /// <summary>
        /// Deletes a file from the storage.
        /// </summary>
        Task DeleteFileAsync(string fileUrl);

        /// <summary>
        /// Gets a presigned URL for a file (useful for private buckets).
        /// </summary>
        Task<string> GetPresignedUrlAsync(string fileName, int expiryInSeconds = 3600);
    }
}
