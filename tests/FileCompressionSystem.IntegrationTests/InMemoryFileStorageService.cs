using FileCompressionSystem.Application.Common.Interfaces;
using System;
using System.Collections.Concurrent;

namespace FileCompressionSystem.IntegrationTests;

public class InMemoryFileStorageService : IFileStorageService
{
    private readonly ConcurrentDictionary<string, byte[]> _storage = new();

    public Task SaveCompressedFileAsync(byte[] fileContent, string fileName)
    {
        _storage[fileName] = fileContent;
        return Task.CompletedTask;
    }

    public Task<byte[]> GetCompressedFileAsync(string fileName)
    {
        _storage.TryGetValue(fileName, out var fileContent);
        return Task.FromResult(fileContent);
    }

    Task<string> IFileStorageService.SaveCompressedFileAsync(byte[] fileContent, string fileName)
    {
        _storage[fileName] = fileContent;
        return Task.FromResult(fileName);
    }

    public string GetOriginalFileName(string zipFileName)
    {
        throw new NotImplementedException();
    }
}
