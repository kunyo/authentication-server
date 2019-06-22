using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AuthenticationService
{
    public interface ICertificateStore
    {
        Task<X509Certificate2> GetCertificateAsync(string name, string password);
    }
}