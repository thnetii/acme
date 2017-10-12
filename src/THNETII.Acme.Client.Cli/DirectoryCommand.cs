using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using THNETII.Common.Cli;

namespace THNETII.Acme.Client.Cli
{
    internal class DirectoryCommand : CliAsyncCommand
    {
        private readonly AcmeClient acmeClient;
        private readonly CancellationTokenSource cts;

        public DirectoryCommand(
            AcmeClient acmeClient,
            IConfiguration configuration,
            CancellationTokenSource cts = null,
            ILogger<DirectoryCommand> logger = null
            ) : base(configuration, logger)
        {
            this.acmeClient = acmeClient ?? throw new ArgumentNullException(nameof(acmeClient));
            this.cts = cts;
        }

        public override async Task<int> RunAsync(CommandLineApplication app)
        {
            try
            {
                var directory = await acmeClient.InitDirectoryTask;
                using (var jsonWriter = new JsonTextWriter(app.Out) { CloseOutput = false, Formatting = Formatting.Indented })
                    JsonSerializer.CreateDefault().Serialize(jsonWriter, directory);
            }
            catch (OperationCanceledException cancelExcept)
            {
                Logger?.LogError(
                    new EventId(cancelExcept.HResult, cancelExcept.GetType().Name), 
#if DEBUG
                    cancelExcept, 
#endif // DEBUG
                    $"Cancelled loading the ACME directory."
                    );
                return 1;
            }

            return 0;
        }
    }
}