using System.Security.Claims;

namespace Huddle.Channel.WebApi.Extensions
{
    internal static class ClaimsPrincipalExtensions
    {
        public static Guid? GetCurrentUserIdentityId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("sub")?.Value;
            return Guid.TryParse(claim, out var id) ? id : null;
        }
    }
}
