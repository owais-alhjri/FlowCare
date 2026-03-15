using FlowCare.Application.Interfaces;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;


namespace FlowCare.Infrastructure.Services;

public class MinioStorageService(IMinioClient minioClient) : IStorageService
{
    public async Task<string> UploadFileAsync(string bucketName, string objectName, Stream content, string contentType)
    {
        // 1. Check if the bucket exists first
        var bktExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
        bool found = await minioClient.BucketExistsAsync(bktExistsArgs);

        // 2. If it doesn't exist, create it
        if (!found)
        {
            var makeBktArgs = new MakeBucketArgs().WithBucket(bucketName);
            await minioClient.MakeBucketAsync(makeBktArgs);
        }

        // 3. Upload the file
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithStreamData(content)
            .WithObjectSize(content.Length)
            .WithContentType(contentType);

        await minioClient.PutObjectAsync(putObjectArgs);

        return $"{bucketName}/{objectName}";
    }

    public async Task<(Stream Content, string ContentType)> GetFileAsync(string fullPath)
    {
        var parts = fullPath.Split('/');
        if (parts.Length < 2) throw new ArgumentException("Invalid file path format.");

        var memoryStream = new MemoryStream();

        var args = new GetObjectArgs()
            .WithBucket(parts[0])
            .WithObject(parts[1])
            .WithCallbackStream(stream => stream.CopyTo(memoryStream));

        var stat = await minioClient.GetObjectAsync(args);
        memoryStream.Position = 0;

        return (memoryStream, stat.ContentType);
    }
}