using FileCompressionSystem.Application.Common.Interfaces;
using MediatR;
using System.IO.Compression;

namespace FileCompressionSystem.Application.Features.CompressFile;
public class CompressFileCommandHandler : IRequestHandler<CompressFileCommand, CompressFileResult>
{
    private readonly IFileStorageService _fileStorageService;

    public CompressFileCommandHandler(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    public async Task<CompressFileResult> Handle(CompressFileCommand request, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal, true))
        {
            await request.File.CopyToAsync(gzipStream);
        }

        var compressedContent = memoryStream.ToArray();
        var compressedFileName = $"{request.File.FileName}.gz"; 

        await _fileStorageService.SaveCompressedFileAsync(compressedContent, compressedFileName);

        return new CompressFileResult
        {
            FileName = compressedFileName,
            Content = compressedContent,
            ContentType = "application/gzip"
        };
    }
}
