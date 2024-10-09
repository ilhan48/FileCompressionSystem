using FileCompressionSystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.IO.Compression;

namespace FileCompressionSystem.Application.Features.CompressFile;
public class CompressFileCommandHandler : IRequestHandler<CompressFileCommand, CompressFileResult>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<CompressFileCommandHandler> _logger;

    public CompressFileCommandHandler(IFileStorageService fileStorageService, ILogger<CompressFileCommandHandler> logger)
    {
        _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CompressFileResult> Handle(CompressFileCommand request, CancellationToken cancellationToken)
    {
        if (request.File == null)
            throw new ArgumentNullException(nameof(request.File));

        var compressedFileName = $"{request.File.FileName}.gz";
        var compressedStream = new MemoryStream();

        try
        {
            using (var sourceStream = request.File.OpenReadStream())
            using (var compressionStream = new GZipStream(compressedStream, CompressionMode.Compress, true))
            {
                await sourceStream.CopyToAsync(compressionStream, 81920, cancellationToken);
            }

            compressedStream.Position = 0;

            // Save the compressed file asynchronously
            await _fileStorageService.SaveCompressedFileAsync(compressedStream, compressedFileName);

            return new CompressFileResult
            {
                FileName = compressedFileName,
                CompressedStream = compressedStream
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while compressing the file");
            compressedStream.Dispose();
            throw;
        }
    }
}
