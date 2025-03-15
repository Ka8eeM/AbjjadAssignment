using AbjjadAssignment.services.abstractions;
using Microsoft.AspNetCore.Mvc;

namespace AbjjadAssignment.endpoints;

public static class DownloadImageEndPointHandler
{
    public static async Task<IResult> HandleAsync(
        [FromRoute] string uniqueImageId,
        [FromQuery] string size,
        IDownloadImage downloadImageService,
        CancellationToken ctx
    )
    {
        try
        {
            var response = await downloadImageService.DownloadImageAsync(uniqueImageId, size);
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
