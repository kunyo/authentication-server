using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AuthenticationService.Providers
{
    public class FileCertificateStore : ICertificateStore
    {
        private readonly string baseDir;

        public FileCertificateStore(string baseDir)
        {
            if (string.IsNullOrWhiteSpace(baseDir))
            {
                throw new System.ArgumentException("Value cannot be null or an empty string", nameof(baseDir));
            }

            this.baseDir = baseDir;
        }

        public async Task<X509Certificate2> GetCertificateAsync(string name, string password)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be null or white space.", nameof(name));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be null or white space.", nameof(password));
            }
            string absolutePath = Path.Combine(this.baseDir, name);
            if (!System.IO.File.Exists(absolutePath))
            {
                throw new ArgumentException("Certificate \"{name}\" not found in the certificate store. The full path was: {absolutePath}", nameof(name));
            }
            byte[] certificateBytes = await File.ReadAllBytesAsync(absolutePath);
            
            // Unprotect certificate
            var certificate = new X509Certificate2(certificateBytes, password);
            return certificate;
        }
    }
}