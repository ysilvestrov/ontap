using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Ontap.Auth
{
    public static class AuthHelper
    {
        public static AuthorizationPolicyBuilder RequireUserType(this AuthorizationPolicyBuilder policy, params string[] userTypes)
        {
            return
                policy.RequireAssertion(
                    context => context.User.HasClaim(c => c.Type == "UserType" && userTypes.Contains(c.Value)));
        }
    }
}
