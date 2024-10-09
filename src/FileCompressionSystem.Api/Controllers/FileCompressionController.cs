using FileCompressionSystem.Application.Features.CompressFile;
using FileCompressionSystem.Application.Features.DecompressFile;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace FileCompressionSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileCompressionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FileCompressionController> _logger;

    public FileCompressionController(IMediator mediator, ILogger<FileCompressionController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("compress")]
    public async Task<IActionResult> CompressFile(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            var command = new CompressFileCommand { File = file };
            var result = await _mediator.Send(command);

            Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{result.FileName}\"");
            return new FileStreamResult(result.CompressedStream, "application/gzip");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while compressing the file");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    [HttpGet("decompress/{fileName}")]
    public async Task<IActionResult> DecompressFile(string fileName)
    {
        try
        {
            if (string.IsNullOrEmpty(fileName))
                return BadRequest("File name is empty");

            var query = new DecompressFileQuery { FileName = fileName };
            var result = await _mediator.Send(query);

            Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{result.FileName}\"");
            return new FileStreamResult(result.DecompressedStream, "application/octet-stream");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while decompressing the file");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }
}
