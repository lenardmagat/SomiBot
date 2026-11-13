using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SomiBot.Routers;
[ApiController]
[Route("API/[Controller]")]
public partial class BibotBrainController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BibotBrainController> _logger;
    public BibotBrainController(IMediator mediator, ILogger<BibotBrainController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
}