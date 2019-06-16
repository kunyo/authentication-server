using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            // Configure the certificate used to signin OAUTH2 Tokens
            var signingCredentialConfig = Configuration.GetSection("SigningCredential").Get<CertificateConfigurationData>();
            var signingCredential = AuthenticationServiceUtils.LoadX509Certificate2(signingCredentialConfig);
            builder.AddSigningCredential(signingCredential);
            
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
