using Huddle.Channel.Application.Commands.Message;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Queries.Messages;
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
        [HttpGet("/older")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetOlder(Guid channelId, [FromQuery] Guid beforeMessageId, [FromQuery] int pageSize = 50)
        {
            var messages = await _messageQueries.GetOlderAsync(channelId, 50, beforeMessageId);

            return Ok(messages);
        }

        // POST api/<MessagesController>
        [HttpPost]
        public async Task<ActionResult> Post(Guid channelId, [FromBody] CreateMessageRequest request)
        {
            CreateMemberCommand command = new(request.AuthorId, channelId, request.Text);

            var result = await _mediator.Send(command);

            return Ok();
        }

        // PUT api/<MessagesController>/5
        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(Guid id, [FromBody] UpdateMessageRequest request)
        {
            Guid commandSenderId = Guid.NewGuid(); // GetFromJwt

            UpdateMessageCommand command = new(request.MessageId, commandSenderId, request.Text);

            var result = await _mediator.Send(command);

            return Ok();
        }

        // DELETE api/<MessagesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            Guid commandSenderId = Guid.NewGuid(); // GetFromJwt

            DeleteMessageCommand command = new(id, commandSenderId);

            var result = await _mediator.Send(command);

            return NoContent();
        }
    }
}
