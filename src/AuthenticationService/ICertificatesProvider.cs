using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AuthenticationService
{
    public interface ICertificatesProvider
    {
        Task<X509Certificate2> GetCertificateAsync(string certificatename, string secretName);
    }
}