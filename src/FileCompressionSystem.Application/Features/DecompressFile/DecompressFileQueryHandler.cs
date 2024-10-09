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
        var compressedStream = await _fileStorageService.GetCompressedFileStreamAsync(request.FileName);
        var decompressedStream = new MemoryStream();

        using (var decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress))
        {
            await decompressionStream.CopyToAsync(decompressedStream, 81920, cancellationToken);
        }

        decompressedStream.Position = 0;
        var originalFileName = Path.GetFileNameWithoutExtension(request.FileName);

        return new DecompressFileResult
        {
            FileName = originalFileName,
            DecompressedStream = decompressedStream
        };
    }
}


