using MediatR;

namespace FileCompressionSystem.Application.Features.DecompressFile;

public class DecompressFileQuery : IRequest<DecompressFileResult>
{
    public string FileName { get; set; }
}

public class DecompressFileResult
{
    public string FileName { get; set; }
    public Stream DecompressedStream { get; set; }
}
