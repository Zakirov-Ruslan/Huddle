using Huddle.Channel.Application.Commands.Invite;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Queries.Invites;
using Huddle.Channel.WebApi.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Huddle.Channel.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvitesController : ControllerBase
    {
        private readonly IInvitesQueries _invitesQuesries;
        private readonly IMediator _mediator;
        public InvitesController(IMediator mediator, IInvitesQueries invitesQuesries)
        {
            _mediator = mediator;
            _invitesQuesries = invitesQuesries;
        }

        // GET api/servers/{serverId}/invites
        [HttpGet("~/api/servers/{serverId}/invites")]
        public async Task<IActionResult> GetInvites(Guid serverId)
        {
            var invites = await _invitesQuesries.GetInvitesByServerId(serverId);

            return Ok(invites);
        }

        // GET api/invites/
        [HttpGet]
        public async Task<IActionResult> GetInvitesByUserId()
        {
            var identityId = User.GetCurrentUserIdentityId();
            if (identityId == null)
                return Unauthorized();

            var invites = await _invitesQuesries.GetInvitesByServerId(identityId.Value);

            return Ok(invites);
        }

        // POST api/servers/{serverId}/invites
        [HttpPost("~/api/servers/{serverId}/invites")]
        public async Task<IActionResult> CreateInvite(Guid serverId, [FromBody] CreateInviteRequest request)
        {
            var command = new CreateInviteCommand(serverId, request.UserId);

            var result = await _mediator.Send(command);

            return Created();
        }

        // POST api/invites/{inviteId}/accept
        [HttpPost("{inviteId}/accept")]
        public async Task<IActionResult> AcceptInvite(Guid inviteId)
        {
            var command = new AcceptInviteCommand(inviteId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // POST api/invites/{inviteId}/decline
        [HttpPost("{inviteId}/decline")]
        public async Task<IActionResult> DeclineInvite(Guid inviteId)
        {
            var command = new DeclineInviteCommand(inviteId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // DELETE api/invites/{code}
        [HttpDelete("{inviteId}")]
        public async Task<IActionResult> DeleteInvite(Guid inviteId)
        {
            var command = new DeleteInviteCommand(inviteId);
            var result = await _mediator.Send(command);
            return NoContent();
        }
    }
}
