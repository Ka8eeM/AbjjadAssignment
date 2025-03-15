using Microsoft.AspNetCore.Mvc;

namespace AbjjadAssignment.services.abstractions;

public interface IDownloadImage
{
    Task<string> DownloadImageAsync(string uniqueImageId, string size);
}
