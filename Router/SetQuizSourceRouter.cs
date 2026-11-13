using Microsoft.AspNetCore.Mvc;
using SomiBot.ErrorHandling.Extensionl;
using SomiBot.Services;

namespace SomiBot.Routers;
public partial class BibotBrainController
{
    [HttpPost("SetQuizSource/{id}")]
    public async Task<IActionResult> SetQuizSource(int id, CancellationToken cancellationToken)
    {
        SetQuizSourceCommand command = new SetQuizSourceCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}