using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuthenticationService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args){
            IConfiguration configuration;
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", optional: false);
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
            configuration = configurationBuilder.Build();
        
            var webCertificateConfig = configuration.GetSection("WebCertificate").Get<CertificateConfigurationData>();
            var webCertificate = AuthenticationServiceUtils.LoadX509Certificate2(webCertificateConfig);

            var webServerConfig = configuration.GetSection("WebServer").Get<WebServerConfigurationData>();
            var listenAddr = webServerConfig.Hostname != null && webServerConfig.Hostname == "*"? System.Net.IPAddress.Any : System.Net.IPAddress.Parse(webServerConfig.Hostname);
            return WebHost.CreateDefaultBuilder(args)
            .UseConfiguration(configuration)
            .UseKestrel(server =>
            {
                server.Listen(
                    address: listenAddr, 
                    port: (int)webServerConfig.Port, 
                    configure: listen => listen.UseHttps(webCertificate)
                );
            })
            .UseStartup<Startup>();
        }
            
    }
}
