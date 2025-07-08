using Huddle.Channel.Application.Commands.Member;
using Huddle.Channel.Application.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Huddle.Channel.WebApi.Controllers
{
    [Route("api/servers/{serverId}/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MembersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST api/server/5/<MembersController>
        [HttpPost]
        public async Task<IActionResult> Post(Guid serverId, [FromBody] CreateMemberRequest request)
        {
            CreateMemberCommand command = new(serverId, request.IdentityId);

            var result = await _mediator.Send(command);

            return Created();
        }

        // PUT api/server/5/<MembersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid serverId, Guid id, [FromBody] UpdateMemberRequest request)
        {
            Guid sender = Guid.NewGuid(); // GetFromJWT

            UpdateMemberCommand command = new(request.Id, request.ServerUsername, sender, serverId);

            var result = await _mediator.Send(command);

            return Ok();
        }

        // DELETE api/server/5/<MembersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid serverId, Guid id)
        {
            Guid sender = Guid.NewGuid(); // GetFromJWT

            DeleteMemberCommand command = new(id, serverId, sender);

            var result = await _mediator.Send(command);

            return NoContent();
        }
    }
}
