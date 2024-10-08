using FileCompressionSystem.Application.Common.Interfaces;
using MediatR;
using System.IO.Compression;

namespace FileCompressionSystem.Application.Features.DecompressFile;

public class DecompressFileQueryHandler : IRequestHandler<DecompressFileQuery, DecompressFileResult>
{
    private readonly IFileStorageService _fileStorageService;

    public DecompressFileQueryHandler(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    public async Task<DecompressFileResult> Handle(DecompressFileQuery request, CancellationToken cancellationToken)
    {
        var compressedContent = await _fileStorageService.GetCompressedFileAsync(request.FileName);

        using var compressedStream = new MemoryStream(compressedContent);
        using var decompressedStream = new MemoryStream();
        using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
        {
            await gzipStream.CopyToAsync(decompressedStream);
        }

        var decompressedContent = decompressedStream.ToArray();
        var originalFileName = Path.GetFileNameWithoutExtension(request.FileName);

        return new DecompressFileResult
        {
            FileName = originalFileName,
            Content = decompressedContent,
            ContentType = "application/octet-stream"
        };
    }
}

