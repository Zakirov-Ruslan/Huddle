using Huddle.Channel.Application.Commands.Message;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Exceptions;
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
            var identityId = User.GetCurrentUserIdentityId();
            if (identityId == null)
                return Unauthorized();

            try
            {
                var messages = await _messageQueries.GetRecentAsync(channelId, pageSize, identityId.Value);

                return Ok(messages);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ForbiddenAccessException ex)
            {
                return Forbid();
            }
        }

        // GET api/<MessagesController>/5
        [HttpGet("older")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetOlder(Guid channelId, [FromQuery] Guid beforeMessageId, [FromQuery] int pageSize = 50)
        {
            var identityId = User.GetCurrentUserIdentityId();
            if (identityId == null)
                return Unauthorized();

            try
            {
                var messages = await _messageQueries.GetOlderAsync(channelId, pageSize, beforeMessageId, identityId.Value);

                return Ok(messages);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ForbiddenAccessException ex)
            {
                return Forbid();
            }
        }

        // POST api/<MessagesController>
        [HttpPost]
        public async Task<ActionResult> Post(Guid channelId, [FromBody] CreateMessageRequest request)
        {
            var identityId = User.GetCurrentUserIdentityId();
            if (identityId == null)
                return Unauthorized();

            try
            {
                CreateMessageCommand command = new(identityId.Value, channelId, request.Text);

                var createdMessage = await _mediator.Send(command);
                return Ok(createdMessage);
            }
            catch (ForbiddenAccessException ex)
            {
                return Forbid();
            }
        }

        // PUT api/<MessagesController>/5
        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(Guid id, [FromBody] UpdateMessageRequest request)
        {
            var identityId = User.GetCurrentUserIdentityId();
            if (identityId == null)
                return Unauthorized();

            UpdateMessageCommand command = new(id, identityId.Value, request.Text);

            try
            {
                var result = await _mediator.Send(command);

                return Ok();
            }
            catch (ForbiddenAccessException ex)
            {
                return Forbid();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE api/<MessagesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var identityId = User.GetCurrentUserIdentityId();
            if (identityId == null)
                return Unauthorized();

            DeleteMessageCommand command = new(id, identityId.Value);

            try
            {
                var result = await _mediator.Send(command);

                return NoContent();
            }
            catch (ForbiddenAccessException ex)
            {
                return Forbid();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
