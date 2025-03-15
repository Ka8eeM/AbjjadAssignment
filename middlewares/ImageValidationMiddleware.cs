using AbjjadAssignment.services;
using AbjjadAssignment.services.shared;


namespace AbjjadAssignment.middlewares;

// FileValidationMiddleware.cs
public class ImageValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ImageValidationMiddleware> _logger;

    public ImageValidationMiddleware(
        RequestDelegate next,
        ILogger<ImageValidationMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only validate POST requests to the upload endpoint
        if (
            context.Request.Method != HttpMethods.Post
            || !context.Request.Path.StartsWithSegments("/api/images/upload")
        )
        {
            await _next(context);
            return;
        }

        try
        {
            // Check if the request contains multipart/form-data
            if (!context.Request.HasFormContentType)
            {
                _logger.LogWarning("Invalid content type for file upload");
                await WriteResponse(
                    context,
                    StatusCodes.Status415UnsupportedMediaType,
                    "Only multipart/form-data is supported"
                );
                return;
            }

            var form = await context.Request.ReadFormAsync();
            var files = form.Files;

            if (files == null || !files.Any())
            {
                _logger.LogWarning("No files provided in upload request");
                await WriteResponse(context, StatusCodes.Status400BadRequest, "No files provided");
                return;
            }

            foreach (var file in files)
            {
                // Validate file size
                if (file.Length > ImageConstants.MaxFileSize)
                {
                    _logger.LogWarning(
                        "File {FileName} exceeds maximum size of 2MB",
                        file.FileName
                    );
                    await WriteResponse(
                        context,
                        StatusCodes.Status413PayloadTooLarge,
                        $"File {file.FileName} exceeds 2MB limit"
                    );
                    return;
                }

                // Validate extension
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!ImageConstants.AllowedExtensions.Contains(extension))
                {
                    _logger.LogWarning(
                        "Invalid file extension {Extension} for file {FileName}",
                        extension,
                        file.FileName
                    );
                    await WriteResponse(
                        context,
                        StatusCodes.Status400BadRequest,
                        $"Invalid file format {extension}. Allowed: jpg, png, webp"
                    );
                    return;
                }
            }

            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in file validation middleware");
            await WriteResponse(
                context,
                StatusCodes.Status500InternalServerError,
                "Internal server error during file validation"
            );
        }
    }

    private static async Task WriteResponse(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        var errorResponse = new { error = message };
        await context.Response.WriteAsJsonAsync(errorResponse);
    }
}

public static class ImageValidationMiddlewareExtensions
{
    public static IApplicationBuilder UseImageValidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ImageValidationMiddleware>();
    }
}
