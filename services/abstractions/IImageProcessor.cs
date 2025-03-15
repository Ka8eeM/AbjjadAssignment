using AbjjadAssignment.services.shared;

namespace AbjjadAssignment.services.abstractions;

public class ImageMetadata
{
    public string GeoLocation { get; set; } = string.Empty;
    public string CameraMake { get; set; } = string.Empty;
    public string CameraModel { get; set; } = string.Empty;
}

public interface IImageProcessor
{
    Task<(byte[]? webpData, ImageMetadata? metadata, ServiceError? error)> ProcessImageAsync(IFormFile image);
    Task<(byte[]? resizedData, ServiceError? error)> ResizeImageAsync(byte[] imageData, string size);
}
