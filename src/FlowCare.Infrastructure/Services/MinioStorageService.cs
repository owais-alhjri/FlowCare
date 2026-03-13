using FlowCare.Application.Interfaces;
using Minio;
using Minio.DataModel.Args;

namespace FlowCare.Infrastructure.Services;

public class MinioStorageService(IMinioClient minioClient) : IStorageService
{


    public async Task<string> UploadFileAsync(string bucketName, string objectName, Stream content, string contentType)
    {
        // to check if the bucket exists
        var beArgs = new BucketExistsArgs().WithBucket(bucketName);
        bool found = await minioClient.BucketExistsAsync(beArgs);
        if (!found)
        {
            await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
        }

        // Upload the file
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithStreamData(content)
            .WithObjectSize(content.Length)
            .WithContentType(contentType);

        await minioClient.PutObjectAsync(putObjectArgs);

        // Return the format: "bucket/filename"
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