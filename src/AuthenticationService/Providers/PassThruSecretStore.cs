using System.Threading.Tasks;

namespace AuthenticationService
{
    public sealed class PassThruSecretStore : ISecretsStore
    {
        public Task<string> GetSecret(string name)
        {
            return Task.FromResult(name);
        }
    }
}