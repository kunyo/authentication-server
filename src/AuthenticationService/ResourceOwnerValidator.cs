using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthenticationService
{
    public class ResourceOwnerValidator : IResourceOwnerPasswordValidator
    {
        private const string JwtIdentityProviderCode = "local";
        private readonly IAuthenticationService authenticationService;

        public ResourceOwnerValidator(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var clientId = context.Request.Client.ClientId;
            var scopes = context.Request.Scopes;
            var response = await authenticationService.ValidateResourceOwnerGrant(new ValidateResourceOwnerGrantRequest
            {
                ClientId = clientId,
                Username = context.UserName,
                Password = context.Password
            });

            if (!response.IsValid)
            {
                context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidGrant);
                return;
            }

            var claims = new List<Claim>();
            claims.Add(new Claim(IdentityModel.JwtClaimTypes.JwtId, Guid.NewGuid().ToString()));
            claims.Add(new Claim(IdentityModel.JwtClaimTypes.Subject, response.SubjectId));
            claims.Add(new Claim(IdentityModel.JwtClaimTypes.IdentityProvider, JwtIdentityProviderCode));
            claims.Add(new Claim(IdentityModel.JwtClaimTypes.AuthenticationMethod, IdentityModel.OidcConstants.AuthenticationMethods.Password));
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int unixTime = (int)t.TotalSeconds;
            claims.Add(new Claim(IdentityModel.JwtClaimTypes.AuthenticationTime, unixTime.ToString()));

            var identity = IdentityModel.Identity.Create(IdentityModel.OidcConstants.AuthenticationMethods.Password, claims.ToArray());
            var principal = new ClaimsPrincipal(identity);
            context.Result = new GrantValidationResult(principal);
        }
    }
}
