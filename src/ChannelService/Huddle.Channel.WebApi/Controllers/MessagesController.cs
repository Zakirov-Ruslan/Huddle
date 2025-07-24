using Huddle.Channel.Application.Commands.Message;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Queries.Messages;
using Huddle.Channel.WebApi.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Huddle.Channel.WebApi.Controllers
{
    [Route("api/channels/{channelId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMessagesQueries _messageQueries;
        public MessagesController(IMediator mediator, IMessagesQueries messageQueries)
        {
            _mediator = mediator;
            _messageQueries = messageQueries;
        }

        // GET: api/<MessagesController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetRecent(Guid channelId, [FromQuery] int pageSize = 50)
        {
            var messages = await _messageQueries.GetRecentAsync(channelId, pageSize);

            return Ok(messages);
        }

        // GET api/<MessagesController>/5
        [HttpGet("older")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetOlder(Guid channelId, [FromQuery] Guid beforeMessageId, [FromQuery] int pageSize = 50)
        {
            var messages = await _messageQueries.GetOlderAsync(channelId, 50, beforeMessageId);

            return Ok(messages);
        }

        // POST api/<MessagesController>
        [HttpPost]
        public async Task<ActionResult> Post(Guid channelId, [FromBody] CreateMessageRequest request)
        {
            var identityId = User.GetCurrentUserIdentityId();
            if (identityId == null)
                return Unauthorized();

            CreateMemberCommand command = new(identityId.Value, channelId, request.Text);

            var createdMessage = await _mediator.Send(command);

            return Ok(createdMessage);
        }

        // PUT api/<MessagesController>/5
        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(Guid id, [FromBody] UpdateMessageRequest request)
        {
            var identityId = User.GetCurrentUserIdentityId();
            if (identityId == null)
                return Unauthorized();

            UpdateMessageCommand command = new(id, identityId.Value, request.Text);

            var result = await _mediator.Send(command);

            return Ok();
        }

        // DELETE api/<MessagesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var identityId = User.GetCurrentUserIdentityId();
            if (identityId == null)
                return Unauthorized();

            DeleteMessageCommand command = new(id, identityId.Value);

            var result = await _mediator.Send(command);

            return NoContent();
        }
    }
}
