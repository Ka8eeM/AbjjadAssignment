namespace AbjjadAssignment.services.abstractions;
using AbjjadAssignment.services.shared;

public class ImageUploadResponse
{
    public string ImageId { get; }
    public string Status { get; }
    public ServiceError? Error { get; }

    public ImageUploadResponse(string imageId, string status, ServiceError? error = null)
    {
        ImageId = imageId;
        Status = status;
        Error = error;
    }

    public bool IsSuccess => Status == "success" && Error == null;
}
public interface IUploadImage
{
    Task<List<ImageUploadResponse>> UploadImagesAsync(IFormFileCollection images);
}
