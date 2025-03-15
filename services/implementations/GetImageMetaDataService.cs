using System.Text.Json;
using AbjjadAssignment.services.abstractions;
using AbjjadAssignment.services.shared;

namespace AbjjadAssignment.services.implementations;

internal sealed class GetImageMetaDataService : IGetImageMetaData
{
    private readonly IImageProcessor _imageProcessor;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<GetImageMetaDataService> _logger;

    public GetImageMetaDataService(
        IImageProcessor imageProcessor,
        IWebHostEnvironment environment,
        ILogger<GetImageMetaDataService> logger
    )
    {
        _imageProcessor = imageProcessor;
        _environment = environment;
        _logger = logger;
    }

    public async Task<(ImageMetadataResponse? response, ServiceError? error)> GetImageMetadataAsync(string uniqueImageId)
    {
        try
        {
            if (string.IsNullOrEmpty(uniqueImageId))
                return (null, ServiceError.InvalidFormat("image ID - null or empty"));

            var metadataPath = Path.Combine(
                _environment.ContentRootPath,
                ImageConstants.ImagesFolderPath,
                uniqueImageId,
                "metadata.json"
            );

            if (!File.Exists(metadataPath))
                return (null, ServiceError.ProcessingFailed($"metadata retrieval for image ID {uniqueImageId} - file not found"));

            var json = await File.ReadAllTextAsync(metadataPath);
            var metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            if (metadata == null)
                return (null, ServiceError.ProcessingFailed($"metadata deserialization for image ID {uniqueImageId}"));

            return (new ImageMetadataResponse(uniqueImageId, metadata), null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving metadata for image ID {ImageId}", uniqueImageId);
            return (null, ServiceError.InternalError($"retrieving metadata for image ID {uniqueImageId}"));
        }
    }
}