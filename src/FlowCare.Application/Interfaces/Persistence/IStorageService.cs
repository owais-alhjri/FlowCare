namespace FlowCare.Application.Interfaces.Persistence;

public interface IStorageService
{

    Task<string> UploadFileAsync(
        string bucketName,
        string objectName,
        Stream content,
        string contentType);


    Task<string> GetPresignedUrlAsync(string fullPath, int expiryMinutes = 60);

    Task DeleteFileAsync(string fullPath);
}