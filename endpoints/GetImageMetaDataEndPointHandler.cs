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

            if (response.error is not null)
            {
                return Results.BadRequest(new { Error = new { response.error.Code, response.error.Message } });
            }

            return Results.Ok(response.response);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
