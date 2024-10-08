using FileCompressionSystem.Application.Features.CompressFile;
using FileCompressionSystem.Application.Features.DecompressFile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FileCompressionSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileCompressionController : ControllerBase
{
    private readonly IMediator _mediator;

    public FileCompressionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("compress")]
    public async Task<IActionResult> CompressFile(IFormFile file)
    {
        var command = new CompressFileCommand { File = file };
        var result = await _mediator.Send(command);

        return File(result.Content, result.ContentType, result.FileName);
    }

    [HttpGet("decompress/{fileName}")]
    public async Task<IActionResult> DecompressFile(string fileName)
    {
        var query = new DecompressFileQuery { FileName = fileName };
        var result = await _mediator.Send(query);

        return File(result.Content, result.ContentType, result.FileName);
    }
}
