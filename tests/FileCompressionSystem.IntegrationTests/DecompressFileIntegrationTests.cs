using FileCompressionSystem.Application.Common.Interfaces;
using FileCompressionSystem.Application.Features.DecompressFile;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Compression;

namespace FileCompressionSystem.IntegrationTests;

public class DecompressFileIntegrationTests : IClassFixture<TestFixture>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly DecompressFileQueryHandler _handler;

    public DecompressFileIntegrationTests(TestFixture fixture)
    {
        _fileStorageService = fixture.ServiceProvider.GetRequiredService<IFileStorageService>();
        _handler = new DecompressFileQueryHandler(_fileStorageService);
    }

    [Fact]
    public async Task Handle_ShouldDecompressFileCorrectly()
    {
        // Arrange
        var fileName = "testfile.gz";
        var originalContent = "Hello, World!";
        var compressedContent = CompressString(originalContent);

        await _fileStorageService.SaveCompressedFileAsync(compressedContent, fileName);

        var request = new DecompressFileQuery { FileName = fileName };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal("testfile", result.FileName);
        Assert.Equal(originalContent, System.Text.Encoding.UTF8.GetString(result.Content));
        Assert.Equal("application/octet-stream", result.ContentType);
    }

    private byte[] CompressString(string str)
    {
        using var memoryStream = new MemoryStream();
        using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
        using (var writer = new StreamWriter(gzipStream))
        {
            writer.Write(str);
        }
        return memoryStream.ToArray();
    }
}