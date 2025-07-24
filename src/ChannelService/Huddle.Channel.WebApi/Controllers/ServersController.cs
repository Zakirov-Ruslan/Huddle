using Huddle.Channel.Application.Commands.Server;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Exceptions;
using Huddle.Channel.Application.Queries.Servers;
using Huddle.Channel.WebApi.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Huddle.Channel.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServersController : ControllerBase
    {
        private readonly IServersQueries _serverQueries;
        private readonly IMediator _mediator;

        public ServersController(IServersQueries severQueries, IMediator mediator)
        {
            _serverQueries = severQueries;
            _mediator = mediator;
        }

        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<ServerDto>>> GetServersForCurrentUser()
        {
            var identityId = User.GetCurrentUserIdentityId();
            if (identityId == null)
                return Unauthorized();

            var servers = await _serverQueries.GetServersByMemberAsync(identityId.Value);
            return Ok(servers);
        }

        // GET api/<ServersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServerDto>> Get(Guid id)
        {
            var identityId = User.GetCurrentUserIdentityId();
            if (identityId == null)
                return Unauthorized();

            try
            {
                var server = await _serverQueries.GetServerAsync(id, identityId.Value);

                return Ok(server);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ForbiddenAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        // POST api/<ServersController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateServerRequest request)
        {
            var identityId = User.GetCurrentUserIdentityId();
            if (identityId == null)
                return Unauthorized();

            var command = new CreateServerCommand(
                creatorId: identityId.Value,
                name: request.Name,
                isPrivate: request.isPrivate
            );

            try
            {
                var createdServer = await _mediator.Send(command);

                return CreatedAtRoute(nameof(Get), new { Id = createdServer.Id }, createdServer);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<ServersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] UpdateServerRequest request)
        {
            var identityId = User.GetCurrentUserIdentityId();
            if (identityId == null)
                return Unauthorized();

            var command = new UpdateServerCommand(
                id: id,
                name: request.Name,
                commandSenderIdenityId: identityId.Value
            );

            try
            {
                await _mediator.Send(command);

                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<ServersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var identityId = User.GetCurrentUserIdentityId();
            if (identityId == null)
                return Unauthorized();

            var command = new DeleteServerCommand(
                serverId: id,
                commandSenderIdenityId: identityId.Value
            );

            try
            {
                await _mediator.Send(command);

                return NoContent();
            }
            catch (ForbiddenAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
