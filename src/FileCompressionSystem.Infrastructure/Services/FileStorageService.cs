using FileCompressionSystem.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FileCompressionSystem.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _storagePath;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(IConfiguration configuration, ILogger<FileStorageService> logger)
    {
        _storagePath = configuration["StoragePath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "CompressedFiles");
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task<Stream> GetCompressedFileStreamAsync(string fileName)
    {
        var filePath = Path.Combine(_storagePath, fileName);
        if (!File.Exists(filePath))
        {
            _logger.LogWarning("File not found: {FilePath}", filePath);
            throw new FileNotFoundException("The specified file does not exist.", filePath);
        }

        return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous);
    }

    public async Task SaveCompressedFileAsync(Stream compressedStream, string fileName)
    {
        var filePath = Path.Combine(_storagePath, fileName);
        try
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous))
            {
                await compressedStream.CopyToAsync(fileStream);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while saving the compressed file: {FilePath}", filePath);
            throw;
        }
    }
}