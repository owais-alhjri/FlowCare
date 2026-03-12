using Microsoft.AspNetCore.Http;

namespace FlowCare.Application.Interfaces.Services;

public class FileValidationService
{
    private static readonly string[] ImageTypes = { "image/jpeg", "image/png", "image/webp" };
    private static readonly string[] AttachmentTypes = { "image/jpeg", "image/png", "image/webp", "application/pdf" };

    private const long MaxIdImageSize = 3 * 1024 * 1024;       // 3 MB
    private const long MaxAttachmentSize = 5 * 1024 * 1024;    // 5 MB

    public (bool IsValid, string? Error) ValidateIdImage(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return (false, "ID image is required.");

        if (file.Length > MaxIdImageSize)
            return (false, "ID image must not exceed 3 MB.");

        if (!ImageTypes.Contains(file.ContentType.ToLower()))
            return (false, "ID image must be a JPEG, PNG, or WebP.");

        return (true, null);
    }

    public (bool IsValid, string? Error) ValidateAttachment(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return (false, "Attachment is required.");

        if (file.Length > MaxAttachmentSize)
            return (false, "Attachment must not exceed 5 MB.");

        if (!AttachmentTypes.Contains(file.ContentType.ToLower()))
            return (false, "Attachment must be a JPEG, PNG, WebP, or PDF.");

        return (true, null);
    }
}