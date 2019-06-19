using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Extensions.Configuration;

namespace AuthenticationService.SmokeTests
{
    public class TestConfiguration
    {
        static TestConfiguration()
        {
            var testEnvironment = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT");
            if (string.IsNullOrWhiteSpace(testEnvironment))
            {
                throw new InvalidOperationException("Test settings validation failed: TEST_ENVIRONMENT environment variable must be defined");
            }

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile($"appsettings.json", optional: false);
            configurationBuilder.AddJsonFile($"appsettings.{testEnvironment}.json", optional: true);
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
            var configuration = configurationBuilder.Build();
            Current = new TestConfiguration(configuration);
        }

        public static TestConfiguration Current { get; }

        private readonly IConfigurationRoot configuration;

        private TestConfiguration(IConfigurationRoot configuration)
        {
            this.configuration = configuration;
            this.BaseUrl = configuration.GetValue<string>("ApiClient:BaseUrl") ?? throw new ArgumentException("fds", nameof(configuration));
        }

        public string BaseUrl { get; }

        private class ApiClientConfiguration
        {
            public string BaseUrl { get; set; }
        }
    }
}