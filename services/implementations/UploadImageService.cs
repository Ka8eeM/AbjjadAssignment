using System.Text.Json;
using AbjjadAssignment.services.abstractions;
using AbjjadAssignment.services.shared;

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
        var responses = new List<ImageUploadResponse>();

        if (images == null || !images.Any())
            return responses;

        // for parallel and faster execution

        var tasks = images.Select(image => ProcessSingleImage(image)).ToList();
        responses.AddRange(await Task.WhenAll(tasks));


        // foreach (var image in images)
        // {

        //     var response = await ProcessSingleImage(image);
        //     responses.Add(response);
        // }

        return responses;
    }
    private async Task<ImageUploadResponse> ProcessSingleImage(IFormFile image)
    {
        try
        {
            // Check file size
            if (image.Length > ImageConstants.MaxFileSize)
                return new ImageUploadResponse("", "error",
                    ServiceError.FileTooLarge(image.FileName, ImageConstants.MaxFileSize));

            // Check file extension
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!ImageConstants.AllowedExtensions.Contains(extension))
                return new ImageUploadResponse("", "error",
                    ServiceError.InvalidFormat(image.FileName));

            // Process image
            var uniqueId = Guid.NewGuid().ToString();
            var storagePath = Path.Combine(
                _environment.ContentRootPath,
                ImageConstants.ImagesFolderPath,
                uniqueId
            );
            Directory.CreateDirectory(storagePath);

            using var stream = image.OpenReadStream();
            var (webpData, metadata, processError) = await _imageProcessor.ProcessImageAsync(image);

            if (webpData == null || processError != null)
                return new ImageUploadResponse(uniqueId, "error",
                    processError ?? ServiceError.ProcessingFailed($"image {image.FileName}"));

            // Save original image
            var originalPath = Path.Combine(storagePath, "original.webp");
            await File.WriteAllBytesAsync(originalPath, webpData);


            //for parallel execution
            var resizeTasks = ImageConstants.SizePresets.Keys.Select(async size =>
                    {
                        var (data, error) = await _imageProcessor.ResizeImageAsync(webpData, size);
                        if (data != null) await File.WriteAllBytesAsync(Path.Combine(storagePath, $"{size}.webp"), data);
                        return error;
                    });
            var errors = (await Task.WhenAll(resizeTasks)).Where(e => e != null).ToList();
            if (errors.Any()) return new ImageUploadResponse(uniqueId, "error", errors.First());

            // // Process and save resized versions
            // foreach (var size in ImageConstants.SizePresets.Keys)
            // {
            //     var (resizedData, resizeError) = await _imageProcessor.ResizeImageAsync(webpData, size);
            //     if (resizedData == null || resizeError != null)
            //         return new ImageUploadResponse(uniqueId, "error",
            //             resizeError ?? ServiceError.ProcessingFailed($"image resize to {size}"));

            //     var resizedPath = Path.Combine(storagePath, $"{size}.webp");
            //     await File.WriteAllBytesAsync(resizedPath, resizedData);
            // }

            // Save metadata
            var metadataPath = Path.Combine(storagePath, "metadata.json");
            await File.WriteAllTextAsync(metadataPath, JsonSerializer.Serialize(metadata));

            return new ImageUploadResponse(uniqueId, "success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing image {FileName}", image.FileName);
            return new ImageUploadResponse("", "error",
                ServiceError.InternalError($"processing image {image.FileName}"));
        }
    }
}
