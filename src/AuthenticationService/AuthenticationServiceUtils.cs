using System;
using System.Security.Cryptography.X509Certificates;

namespace AuthenticationService
{
    internal static class AuthenticationServiceUtils
    {
        public static X509Certificate2 LoadX509Certificate2(CertificateConfigurationData src)
        {
            if (string.IsNullOrWhiteSpace(src.Path) || string.IsNullOrWhiteSpace(src.Password))
            {
                throw new Exception("Invalid certificate configuration data: both CertificatePath and Password must be configured.");
            }

            var certAbsolutePath = System.IO.Path.Combine(Environment.CurrentDirectory, src.Path);
            if (!System.IO.File.Exists(certAbsolutePath))
            {
                throw new Exception($"Invalid certificate configuration data. \"CertificatePath\" points to a non existing file: {certAbsolutePath}");
            }

            // Unprotect certificate
            var certificate = new X509Certificate2(certAbsolutePath, src.Password);
            return certificate;
        }
    }
}