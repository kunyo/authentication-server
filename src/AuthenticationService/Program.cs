using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationService.Providers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace AuthenticationService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            IConfiguration configuration;
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", optional: false);
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
            configuration = configurationBuilder.Build();

            return WebHost.CreateDefaultBuilder(args)
            .UseConfiguration(configuration)
            .UseStartup<Startup>()
            .UseKestrel(server =>
            {
                var certificatesProvider = server.ApplicationServices.GetRequiredService<ICertificatesProvider>();
                var bindings = configuration.GetSection("Listen").Get<IEnumerable<WebServerBindingConfigurationData>>();
                if (bindings == null || bindings.Count() == 0)
                {
                    throw new Exception("Listen: configuration section contains invalid configuration data. No binding defined.");
                }
                foreach (var binding in bindings)
                {
                    switch (binding.Protocol.ToUpper())
                    {
                        case "HTTP":
                            server.Listen(System.Net.IPAddress.Parse(binding.Ip), (int)binding.Port);
                            break;
                        case "HTTPS":
                            var getCertificateTask = certificatesProvider.GetCertificateAsync(binding.Certificate.Path, binding.Certificate.Password);
                            getCertificateTask.Wait();
                            var bindingCertificate = getCertificateTask.Result;

                            server.Listen(
                                System.Net.IPAddress.Parse(binding.Ip),
                                (int)binding.Port,
                                listen => listen.UseHttps(bindingCertificate)
                            );
                            break;
                        default:
                            throw new NotSupportedException("Listen: invalid binding protocol. Accepted values are either \"HTTP\" or \"HTTPS\"");
                    }
                }
            });
        }
    }
}
