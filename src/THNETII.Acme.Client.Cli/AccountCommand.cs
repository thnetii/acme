using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using THNETII.Common.Cli;

namespace THNETII.Acme.Client.Cli
{
    public class AccountCommand : CliCommand
    {
        public AccountCommand() : base() { }

        public AccountCommand(IConfiguration configuration, ILogger<AccountCommand> logger) : base(configuration, logger) { }
    }
}