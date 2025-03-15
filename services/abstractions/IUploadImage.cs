namespace AbjjadAssignment.services.abstractions;

public record ImageUploadResponse(string UniqueImageId, string Status);

public interface IUploadImage
{
    Task<List<ImageUploadResponse>> UploadImagesAsync(IFormFileCollection images);
}
