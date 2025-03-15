using Microsoft.AspNetCore.Mvc;

namespace AbjjadAssignment.services.abstractions;

public interface IDownloadImage
{
    Task<FileStreamResult> DownloadImageAsync(string uniqueImageId, string size);
}
