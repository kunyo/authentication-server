using System.Threading.Tasks;
using IdentityServer4.Services;

namespace AuthenticationService
{
    public sealed class CorsPolicyService : ICorsPolicyService
    {
        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            return Task.FromResult(true);
        }
    }
}