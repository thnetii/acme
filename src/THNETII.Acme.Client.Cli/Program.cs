using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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

            rootCommand.AddOption(
                new Option(
                    new[] { "-d", "--directory" }, "ACME directory URL"
                )
                { Argument = new Argument<Uri>() { Name = "URI" } }
                );
            rootCommand.AddOption(new Option("--verbose", "Verbose Output"));

            var parser = new CommandLineBuilder(rootCommand)
                .UseDefaults()
                .UseHost(Host.CreateDefaultBuilder, host =>
                {
                    host.ConfigureServices(ConfigureServices);
                    host.ConfigureLogging((context, logging) =>
                    {
                        _ = context.Properties.TryGetValue(typeof(InvocationContext), out object invocationObj);
                        var invocation = invocationObj as InvocationContext;
                        var parseResult = invocation?.ParseResult;
                        if (parseResult?.HasOption("--verbose") ?? false)
                        {
                            logging.SetMinimumLevel(LogLevel.Debug);
                        }
                    });
                })
                .Build();

            return parser.InvokeAsync(args);
        }

        internal static readonly string AcmeDirectoryConfigKey = ConfigurationPath.Combine("Acme", "Directory");
        internal static readonly string LogLevelConfigKey = ConfigurationPath.Combine("Logging", nameof(LogLevel), "Default");

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(Program).Assembly);
            services.AddSingleton(serviceProvider =>
            {
                var directoryUri = serviceProvider.GetService<IConfiguration>()?[AcmeDirectoryConfigKey];
                var httpHandler = serviceProvider.GetService<HttpMessageHandler>();
                var acmeLogger = serviceProvider.GetService<ILogger<AcmeClient>>();

                return AcmeClient.CreateAsync(
                    directoryUri.NotNullOrWhiteSpace(otherwise: LetsEncrypt.DirectoryUri),
                    httpHandler, disposeHandler: false
                    ).ConfigureAwait(false).GetAwaiter().GetResult();
            });
        }

    }
}
