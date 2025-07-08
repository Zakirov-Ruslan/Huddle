using Huddle.Channel.Application.Commands.Channel;
using Huddle.Channel.Application.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Huddle.Channel.WebApi.Controllers
{
    [Route("api/Servers/{serverId}/[controller]")]
    [ApiController]
    public class ChannelsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChannelsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST api/servers/5/<ChannelsController>
        [HttpPost]
        public async Task<ActionResult> Post(Guid serverId, CreateChannelRequest createChannelRequest)
        {
            var command = new CreateChannelCommand(serverId, createChannelRequest.ChannelType, createChannelRequest.Name);

            var result = await _mediator.Send(command);

            return Created();
        }

        // PUT api/servers/5/<ChannelsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid serverId, Guid id, UpdatedChannelRequest updateChannelRequest)
        {
            var command = new UpdateChannelCommand(serverId, updateChannelRequest.Id, updateChannelRequest.Name);

            var result = await _mediator.Send(command);

            return Ok();
        }

        // DELETE api/servers/5/<ChannelsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid serverId, Guid id)
        {
            var command = new DeleteChannelCommand(serverId, id);

            var result = await _mediator.Send(command);

            return NoContent();
        }
    }
}
