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
            var responses = await imageService.UploadImagesAsync(images);

            if (responses.Any(r => !r.IsSuccess))
            {
                return Results.BadRequest(new
                {
                    Errors = responses.Where(r => !r.IsSuccess)
                                  .Select(r => new { r.ImageId, r.Error })
                });
            }
            return Results.Ok(responses);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
