using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Ontap.Models;

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

        public static bool HasRights(this User user, BeerPrice price) => 
            user.IsAdmin ||
            user.CanAdminBrewery && price.Beer.Brewery.Admins.Any(u => u.User.Id == user.Id) ||
            user.CanAdminPub && price.Pub.Admins.Any(u => u.User.Id == user.Id);

        public static bool HasRights(this User user, BeerKegOnTap keg) => 
            user.IsAdmin ||
            user.CanAdminBrewery && keg.Keg.Beer.Brewery.Admins.Any(u => u.User.Id == user.Id) ||
            user.CanAdminPub && keg.Tap.Pub.Admins.Any(u => u.User.Id == user.Id);

        public static bool HasRights(this User user, Brewery brewery) => 
            user.IsAdmin ||
            user.CanAdminBrewery && brewery.Admins.Any(u => u.User.Id == user.Id);

        public static bool HasRights(this User user, Pub pub) =>
            user.IsAdmin ||
            user.CanAdminPub && 
            (
                pub.Admins != null && pub.Admins.Any(u => u.User.Id == user.Id) ||
                user.Pubs != null && user.Pubs.Any(p => p.Id == pub.Id)
            );

    }
}
