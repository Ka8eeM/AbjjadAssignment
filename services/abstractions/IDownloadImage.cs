using AbjjadAssignment.services.shared;
using Microsoft.AspNetCore.Mvc;


namespace AbjjadAssignment.services.abstractions;

public interface IDownloadImage
{
    (FileStreamResult? result, ServiceError? error) DownloadImageAsync(string uniqueImageId, string size);
}
