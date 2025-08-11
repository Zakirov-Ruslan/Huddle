using Huddle.Channel.Application.Commands.Member;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Queries.Members;
using Huddle.Channel.Domain;
using Huddle.Channel.WebApi.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Huddle.Channel.WebApi.Controllers
{
    [Route("api/servers/{serverId}/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMembersQueries _membersQueries;

        public MembersController(IMediator mediator, IMembersQueries membersQueries)
        {
            _mediator = mediator;
            _membersQueries = membersQueries;
        }

        [HttpGet("{memberId}")]
        public async Task<ActionResult<MemberDto>> Get(Guid memberId)
        {
            var member = await _membersQueries.GetAsync(memberId);

            return Ok(member);
        }

        [HttpGet()]
        public async Task<ActionResult<PaginatedItems<MemberDto>>> GetByServerId(Guid serverId, [FromQuery] Guid? cursor = null, [FromQuery] int limit = 50)
        {
            var members = await _membersQueries.GetByServerId(serverId, cursor, limit);

            return Ok(members);
        }

        // POST api/server/5/join
        [HttpPost("~/api/server/{serverId}/join")]
        public async Task<IActionResult> Post(Guid serverId)
        {
            var identityId = User.GetCurrentUserIdentityId();
            if (identityId == null)
                return Unauthorized();

            CreateMemberCommand command = new(serverId, identityId.Value);

            var result = await _mediator.Send(command);

            return Created();
        }

        // PUT api/<MembersController>/5
        [HttpPut("~/api/[controller]/{memberId}")]
        public async Task<IActionResult> Put(Guid memberId, [FromBody] UpdateMemberRequest request)
        {
            var identityId = User.GetCurrentUserIdentityId();
            if (identityId == null)
                return Unauthorized();

            UpdateMemberCommand command = new(memberId, request.ServerUsername, identityId.Value);

            var result = await _mediator.Send(command);

            return Ok();
        }

        // DELETE api/<MembersController>/5
        [HttpDelete("~/api/[controller]/{memberId}")]
        public async Task<IActionResult> Delete(Guid memberId)
        {
            var identityId = User.GetCurrentUserIdentityId();
            if (identityId == null)
                return Unauthorized();

            DeleteMemberCommand command = new(memberId, identityId.Value);

            var result = await _mediator.Send(command);

            return NoContent();
        }
    }
}
