using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly Account[] users;

        public AuthenticationService()
        {
            this.users = new[] {
                new Account{ id = 1000, account_id = "81f92af1-1874-4dec-9f32-e1acade9bec5", username = "john.smith", password = "S3cret!", state = AccountState.Active, claims = new[]{ "user-claim-1", "user-claim-2" } },
                new Account{ id = 1001, account_id = "e43ab9c2-dad9-41f7-964c-448437280e88", username = "john.smith2", password = "S3cret!", claims = new[]{ "user-claim-1", "user-claim-2" } }
            };
        }

        public Task<ValidateResourceOwnerGrantResponse> ValidateResourceOwnerGrant(ValidateResourceOwnerGrantRequest request)
        {
            var user = users.FirstOrDefault(x => x.username == request.Username && x.password == request.Password);
            if (user != null && user.state == AccountState.Active)
            {
                return Task.FromResult(new ValidateResourceOwnerGrantResponse(true, user.account_id));
            }
            return Task.FromResult(ValidateResourceOwnerGrantResponse.Invalid);
        }

        private class Account
        {
            public long id { get; set; }
            public string account_id { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public AccountState state { get; set; }
            public string[] claims { get; set; }
        }

        private enum AccountState
        {
            Inactive,
            Active
        }
    }
}
