using Huddle.Channel.WebApi.Extensions;
using Huddle.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Huddle.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfilesController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserProfileDto>> GetUserProfile(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            return new UserProfileDto
            {
                UserId = user.Id,
                UserName = user.UserName
            };
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserProfileDto>> GetMyProfile()
        {
            var identityId = User.GetCurrentUserIdentityId();
            if (identityId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(identityId.Value.ToString());
            if (user == null)
                return NotFound();

            return new UserProfileDto
            {
                UserId = user.Id,
                UserName = user.UserName
            };
        }
    }

    public class UserProfileDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
