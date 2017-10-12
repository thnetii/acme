using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using THNETII.Common.Cli;

namespace THNETII.Acme.Client.Cli
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                cts.Cancel();
                e.Cancel = true;
            };

            var cli = new CliBuilder<CliCommand>(typeof(Program))
                .Configuration(Configuration)
                .ConfigureServices(ConfigureServices)
                .ConfigureServices(services => services.AddSingleton(cts))
                .PreRunCommand(OnRun)
                .AddHelpOption()
                .AddVersionOption()
                .AddOption("-d|--directory=<URL>", "ACME directory URL", CommandOptionType.SingleValue, true, (directoryOption, configDict) => configDict[AcmeDirectoryConfigKey] = directoryOption.Value())
                .AddOption("-v|--verbose", "Verbose Output", CommandOptionType.NoValue, true, (verboseOption, configDict) =>
                {
                    if (verboseOption.HasValue())
                    {
#if DEBUG
                        configDict[LogLevelConfigKey] = nameof(LogLevel.Trace);
#else
                        configDict[LogLevelConfigKey] = nameof(LogLevel.Information);
#endif
                    }
                })
                .AddSubCommand<AboutCliCommand>("about", null, aboutCliApp =>
                {
                    SetSubCommandFullName(aboutCliApp, "Application About Command");
                    aboutCliApp.Description = "Get extensive application and runtime information.";
#if !DEBUG
                    aboutCliApp.ShowInHelpText = false;
#endif
                })
#if DEBUG
                .AddSubCommand<CancelCommand>("cancel", null, cancelCliApp => cancelCliApp.ShowInHelpText = false, throwOnUnexpectedArg: false)
#endif // DEBUG
                .AddSubCommand<DirectoryCommand>("directory", null, directoryCmdApp =>
                {
                    SetSubCommandFullName(directoryCmdApp, "ACME Directory Command");
                    directoryCmdApp.Description = "Print ACME directory";
                })
                .AddSubCommand<AccountCommand>("account-key", accountCliApp =>
                {
                    accountCliApp.AddOption("-t|--type <TYPE>", "JWK Type (EC, RSA, oct)", CommandOptionType.SingleValue, true,
                        (opt, config) => ConfigurationPath.Combine());
                }, accountCmdApp =>
                {
                    SetSubCommandFullName(accountCmdApp, "ACME Account Key Management Command");
                    accountCmdApp.Description = "Creates or imports an account key for ACME";
                })
                .AddSubCommand<RegisterCommand>("register", null, registerCmdApp =>
                {
                    SetSubCommandFullName(registerCmdApp, "ACME Registration Command");
                    registerCmdApp.Description = "Create or Update ACME account registration";
                })
                .Build();
            cli.ExtendedHelpText = @"
Command names can be shortened.
'directory' and 'dir' both match the directory command.";
            return cli.Execute(args ?? new string[0]);
        }

        private static void SetSubCommandFullName(CommandLineApplication directoryCmdApp, string appendSubCommmandFullName)
        {
            var directoryFullNameBuilder = new StringBuilder();
            using (var parentFullNameReader = new StringReader(directoryCmdApp.Parent?.FullName ?? string.Empty))
                directoryFullNameBuilder.AppendLine(parentFullNameReader.ReadLine());
            directoryFullNameBuilder.AppendFormat(appendSubCommmandFullName);
            directoryCmdApp.FullName = directoryFullNameBuilder.ToString();
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

        private static void OnRun(CommandLineApplication command, IServiceProvider serviceProvider)
        {
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory?
#if DEBUG
                .AddDebug(LogLevel.Trace)
#else
                .AddDebug(LogLevel.Information)
#endif
                .AddConsole(serviceProvider.GetService<IConfiguration>()?.GetSection("Logging"))
                ;
            var programLogger = loggerFactory?.CreateLogger(typeof(Program));
            Console.CancelKeyPress += (sender, e) => programLogger.LogDebug("Cancel Key Press detected: {cancelKey}", e.SpecialKey);
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

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddSingleton(System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(Program)).Assembly);
            services.AddSingleton(serviceProvider =>
            {
                var directoryUri = serviceProvider.GetService<IConfiguration>()?[AcmeDirectoryConfigKey];
                var httpClient = serviceProvider.GetService<HttpClient>();
                var acmeLogger = serviceProvider.GetService<ILogger<AcmeClient>>();

                return new AcmeClient(directoryUri, httpClient, acmeLogger);
            });
        }

    }
}
