using System.Threading.Tasks;
using Api.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PropertyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{PropertyId}")]
        public async Task<IActionResult> Get([FromRoute]PropertyDetails.Query request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpPost]
        public async Task<IActionResult> Get([FromBody]PropertyList.Query request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpPost("ForMap")]
        public async Task<IActionResult> GetForMap([FromBody]PropertyMap.Query request)
        {
            return Ok(await _mediator.Send(request));
        }

    }
}
