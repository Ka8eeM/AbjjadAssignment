using AbjjadAssignment.services.abstractions;
using AbjjadAssignment.services.implementations;

namespace AbjjadAssignment.services;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IUploadImage, UploadImageService>();
        services.AddTransient<IDownloadImage, DownloadImageService>();
        services.AddTransient<IGetImageMetaData, GetImageMetaDataService>();
        services.AddTransient<IImageProcessor, ImageProcessorService>();
        return services;
    }
}
