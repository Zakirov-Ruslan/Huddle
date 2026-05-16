using Huddle.Channel.Application.Commands.Message;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Exceptions;
using Huddle.Channel.Application.Queries.Messages;
using Huddle.Channel.Domain;
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

        [HttpGet]
        public async Task<ActionResult<PaginatedItems<MessageDto>>> Get(Guid channelId, [FromQuery] Guid? cursor = null, [FromQuery] bool older = true, [FromQuery] int limit = 20)
        {
            var identityId = User.GetCurrentUserIdentityId();
            if (identityId == null)
                return Unauthorized();

            try
            {
                var messages = await _messageQueries.GetMessages(identityId.Value, channelId, cursor, older, limit);

                return Ok(messages);
            }
            catch (ForbiddenAccessException)
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
            catch (ForbiddenAccessException)
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
            catch (ForbiddenAccessException)
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
            catch (ForbiddenAccessException)
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
