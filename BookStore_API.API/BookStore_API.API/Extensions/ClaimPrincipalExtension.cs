using System.Security.Claims;

namespace BookStore_API.API.Extensions
{
    /// <summary>
    /// Extension methods for ClaimsPrincipal to retrieve user-related information.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Retrieves the user ID from claims if available.
        /// </summary>
        public static int GetUserId(this ClaimsPrincipal user)
        {
            if (int.TryParse(user.Claims?.FirstOrDefault(c => c.Type == "userId")?.Value, out int id))
            {
                return id;
            }

            return 0;
        }


        /// <summary>
        /// Retrieves the role associated with the user.
        /// </summary>
        public static List<string> GetRoles(this ClaimsPrincipal user)
        {
            List<string> roles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value.ToString()).ToList();
            var roles1 = user.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();

            return roles;
        }
    }
}
