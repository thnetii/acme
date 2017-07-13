using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using THNETII.Common.Cli;

namespace THNETII.Acme.Client.Cli
{
    internal class DirectoryCommand : CliCommand
    {
        private readonly IServiceProvider serviceProvider;

        public DirectoryCommand(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILogger<DirectoryCommand> logger = null
            ) : base(configuration, logger)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public override int Run(CommandLineApplication app)
        {
            if (string.IsNullOrWhiteSpace(Configuration?[Program.AcmeDirectoryConfigKey]))
            {
                Logger?.LogCritical("Missing required option: {option}", "directory");
                return 1;
            }

            var acmeClient = serviceProvider.GetRequiredService<AcmeClient>();
            app.Out.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(acmeClient.Directory, Newtonsoft.Json.Formatting.Indented));
            return 0;
        }

        private bool LogCritical(Exception except)
        {
            Logger?.LogCritical(default(EventId), except, "An unexpected error occurred.");
            return true;
        }
    }
}