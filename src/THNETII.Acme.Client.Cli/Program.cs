using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using THNETII.Common;
using THNETII.Common.Reflection;

namespace THNETII.Acme.Client.Cli
{
    public static class Program
    {
        public static Task Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var assemblyAccessor = new AssemblyAttributesAccessor(typeof(Program).Assembly);
            var rootCommand = new RootCommand(assemblyAccessor.Description);

            rootCommand.AddOption(new Option(
                new[] { "-d", "--directory" }, "ACME directory URL",
                new Argument<Uri>() { Name = "URI" }
                ));
            rootCommand.AddOption(new Option("--verbose", "Verbose Output"));

            return rootCommand.InvokeAsync(args);
        }

        internal static readonly string AcmeDirectoryConfigKey = ConfigurationPath.Combine("Acme", "Directory");
        internal static readonly string LogLevelConfigKey = ConfigurationPath.Combine("Logging", nameof(LogLevel), "Default");

        private static IDictionary<string, string> GetDefaultConfiguration()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [AcmeDirectoryConfigKey] = LetsEncrypt.DirectoryUri,

#if DEBUG
                [LogLevelConfigKey] = nameof(LogLevel.Information)
#else
                [LogLevelConfigKey] = nameof(LogLevel.Warning)
#endif
            };
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(Program).GetTypeInfo().Assembly);
            services.AddSingleton(serviceProvider =>
            {
                var directoryUri = serviceProvider.GetService<IConfiguration>()?[AcmeDirectoryConfigKey];
                var httpClient = serviceProvider.GetService<HttpClient>();
                var acmeLogger = serviceProvider.GetService<ILogger<AcmeClient>>();

                return new AcmeClient(directoryUri.NotNullOrWhiteSpace(otherwise: LetsEncrypt.DirectoryUri), httpClient, acmeLogger);
            });
        }

    }
}
