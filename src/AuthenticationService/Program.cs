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

            return WebHost.CreateDefaultBuilder(args)
            .UseConfiguration(configuration)
            .UseKestrel(server =>
            {
                var bindings = configuration.GetSection("Listen").Get<IEnumerable<WebServerBindingConfigurationData>>();
                if(bindings == null || bindings.Count() == 0)
                {
                    throw new Exception("Listen: configuration section contains invalid configuration data. No binding defined.");
                }                
                foreach(var binding in bindings)
                {
                    switch(binding.Protocol.ToUpper())
                    {
                        case "HTTP":
                            server.Listen(System.Net.IPAddress.Parse(binding.Ip), (int)binding.Port);
                            break;
                        case "HTTPS":
                            var certificateInfo = AuthenticationServiceUtils.LoadX509Certificate2(binding.Certificate);                        
                            server.Listen(System.Net.IPAddress.Parse(binding.Ip), (int)binding.Port, listen => listen.UseHttps(certificateInfo));
                            break;
                        default:
                            throw new NotSupportedException("Listen: invalid binding protocol. Accepted values are either \"HTTP\" or \"HTTPS\"");
                    }
                }
            })
            .UseStartup<Startup>();
        }
            
    }
}
