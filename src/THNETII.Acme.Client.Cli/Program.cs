using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;

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

            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {

            });
            var assemblyFileName = typeof(Program).GetTypeInfo().Assembly.Location;
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

            return -1;
        }
    }
}
