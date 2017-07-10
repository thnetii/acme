using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using THNETII.Common.Cli;
using static Microsoft.Extensions.Configuration.ConfigurationPath;

namespace THNETII.Acme.Client.Cli
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var cliapp = new CliApplication<CliCommand>();
            cliapp
                .Configuration(configBuilder =>
                {
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

                })
                .ConfigureServices(services =>
                {
                    services.AddLogging();
                })
                .AddHelpOption()
                .AddVersionOption()
                .AddOption("-d|--directory", "ACME directory URL", CommandOptionType.SingleValue, true, (directoryOption, configDict) => configDict[$"Acme{KeyDelimiter}Directory"] = directoryOption.Value())
                //.AddSubCommand<DirectoryCommand>("directory", subCliBuilder =>
                //{
                //})
                ;

            cliapp.Executing += (sender, serviceProvider) =>
            {
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                loggerFactory?
                    .AddDebug()
                    .AddConsole(serviceProvider.GetService<IConfiguration>()?.GetSection("Logging"))
                    ;
            };

            return cliapp.Execute(args ?? new string[0]);
        }
    }
}
