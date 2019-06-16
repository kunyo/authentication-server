using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AuthenticationService.SmokeTests
{
    public class AuthenticationServiceTests : IDisposable
    {
        private const string ServerBaseUrl = "https://localhost:44331";
        private const string TestClientId = "client-1";
        private const string TestClientSecret = "client-1-secret";
        private readonly HttpClientHandler httpClientHandler;
        private readonly HttpClient client;

        public AuthenticationServiceTests()
        {
            this.httpClientHandler = new HttpClientHandler();
            this.httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            this.client = new HttpClient(httpClientHandler);
            this.client.BaseAddress = new Uri(ServerBaseUrl);
        }

        public void Dispose()
        {
            this.client.Dispose();
            this.httpClientHandler.Dispose();
        }

        [Fact]
        public async void MustGetServiceConfiguration()
        {
            var response = await client.GetAsync("/.well-known/openid-configuration");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(stringResponse);
            Assert.True(jsonResponse.ContainsKey("issuer"));
            Assert.True(jsonResponse.ContainsKey("token_endpoint"));
            Assert.True(jsonResponse.ContainsKey("introspection_endpoint"));
        }

        [Fact]
        public async void MustGetTokenWithValidCredentials()
        {
            var tokenInfo = await PostGetToken(TestClientId, TestClientSecret, "john.smith", "S3cret!");
            Assert.False(string.IsNullOrWhiteSpace(tokenInfo.access_token));
            Assert.Equal("Bearer", tokenInfo.token_type);
        }

        [Fact]
        public async void MustIntrospectToken()
        {
            var tokenInfo = await PostGetToken(TestClientId, TestClientSecret, "john.smith", "S3cret!");

            string authorizationHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes("test-api:test-api-secret"));
            var content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "token", tokenInfo.access_token }
            });
            client.DefaultRequestHeaders.Add("Authorization", new[] { "Basic " + authorizationHeader });
            var response = await client.PostAsync("/connect/introspect", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonConvert.DeserializeObject<IntrospectTokenContract>(stringResponse);
            Assert.True(jsonResponse.active);
            Assert.Equal("pwd", jsonResponse.amr);
            Assert.True(jsonResponse.auth_time > 0);
            Assert.Equal(TestClientId, jsonResponse.client_id);
            Assert.True(jsonResponse.exp > 0);
            Assert.Equal("local", jsonResponse.idp);
            Assert.Equal(ServerBaseUrl, jsonResponse.iss);
            Assert.Equal("test-api", jsonResponse.scope);
            Assert.Equal("81f92af1-1874-4dec-9f32-e1acade9bec5", jsonResponse.sub);
        }

        [Fact]
        public async void MustNotGetTokenWithInvalidClient()
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                    {
                        { "grant_type", "password" },
                        { "client_id", "helo" },
                        { "client_secret", "helo" },
                        { "username", "helo" },
                        { "password", "helo" }
                    });
            var getTokenResponse = await client.PostAsync("/connect/token", content);
            Assert.Equal(HttpStatusCode.BadRequest, getTokenResponse.StatusCode);
        }

        private async Task<AccessTokenContract> PostGetToken(string clientId, string clientSecret, string username, string password)
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                    {
                        { "grant_type", "password" },
                        { "client_id", clientId },
                        { "client_secret", clientSecret },
                        { "username", username },
                        { "password", password }
                    });
            var getTokenResponse = await client.PostAsync("/connect/token", content);
            Assert.Equal(HttpStatusCode.OK, getTokenResponse.StatusCode);
            var stringResponse = await getTokenResponse.Content.ReadAsStringAsync();
            var tokenInfo = JsonConvert.DeserializeObject<AccessTokenContract>(stringResponse);
            return tokenInfo;
        }

        private class AccessTokenContract
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
        }

        private class IntrospectTokenContract
        {
            public bool active { get; set; }
            public int exp { get; set; }
            public string iss { get; set; }
            public string client_id { get; set; }
            public string sub { get; set; }
            public string idp { get; set; }
            public string amr { get; set; }
            public string scope { get; set; }
            public int auth_time { get; set; }
        }
    }
}
