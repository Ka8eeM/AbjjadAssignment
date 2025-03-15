namespace AbjjadAssignment.endpoints;

public static class MapEndPointsAPI
{
    public static void MapEndPoints(this WebApplication app)
    {
        app.MapPost("/api/images/upload", UploadImagesEndPointHandler.HandleAsync)
            .Accepts<IFormFileCollection>("multipart/form-data")
            .Produces<services.abstractions.ImageUploadResponse>(200)
            .Produces<string>(400)
            .DisableAntiforgery();

        app.MapGet("/api/images/download/{uniqueImageId}", DownloadImageEndPointHandler.HandleAsync)
            .Produces(200, contentType: "image/webp");

        app.MapGet(
                "/api/images/metadata/{uniqueImageId}",
                GetImageMetaDataEndPointHandler.HandleAsync
            )
            .Produces<services.abstractions.ImageMetadataResponse>(200);

        app.MapGet("/health", () => Results.Ok("Healthy")).Produces<string>(200);
    }
}
