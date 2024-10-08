using FileCompressionSystem.Application.Common.Interfaces;
using System.IO.Compression;

namespace FileCompressionSystem.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "CompressedFiles");

    public FileStorageService()
    {
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task<string> SaveCompressedFileAsync(byte[] fileContent, string fileName)
    {
        var originalExtension = Path.GetExtension(fileName);
        if (originalExtension == ".zip")
        {
            throw new ArgumentException("The file to be compressed cannot have a .zip extension.");
        }

        var zipFileName = $"{Path.GetFileNameWithoutExtension(fileName)}.zip";
        var zipFilePath = Path.Combine(_storagePath, zipFileName);

        using (var archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
        {
            var zipEntry = archive.CreateEntry(fileName);
            using (var entryStream = zipEntry.Open())
            {
                await entryStream.WriteAsync(fileContent, 0, fileContent.Length);
            }
        }

        return zipFileName;
    }

    public async Task<byte[]> GetCompressedFileAsync(string fileName)
    {
        if (Path.GetExtension(fileName) != ".zip")
        {
            fileName = $"{Path.GetFileNameWithoutExtension(fileName)}.zip";
        }

        var zipFilePath = Path.Combine(_storagePath, fileName);

        if (!File.Exists(zipFilePath))
        {
            throw new FileNotFoundException("The specified zip file does not exist.", zipFilePath);
        }

        using (var archive = ZipFile.OpenRead(zipFilePath))
        {
            var zipEntry = archive.Entries.FirstOrDefault();
            if (zipEntry == null)
            {
                throw new InvalidDataException("The zip file is empty.");
            }

            using (var entryStream = zipEntry.Open())
            using (var resultStream = new MemoryStream())
            {
                await entryStream.CopyToAsync(resultStream);
                return resultStream.ToArray();
            }
        }
    }

    public string GetOriginalFileName(string zipFileName)
    {
        if (Path.GetExtension(zipFileName) != ".zip")
        {
            zipFileName = $"{Path.GetFileNameWithoutExtension(zipFileName)}.zip";
        }

        var zipFilePath = Path.Combine(_storagePath, zipFileName);

        if (!File.Exists(zipFilePath))
        {
            throw new FileNotFoundException("The specified zip file does not exist.", zipFilePath);
        }

        using (var archive = ZipFile.OpenRead(zipFilePath))
        {
            var zipEntry = archive.Entries.FirstOrDefault();
            if (zipEntry == null)
            {
                throw new InvalidDataException("The zip file is empty.");
            }

            return zipEntry.FullName;
        }
    }
}