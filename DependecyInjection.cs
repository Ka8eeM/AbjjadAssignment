using AbjjadAssignment.services;

namespace AbjjadAssignment;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHealthChecks();

        var StoragePath = "./" + ImageConstants.ImagesFolderPath;

        if (!Directory.Exists(StoragePath))
            Directory.CreateDirectory(StoragePath);
        return services;
    }
}
