using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using THNETII.Common.Cli;
using static Microsoft.Extensions.Configuration.ConfigurationPath;

namespace THNETII.Acme.Client.Cli
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var cli = new CliBuilder<CliCommand>(typeof(Program))
                .Configuration(Configuration)
                .ConfigureServices(ConfigureServices)
                .PreRunCommand(OnRun)
                .AddHelpOption()
                .AddVersionOption()
                .AddOption("-d|--directory=<URL>", "ACME directory URL", CommandOptionType.SingleValue, true, (directoryOption, configDict) => configDict[$"Acme{KeyDelimiter}Directory"] = directoryOption.Value())
                .AddOption("-v|--verbose", "Verbose Output", CommandOptionType.NoValue, (verboseOption, configDict) =>
                {
                    if (verboseOption.HasValue())
                    {
#if DEBUG
                        configDict[LogLevelConfigKey] = nameof(LogLevel.Trace);
#else
                        configDict[LogLevelConfigKey] = nameof(LogLevel.Warning);
#endif
                    }
                })
                .AddSubCommand<DirectoryCommand>("directory", directoryCmdApp =>
                {
                    directoryCmdApp.Description = "Print ACME directory";
                }, null)
                .Build();
            cli.ExtendedHelpText = @"
Command names can be shortened.
'directory' and 'dir' both match the directory command.";
            return cli.Execute(args ?? new string[0]);
        }

        private static readonly string LogLevelConfigKey = $"Logging{KeyDelimiter}LogLevel{KeyDelimiter}Default";

        private static IDictionary<string, string> GetDefaultConfiguration()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
#if DEBUG
                [LogLevelConfigKey] = nameof(LogLevel.Information)
#else
                [LogLevelConfigKey] = nameof(LogLevel.Warning)
#endif
            };
        }

        private static void OnRun(CommandLineApplication command, IServiceProvider serviceProvider)
        {
            serviceProvider.GetService<ILoggerFactory>()?
#if DEBUG
                .AddDebug(LogLevel.Trace)
#else
                .AddDebug(LogLevel.Information)
#endif
                .AddConsole(serviceProvider.GetService<IConfiguration>()?.GetSection("Logging"))
                ;
        }

        private static void Configuration(IConfigurationBuilder configBuilder)
        {
            configBuilder.AddInMemoryCollection(GetDefaultConfiguration());
            var assemblyFileName = System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(Program)).Assembly.Location;
            string executableDirectory;
            try { executableDirectory = Path.GetDirectoryName(assemblyFileName); }
            catch (ArgumentException) { executableDirectory = null; }
            catch (PathTooLongException) { executableDirectory = null; }
            if (!string.IsNullOrWhiteSpace(executableDirectory) && !string.Equals(Directory.GetCurrentDirectory(), executableDirectory, StringComparison.OrdinalIgnoreCase))
            {
                configBuilder.AddJsonFile(Path.Combine(executableDirectory, "appsettings.json"), optional: true);
#if DEBUG
                configBuilder.AddJsonFile(Path.Combine(executableDirectory, "appsettings.Debug.json"), optional: true);
#endif // DEBUG
            }
            configBuilder.AddJsonFile("appsettings.json", optional: true);
#if DEBUG
            configBuilder.AddJsonFile("appsettings.Debug.json", optional: true);
#endif // DEBUG
            configBuilder.AddEnvironmentVariables();
        }

        private static void ConfigureServices(IServiceCollection services) => services.AddLogging();
    }
}
