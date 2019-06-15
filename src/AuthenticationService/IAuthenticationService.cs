using System.Threading.Tasks;

namespace AuthenticationService
{
    public interface IAuthenticationService
    {
        Task<ValidateResourceOwnerGrantResponse> ValidateResourceOwnerGrant(ValidateResourceOwnerGrantRequest request);
    }
}