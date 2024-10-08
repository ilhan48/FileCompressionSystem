using FileCompressionSystem.Application.Common.Interfaces;
using FileCompressionSystem.Application.Features.DecompressFile;
using Moq;
using System.IO.Compression;

namespace FileCompressionSystem.UnitTests;

public class DecompressFileQueryHandlerTests
{
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly DecompressFileQueryHandler _handler;

    public DecompressFileQueryHandlerTests()
    {
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _handler = new DecompressFileQueryHandler(_fileStorageServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDecompressFileCorrectly()
    {
        // Arrange
        var fileName = "testfile.gz";
        var originalContent = "Hello, World!";
        var compressedContent = CompressString(originalContent);

        _fileStorageServiceMock.Setup(x => x.GetCompressedFileAsync(fileName))
            .ReturnsAsync(compressedContent);

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