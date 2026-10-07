using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace BGK.TestSPA.Server.Keycloak
{
    public class KeycloakClaimsTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = principal.Identity as ClaimsIdentity ?? throw new NullReferenceException();

            var realmAccess = principal.FindFirst("realm_access")?.Value;
            if (realmAccess != null)
            {
                var roles = JsonSerializer.Deserialize<Dictionary<string, string[]>>(realmAccess) ?? [];

                if (roles.ContainsKey("roles"))
                    foreach (var role in roles["roles"])
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, role));
                        identity.AddClaim(new Claim("Roles", role));
                        identity.AddClaim(new Claim("role", role));
                    }
            }

            return Task.FromResult(principal);
        }
    }
}
