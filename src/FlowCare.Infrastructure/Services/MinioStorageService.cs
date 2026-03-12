using FlowCare.Application.Interfaces.Persistence;
using Minio;
using Minio.DataModel.Args;

namespace FlowCare.Infrastructure.Services;

public class MinioStorageService : IStorageService
{
    private readonly IMinioClient _minioClient;

    public MinioStorageService(IMinioClient minioClient)
    {
        _minioClient = minioClient;
    }

    public async Task<string> UploadFileAsync(string bucketName, string objectName, Stream content, string contentType)
    {
        // 1. Ensure the bucket exists
        var beArgs = new BucketExistsArgs().WithBucket(bucketName);
        bool found = await _minioClient.BucketExistsAsync(beArgs);
        if (!found)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
        }

        // 2. Upload the file
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithStreamData(content)
            .WithObjectSize(content.Length)
            .WithContentType(contentType);

        await _minioClient.PutObjectAsync(putObjectArgs);

        // Return the format: "bucket/filename"
        return $"{bucketName}/{objectName}";
    }

    public async Task<string> GetPresignedUrlAsync(string fullPath, int expiryMinutes = 60)
    {
        if (string.IsNullOrEmpty(fullPath)) return string.Empty;

        // Split "customer-ids/file.jpg" into bucket and object
        var parts = fullPath.Split('/');
        if (parts.Length < 2) throw new ArgumentException("Invalid file path format.");

        var args = new PresignedGetObjectArgs()
            .WithBucket(parts[0])
            .WithObject(parts[1])
            .WithExpiry(expiryMinutes * 60); // MinIO expects seconds

        return await _minioClient.PresignedGetObjectAsync(args);
    }

    public async Task DeleteFileAsync(string fullPath)
    {
        var parts = fullPath.Split('/');
        if (parts.Length < 2) return;

        var args = new RemoveObjectArgs()
            .WithBucket(parts[0])
            .WithObject(parts[1]);

        await _minioClient.RemoveObjectAsync(args);
    }
}