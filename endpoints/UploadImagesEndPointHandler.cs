using AbjjadAssignment.services.abstractions;
using Microsoft.AspNetCore.Mvc;

namespace AbjjadAssignment.endpoints;

public static class UploadImagesEndPointHandler
{
    public static async Task<IResult> HandleAsync(
        [FromForm] IFormFileCollection images,
        IUploadImage imageService
    )
    {
        try
        {
            var response = await imageService.UploadImagesAsync(images);
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
