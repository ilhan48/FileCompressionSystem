using MediatR;
using Microsoft.AspNetCore.Http;

namespace FileCompressionSystem.Application.Features.CompressFile;

public class CompressFileCommand : IRequest<CompressFileResult>
{
    public IFormFile File { get; set; }
}

public class CompressFileResult
{
    public string FileName { get; set; }
    public Stream CompressedStream { get; set; }
}