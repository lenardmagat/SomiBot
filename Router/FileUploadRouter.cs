using Microsoft.AspNetCore.Mvc;
using SomiBot.DTOs;
using SomiBot.ErrorHandling.Extensionl;
using SomiBot.Services;

namespace SomiBot.Routers;
public partial class BibotBrainController
{
    [HttpPost("Upload")]
    public async Task<IActionResult> UploadDocx([FromForm]
        UploadDTO uploadDTO,
        CancellationToken cancellation)
    {
        var ext = Path.GetExtension(uploadDTO.File.FileName).ToLowerInvariant();
        if(ext != ".docx")
            return BadRequest("Only .docx file are allowed");
        UploadFileCommand command = new UploadFileCommand(uploadDTO);
        var result = await _mediator.Send(command, cancellation);
        return result.ToActionResult();
    }
}