using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AuthenticationService.Providers;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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
            var secretsConfig = Configuration.GetSection("Secrets");
            var secretsProviderName = secretsConfig.GetValue<string>("Provider");
            ICertificateStore certificatesStore;
            ISecretsStore secretsStore = null;
            switch (secretsProviderName)
            {
                case "aws_ssm_parameter":
                    secretsStore = new AwsSsmParameterSecretsStore(secretsConfig.Get<AwsSsmParameterSecretStoreConfiguration>());
                    certificatesStore = new SecretsCertificateStore(secretsStore);
                    break;
                default:
                    secretsStore = new PassThruSecretStore();
                    certificatesStore = new FileCertificateStore(Directory.GetCurrentDirectory());
                    break;
            }
            ICertificatesProvider certificatesProvider = new CertificatesProvider(certificatesStore, secretsStore);
            services.AddSingleton<ICertificatesProvider>(certificatesProvider);

            var builder = services.AddIdentityServer()
                            .AddInMemoryIdentityResources(AuthenticationServiceConfiguration.GetIdentityResources())
                            .AddInMemoryApiResources(AuthenticationServiceConfiguration.GetApis())
                            .AddInMemoryClients(AuthenticationServiceConfiguration.GetClients())
                            .AddResourceOwnerValidator<ResourceOwnerValidator>()
                            .AddCorsPolicyService<CorsPolicyService>();

            services.AddSingleton<IAuthenticationService, AuthenticationService>();

            // Configure the certificate used to signin OAUTH2 Tokens
            var signingCredentialConfig = Configuration.GetSection("SigningCredential").Get<CertificateConfigurationData>();

            var getSigningCredentialTask = certificatesProvider.GetCertificateAsync(signingCredentialConfig.Path, signingCredentialConfig.Password);
            getSigningCredentialTask.Wait();
            var signingCredential = getSigningCredentialTask.Result;
            builder.AddSigningCredential(signingCredential);

            // services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
            // app.UseMvc();
        }
    }
}
