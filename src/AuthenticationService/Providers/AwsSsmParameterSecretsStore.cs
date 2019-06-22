using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Amazon.Runtime;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace AuthenticationService.Providers
{
    public class AwsSsmParameterSecretStoreConfiguration
    {
        public string ParameterStore { get; set; }
    }

    public class AwsSsmParameterSecretsStore : ISecretsStore
    {
        private readonly AmazonSimpleSystemsManagementClient awsSsmClient;
        private readonly Task<IDictionary<string, string>> secretsPreloadTask;

        public AwsSsmParameterSecretsStore(AwsSsmParameterSecretStoreConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (string.IsNullOrWhiteSpace(configuration.ParameterStore))
            {
                throw new ArgumentException($"Configuration property \"{nameof(configuration.ParameterStore)}\" cannot be null or whitespace.", nameof(configuration));
            }

            this.awsSsmClient = new AmazonSimpleSystemsManagementClient();
            this.secretsPreloadTask = LoadSecrets(configuration.ParameterStore);
        }

        public async Task<string> GetSecret(string name)
        {
            var secrets = await secretsPreloadTask;
            string secret;
            if (!secrets.TryGetValue(name, out secret))
            {
                throw new Exception($"Secret store does not contain a secret with name \"{name}\".");
            }

            return secret;
        }

        private async Task<IDictionary<string, string>> LoadSecrets(string parameterStoreArn)
        {
            GetParametersByPathResponse response;
            try
            {
                response = await awsSsmClient.GetParametersByPathAsync(new GetParametersByPathRequest
                {
                    Path = parameterStoreArn,
                    WithDecryption = true
                });
            }
            catch (HttpRequestException exception)
            {
                throw new Exception($"Could not load secrets from paramete store. See inner exception. Parameter store was: {parameterStoreArn}", exception);
            }

            var result = new Dictionary<string, string>();
            foreach (var p in response.Parameters)
            {
                Console.WriteLine($"FULL_NAME {p.Name} NAME {p.Name.Substring(parameterStoreArn.Length)} VALUE {p.Value}");
                result.Add(p.Name.Substring(parameterStoreArn.Length), p.Value);
            }

            return result;
        }
    }
}