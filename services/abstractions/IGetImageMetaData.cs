namespace AbjjadAssignment.services.abstractions;

public record ImageMetadataResponse(
    string UniqueImageId, 
    Dictionary<string, string> Metadata);


public interface IGetImageMetaData
{
    Task<ImageMetadataResponse> GetImageMetadataAsync(string uniqueImageId);
}
