using AuthenticationService.Providers;
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

        [Theory]
        [InlineData("/signing-credential-data")]
        [InlineData("/signing-credential-password")]
        [InlineData("/web-certificate-data")]
        [InlineData("/web-certificate-password")]
        public async void MustNotContainEmptySecrets(string paramName)
        {
            var secret = await this.secretsStore.GetSecret(paramName);

            Assert.True(!string.IsNullOrWhiteSpace(secret));
        }
    }
}
