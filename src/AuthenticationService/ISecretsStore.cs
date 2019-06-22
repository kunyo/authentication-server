using System.Threading.Tasks;

namespace AuthenticationService
{
    public interface ISecretsStore
    {
        Task<string> GetSecret(string name);
    }
}