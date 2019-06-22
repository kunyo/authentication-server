using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AuthenticationService.Providers
{
    public class SecretsCertificateStore : ICertificateStore
    {
        private readonly ISecretsStore secretsStore;

        public SecretsCertificateStore(ISecretsStore secretsStore)
        {
            this.secretsStore = secretsStore;
        }

        public async Task<X509Certificate2> GetCertificateAsync(string name, string password)
        {
            string base64String = await secretsStore.GetSecret(name);
            byte[] gzBytes = System.Convert.FromBase64String(base64String);

            using (var compressedStream = new MemoryStream(gzBytes))
            using (var uncompressedStream = new MemoryStream())
            using (var gz = new System.IO.Compression.GZipStream(compressedStream, System.IO.Compression.CompressionMode.Decompress))
            {
                gz.CopyTo(uncompressedStream);
                // Unprotect certificate
                var certificate = new X509Certificate2(uncompressedStream.ToArray(), password);
                return certificate;
            }
        }
    }
}