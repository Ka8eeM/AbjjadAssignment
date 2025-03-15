using AbjjadAssignment.services.shared;


namespace AbjjadAssignment.services.abstractions;


public record ImageMetadataResponse(
    string UniqueImageId,
    Dictionary<string, string> Metadata);


public interface IGetImageMetaData
{
    Task<(ImageMetadataResponse? response, ServiceError? error)> GetImageMetadataAsync(string uniqueImageId);
}
