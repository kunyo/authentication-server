using System;
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
            X509Certificate2 certificate;
            try
            {
                var password = await this.secretsStore.GetSecret(secretName);
                certificate = await this.certificatesStore.GetCertificateAsync(certificateName, password);
            }
            catch (Exception exception)
            {
                throw new Exception($"Could not get certificate \"{certificateName}\". See inner exception.", exception);
            }

            return certificate;
        }
    }
}