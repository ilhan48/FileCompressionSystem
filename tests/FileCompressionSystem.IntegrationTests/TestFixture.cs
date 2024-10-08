using FileCompressionSystem.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FileCompressionSystem.IntegrationTests;

public class TestFixture : IDisposable
{
    public ServiceProvider ServiceProvider { get; private set; }

    public TestFixture()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<IFileStorageService, InMemoryFileStorageService>();
        

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    public void Dispose()
    {
        ServiceProvider?.Dispose();
    }
}
