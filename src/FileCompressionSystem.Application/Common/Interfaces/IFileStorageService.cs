namespace FileCompressionSystem.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<Stream> GetCompressedFileStreamAsync(string fileName);
    Task SaveCompressedFileAsync(Stream compressedStream, string fileName);
}

