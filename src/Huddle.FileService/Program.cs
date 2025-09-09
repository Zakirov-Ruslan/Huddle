using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;

namespace Huddle.FileService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var accessKey = "minioadmin";
        var secretKey = "minioadmin";

        builder.Services.AddMinio(accessKey, secretKey);

        var app = builder.Build();

        app.MapPost("/upload", async (IFormFile file, [FromServices] IMinioClient minioClient) =>
        {
            var objectName = Guid.NewGuid().ToString();
            var putObjectArgs = new PutObjectArgs()
                .WithBucket("my-bucket")
                .WithObject(objectName)
                .WithStreamData(file.OpenReadStream())
                .WithObjectSize(file.Length)
                .WithContentType(file.ContentType);

            await minioClient.PutObjectAsync(putObjectArgs);

            // Публикация события
            // await eventBus.PublishAsync(new FileUploadedEvent(objectName, file.FileName));

            return Results.Ok(new { ObjectName = objectName });
        });

        app.Run();
    }
}
