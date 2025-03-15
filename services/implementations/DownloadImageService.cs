using AbjjadAssignment.services.abstractions;
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

    public async Task<string> DownloadImageAsync(string uniqueImageId, string size)
    {
        await Task.CompletedTask;
        try
        {
            if (
                string.IsNullOrEmpty(uniqueImageId) || !ImageConstants.SizePresets.ContainsKey(size)
            )
                throw new ArgumentException("Invalid parameters");

            var filePath = Path.Combine(
                _environment.ContentRootPath,
                ImageConstants.ImagesFolderPath,
                uniqueImageId,
                $"{size}.webp"
            );

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Image not found");

            // return the filePath is best practice the file could be saved on
            // content management service like S3 buckets
            // so return the full path of the file then download it.

            // we can return the file stream itself if the business require this.
            return filePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading image");
            throw;
        }
    }
}
