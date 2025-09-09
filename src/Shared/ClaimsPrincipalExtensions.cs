using System.Security.Claims;

namespace Huddle.Channel.WebApi.Extensions
{
    internal static class ClaimsPrincipalExtensions
    {
        public static Guid? GetCurrentUserIdentityId(this ClaimsPrincipal user)
        {
            var claim = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (claim == null)
                return null;
            return Guid.TryParse(claim.Value, out var id) ? id : null;
        }
    }
}
