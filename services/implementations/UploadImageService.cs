using System.Text.Json;
using AbjjadAssignment.services.abstractions;

namespace AbjjadAssignment.services.implementations;

internal sealed class UploadImageService : IUploadImage
{
    private readonly IImageProcessor _imageProcessor;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<UploadImageService> _logger;

    public UploadImageService(
        IImageProcessor imageProcessor,
        IWebHostEnvironment environment,
        ILogger<UploadImageService> logger
    )
    {
        _imageProcessor = imageProcessor;
        _environment = environment;
        _logger = logger;
    }

    public async Task<List<ImageUploadResponse>> UploadImagesAsync(IFormFileCollection images)
    {
        try
        {
            var responses = new List<ImageUploadResponse>();

            if (images == null || !images.Any())
                return responses;

            foreach (var image in images)
            {
                if (image.Length > ImageConstants.MaxFileSize)
                    throw new ArgumentException($"Image {image.FileName} exceeds 2MB limit");

                var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                if (!ImageConstants.AllowedExtensions.Contains(extension))
                    throw new ArgumentException(
                        $"Image {image.FileName} is an invalid image format"
                    );

                var uniqueId = Guid.NewGuid().ToString();
                var storagePath = Path.Combine(
                    _environment.ContentRootPath,
                    ImageConstants.ImagesFolderPath,
                    uniqueId
                );
                Directory.CreateDirectory(storagePath);

                using var stream = image.OpenReadStream();
                var (webpData, metadata) = await _imageProcessor.ProcessImageAsync(image);

                if (webpData == null)
                    throw new InvalidOperationException(
                        $"Failed to process image {image.FileName}"
                    );

                var originalPath = Path.Combine(storagePath, "original.webp");
                await File.WriteAllBytesAsync(originalPath, webpData);

                foreach (var size in ImageConstants.SizePresets.Keys)
                {
                    var resizedData = await _imageProcessor.ResizeImageAsync(webpData, size);
                    var resizedPath = Path.Combine(storagePath, $"{size}.webp");
                    await File.WriteAllBytesAsync(resizedPath, resizedData);
                }

                var metadataPath = Path.Combine(storagePath, "metadata.json");
                await File.WriteAllTextAsync(metadataPath, JsonSerializer.Serialize(metadata));

                responses.Add(new ImageUploadResponse(uniqueId, "success"));
            }

            return responses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing image upload");
            throw;
        }
    }
}
