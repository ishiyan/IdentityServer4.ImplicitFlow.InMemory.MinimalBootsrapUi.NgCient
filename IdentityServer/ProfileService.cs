using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.Extensions.Logging;

// ReSharper disable once ClassNeverInstantiated.Global
namespace IdentityServer
{
    public sealed class ProfileService : TestUserProfileService
    {
        public ProfileService(TestUserStore users, ILogger<ProfileService> logger)
            : base(users, logger)
        {
        }

        public override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.LogProfileRequest(Logger);
            if (context.RequestedClaimTypes.Any())
            {
                TestUser user = Users.FindBySubjectId(context.Subject.GetSubjectId());
                if (user != null)
                {
                    context.AddRequestedClaims(user.Claims);
                    var apiLevel = user.Claims.Where(claim => claim.Type == Config.ApiLevel);
                    context.IssuedClaims.AddRange(apiLevel);
                }
            }

            context.LogIssuedClaims(Logger);
            return Task.CompletedTask;
        }
    }
}