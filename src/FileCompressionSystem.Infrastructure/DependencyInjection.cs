using FileCompressionSystem.Application.Common.Interfaces;
using FileCompressionSystem.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FileCompressionSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IFileStorageService, FileStorageService>();
        return services;
    }
}
