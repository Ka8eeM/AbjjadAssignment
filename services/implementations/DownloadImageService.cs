using AbjjadAssignment.services.abstractions;
using AbjjadAssignment.services.shared;
using Microsoft.AspNetCore.Mvc;

namespace AbjjadAssignment.services.implementations;

internal sealed class DownloadImageService : IDownloadImage
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<DownloadImageService> _logger;

    public DownloadImageService(
        IWebHostEnvironment environment,
        ILogger<DownloadImageService> logger
    )
    {
        _environment = environment;
        _logger = logger;
    }

    public (FileStreamResult? result, ServiceError? error) DownloadImageAsync(string uniqueImageId, string size)
    {
        try
        {
            if (string.IsNullOrEmpty(uniqueImageId))
                return (null, ServiceError.InvalidFormat("image ID - null or empty"));

            if (!ImageConstants.SizePresets.ContainsKey(size))
                return (null, ServiceError.InvalidFormat($"size parameter: {size}"));

            var filePath = Path.Combine(
                _environment.ContentRootPath,
                ImageConstants.ImagesFolderPath,
                uniqueImageId,
                $"{size}.webp"
            );

            if (!File.Exists(filePath))
                return (null, ServiceError.ProcessingFailed($"image download for ID {uniqueImageId} size {size} - file not found"));

            // much better: return the filePath is best practice the file could be saved on
            // content management service like S3 buckets

            // but will return the file stream as required
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var fileStreamResult = new FileStreamResult(stream, "image/webp");
            return (fileStreamResult, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading image ID {ImageId} size {Size}", uniqueImageId, size);
            return (null, ServiceError.InternalError($"downloading image ID {uniqueImageId} size {size}"));
        }
    }
}