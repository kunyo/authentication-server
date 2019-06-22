using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Extensions.Configuration;

namespace AuthenticationService.IntegrationTests
{
    public class TestConfiguration
    {
        static TestConfiguration()
        {
            var environmentConfigFile = Environment.GetEnvironmentVariable("TEST_CONFIG");
            if (string.IsNullOrWhiteSpace(environmentConfigFile))
            {
                throw new InvalidOperationException("TEST_CONFIG environment variable must be defined");
            }

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile(environmentConfigFile, optional: false);
            var configuration = configurationBuilder.Build();
            Current = new TestConfiguration(configuration);
        }

        public static TestConfiguration Current { get; }

        private readonly IConfigurationRoot configuration;

        private TestConfiguration(IConfigurationRoot configuration)
        {
            this.configuration = configuration;
        }

        public string ParameterStore { 
            get{
                return configuration.GetValue<string>("Secrets:ParameterStore");
            }
        }
    }
}