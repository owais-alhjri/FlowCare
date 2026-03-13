namespace FlowCare.Application.Interfaces;

public interface IStorageService
{
    Task<string> UploadFileAsync(
        string bucketName,
        string objectName,
        Stream content,
        string contentType);

    Task<(Stream Content, string ContentType)> GetFileAsync(string fullPath);
}