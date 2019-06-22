using AuthenticationService.Providers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AuthenticationService.IntegrationTests.Providers
{
    public class AwsSsmParameterSecretsStoreTests
    {
        private readonly TestConfiguration configuration;
        private readonly AwsSsmParameterSecretsStore secretsStore;

        public AwsSsmParameterSecretsStoreTests()
        {
            this.secretsStore = new AwsSsmParameterSecretsStore(new AwsSsmParameterSecretStoreConfiguration { ParameterStore = TestConfiguration.Current.ParameterStore });
        }

        [Fact]
        public async void MustGetServiceConfiguration()
        {
            var secret = await this.secretsStore.GetSecret("/web-certificate");

            Assert.Equal("NOVALUE", secret);
        }
    }
}
