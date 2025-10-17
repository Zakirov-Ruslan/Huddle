using Huddle.Channel.Application.Commands;
using Huddle.Channel.Application.Commands.Invite;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Queries.Invites;
using Huddle.Channel.WebApi.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<IEnumerable<InviteDto>>> GetInvites(Guid serverId)
        {
            var invites = await _invitesQuesries.GetInvitesByServerId(serverId);

            return Ok(invites);
        }

        // POST api/servers/{serverId}/invites
        [HttpPost("~/api/servers/{serverId}/invites")]
        public async Task<IActionResult> CreateInvite(Guid serverId)
        {
            var identityId = User.GetCurrentUserIdentityId();
            if (identityId == null)
                return Unauthorized();

            var command = new CreateInviteCommand(serverId, identityId.Value);

            var invite = await _mediator.Send(command);

            return Ok(invite);
        }

        // POST api/invites/{code}/accept
        [HttpPost("{code}/accept")]
        public async Task<IActionResult> AcceptInvite([FromHeader(Name = "x-requestid")] Guid requestId, string code)
        {
            var identityId = User.GetCurrentUserIdentityId();
            if (identityId == null)
                return Unauthorized();

            var acceptInviteCommand = new AcceptInviteCommand(code, identityId.Value);
            var command = new IdentifiedCommand<AcceptInviteCommand, Guid>(acceptInviteCommand, requestId);
            try
            {
                var serverId = await _mediator.Send(command);

                return Ok(new { ServerId = serverId });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(409);
                throw;
            }

        }

        // DELETE api/invites/{inviteId}
        [HttpDelete("{inviteId}")]
        public async Task<IActionResult> DeleteInvite(Guid inviteId)
        {
            var command = new DeleteInviteCommand(inviteId);
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
