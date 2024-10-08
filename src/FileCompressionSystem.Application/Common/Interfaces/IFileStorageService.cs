namespace FileCompressionSystem.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveCompressedFileAsync(byte[] fileContent, string fileName);
    Task<byte[]> GetCompressedFileAsync(string fileName);
    string GetOriginalFileName(string zipFileName);
}
