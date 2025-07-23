using Huddle.Channel.Application.Commands.Server;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Queries.Servers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Huddle.Channel.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServersController : ControllerBase
    {
        private readonly IServersQueries _severQueries;
        private readonly IMediator _mediator;

        public ServersController(IServersQueries severQueries, IMediator mediator)
        {
            _severQueries = severQueries;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServerDto>>> GetByMemberId([FromQuery]Guid memberId)
        {
            var servers = await _severQueries.GetServersByMemberAsync(memberId);

            return Ok(servers);
        }

        // GET api/<ServersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServerDto>> Get(Guid id)
        {
            try
            {
                var server = await _severQueries.GetServerAsync(id);

                return Ok(server);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST api/<ServersController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateServerRequest request)
        {
            Guid creatorId = Guid.NewGuid(); // get from jwt

            var command = new CreateServerCommand(
                creatorId: creatorId,
                name: request.Name,
                isPrivate: request.isPrivate
            );

            var result = await _mediator.Send(command);

            return Created();
        }

        // PUT api/<ServersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] UpdateServerRequest request)
        {
            var command = new UpdateServerCommand(
                Id: id,
                name: request.Name
            );

            await _mediator.Send(command);

            return Ok();
        }

        // DELETE api/<ServersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteServerCommand(
                serverId: id
            );

            await _mediator.Send(command);

            return NoContent();
        }
    }
}
