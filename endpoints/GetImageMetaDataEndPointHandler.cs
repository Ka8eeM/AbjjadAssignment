using AbjjadAssignment.services.abstractions;
using Microsoft.AspNetCore.Mvc;

namespace AbjjadAssignment.endpoints;

public static class GetImageMetaDataEndPointHandler
{
    public static async Task<IResult> HandleAsync(
        [FromRoute] string uniqueImageId,
        IGetImageMetaData getImageMetaDataService,
        CancellationToken ctx
    )
    {
        try
        {
            var response = await getImageMetaDataService.GetImageMetadataAsync(uniqueImageId);
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
