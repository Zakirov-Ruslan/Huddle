using Huddle.Channel.Application.Commands.Member;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Queries.Members;
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
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetByServerId(Guid serverId)
        {
            var members = await _membersQueries.GetByServerId(serverId);

            return Ok(members);
        }

        // POST api/server/5/<MembersController>
        [HttpPost]
        public async Task<IActionResult> Post(Guid serverId, [FromBody] CreateMemberRequest request)
        {
            CreateMemberCommand command = new(serverId, request.IdentityId);

            var result = await _mediator.Send(command);

            return Created();
        }

        // PUT api/<MembersController>/5
        [HttpPut("~/api/[controller]/{memberId}")]
        public async Task<IActionResult> Put(Guid memberId, [FromBody] UpdateMemberRequest request)
        {
            Guid sender = Guid.NewGuid(); // GetFromJWT

            UpdateMemberCommand command = new(memberId, request.ServerUsername, sender);

            var result = await _mediator.Send(command);

            return Ok();
        }

        // DELETE api/<MembersController>/5
        [HttpDelete("~/api/[controller]/{memberId}")]
        public async Task<IActionResult> Delete(Guid memberId)
        {
            Guid sender = Guid.NewGuid(); // GetFromJWT

            DeleteMemberCommand command = new(memberId, sender);

            var result = await _mediator.Send(command);

            return NoContent();
        }
    }
}
