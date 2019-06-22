using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AuthenticationService
{
    public class CertificatesProvider : ICertificatesProvider
    {
        private readonly ICertificateStore certificatesStore;
        private readonly ISecretsStore secretsStore;

        public CertificatesProvider(ICertificateStore certificatesStore, ISecretsStore secretsStore)
        {
            this.certificatesStore = certificatesStore;
            this.secretsStore = secretsStore;
        }

        public async Task<X509Certificate2> GetCertificateAsync(string certificateName, string secretName)
        {
            var password = await this.secretsStore.GetSecret(secretName);
            var certificate = await this.certificatesStore.GetCertificateAsync(certificateName, password);

            return certificate;
        }
    }
}