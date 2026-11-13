using Microsoft.AspNetCore.Mvc;
using SomiBot.ErrorHandling.Extensionl;
using SomiBot.Services;

namespace SomiBot.Routers;
public partial class BibotBrainController
{
    [HttpGet("Files")]
    public async Task<IActionResult> Files(CancellationToken cancellationToken)
    {
        GetFileDetailsCommand command = new GetFileDetailsCommand();
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}