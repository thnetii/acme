using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using THNETII.Common.Cli;

namespace THNETII.Acme.Client.Cli
{
    internal class RegisterCommand : CliAsyncCommand
    {
        private readonly object acmeClient;
        private readonly CancellationTokenSource cts;

        public RegisterCommand(AcmeClient acmeClient,
            IConfiguration configuration,
            CancellationTokenSource cts = null,
            ILogger<RegisterCommand> logger = null
            ) : base(configuration, logger)
        {
            this.acmeClient = acmeClient ?? throw new ArgumentNullException(nameof(acmeClient));
            this.cts = cts;
        }
    }
}