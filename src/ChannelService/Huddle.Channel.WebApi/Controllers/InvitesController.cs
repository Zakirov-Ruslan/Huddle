using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Queries.Invites;
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

        // POST /servers/{serverId}/invites
        [HttpPost]
        public async Task<IActionResult> CreateInvite(Guid serverId, [FromBody] CreateInviteRequest request)
        {
            //request.ServerId = serverId;
            //var response = await _inviteService.CreateInviteAsync(request);
            //return Created($"/invites/{response.Code}", response);

            return null;
        }

        // GET /servers/{serverId}/invites
        [HttpGet]
        public async Task<IActionResult> GetInvites(Guid serverId)
        {
            //var invites = await _inviteService.GetInvitesByServerIdAsync(serverId);
            //return Ok(invites);

            return null;
        }

        // GET /servers/{serverId}/invites
        [HttpGet]
        public async Task<IActionResult> GetInvitesByUserId(Guid userId)
        {
            //var invites = await _inviteService.GetInvitesByUserIdAsync(serverId);
            //return Ok(invites);

            return null;
        }

        // GET /invites/{code}
        [HttpGet]
        public async Task<IActionResult> GetInvite(string code)
        {
            //var invite = await _inviteService.GetInviteByCodeAsync(code);
            //if (invite == null) return NotFound();
            //return Ok(invite);

            return null;
        }

        // DELETE /invites/{code}
        [HttpDelete]
        public async Task<IActionResult> DeleteInvite(string code)
        {
            //await _inviteService.RevokeInviteAsync(code);
            //return NoContent();

            return null;
        }
    }
}
