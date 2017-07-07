using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using THNETII.Common.Cli;

namespace THNETII.Acme.Client.Cli
{
    internal class DirectoryCommand : CliCommand
    {
        public DirectoryCommand() : base() { }

        public DirectoryCommand(IConfiguration configuration, ILogger<DirectoryCommand> logger = null) : base(configuration, logger) { }

        public override int Run(CommandLineApplication app)
        {
            try { throw new NotImplementedException(); }
            catch (Exception except) when (LogCritical(except)) { throw; }
        }

        private bool LogCritical(Exception except)
        {
            Logger?.LogCritical(default(EventId), except, null);
            return false;
        }
    }
}