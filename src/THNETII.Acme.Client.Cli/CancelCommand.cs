using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using THNETII.Common.Cli;

namespace THNETII.Acme.Client.Cli
{
#if DEBUG
    internal class CancelCommand : CliCommand
    {
        private readonly ManualResetEventSlim cancelEvent;

        public CancelCommand(IConfiguration config, ILogger<CancelCommand> logger)
            : base(config, logger)
        {
            cancelEvent = new ManualResetEventSlim(initialState: false);
            Console.CancelKeyPress += (sender, e) => cancelEvent.Set();
        }

        public override int Run(CommandLineApplication app)
        {
            Logger?.LogInformation("Waiting for cancel key press event");
            cancelEvent.Wait();

            return app.Parent.Execute(app.RemainingArguments.ToArray());
        }
    }
#endif // DEBUG
}