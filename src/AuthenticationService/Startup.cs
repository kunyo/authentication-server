using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthenticationService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var builder = services.AddIdentityServer()
                            .AddInMemoryIdentityResources(AuthenticationServiceConfiguration.GetIdentityResources())
                            .AddInMemoryApiResources(AuthenticationServiceConfiguration.GetApis())
                            .AddInMemoryClients(AuthenticationServiceConfiguration.GetClients())
                            .AddResourceOwnerValidator<ResourceOwnerValidator>();

            services.AddSingleton<IAuthenticationService, AuthenticationService>();

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            // if (environmentName == "Development")
            // {
            //     builder.AddDeveloperSigningCredential();
            // }
            // else
            // {
            //     throw new Exception("need to configure key material");
            // }

            
            var certificateRelativePath = Configuration.GetValue<string>("SigningCredential:CertificatePath");
            var certificatePassword = Configuration.GetValue<string>("SigningCredential:Password");

            if(string.IsNullOrWhiteSpace(certificateRelativePath) || string.IsNullOrWhiteSpace(certificatePassword))
            {
                throw new Exception("Invalid SigningCredential configuration: both CertificatePath and Password must be configured.");
            }

            var certificatePath = System.IO.Path.Combine(Environment.CurrentDirectory, certificateRelativePath);

            if(!System.IO.File.Exists(certificatePath))
            {
                throw new Exception($"Invalid SigningCredential configuration. \"CertificatePath\" points to a non existing file: {certificatePath}");
            }

            var signingCertificate = new X509Certificate2(certificatePath, certificatePassword);
            builder.AddSigningCredential(signingCertificate);
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseIdentityServer();
            //app.UseMvc();
        }
    }
}
