using AbjjadAssignment.services.abstractions;
using Microsoft.AspNetCore.Mvc;

namespace AbjjadAssignment.endpoints;

public static class DownloadImageEndPointHandler
{
    public static IResult HandleAsync(
        [FromRoute] string uniqueImageId,
        [FromQuery] string size,
        IDownloadImage downloadImageService,
        CancellationToken ctx
    )
    {
        try
        {
            var (result, error) = downloadImageService.DownloadImageAsync(uniqueImageId, size);

            if (result is null || error is not null)
            {
                return Results.BadRequest(new { Error = new { error!.Code, error.Message } });
            }

            return Results.File(result.FileStream, "image/webp", $"{uniqueImageId}.webp");
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
