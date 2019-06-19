namespace AuthenticationService
{
    public sealed class WebServerBindingConfigurationData
    {
        public string Protocol { get; set; }
        public string Ip { get; set; }
        public uint Port { get; set; }
        public CertificateConfigurationData Certificate { get; set; }
    }
}