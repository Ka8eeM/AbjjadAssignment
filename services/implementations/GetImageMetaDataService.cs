using System.Text.Json;
using AbjjadAssignment.services.abstractions;

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

    public async Task<ImageMetadataResponse> GetImageMetadataAsync(string uniqueImageId)
    {
        try
        {
            if (string.IsNullOrEmpty(uniqueImageId))
                throw new ArgumentException("Invalid image ID");

            var metadataPath = Path.Combine(
                _environment.ContentRootPath,
                ImageConstants.ImagesFolderPath,
                uniqueImageId,
                "metadata.json"
            );

            if (!File.Exists(metadataPath))
                throw new FileNotFoundException("Metadata not found");

            var json = await File.ReadAllTextAsync(metadataPath);
            var metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            return new ImageMetadataResponse(uniqueImageId, metadata!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving metadata");
            throw;
        }
    }
}
