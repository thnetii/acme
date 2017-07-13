using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static THNETII.Acme.Client.UriSafeConverter;

namespace THNETII.Acme.Client
{
    public partial class AcmeClient
    {
        private static readonly string httpUserAgentProduct;
        private static readonly string httpUserAgentVersion;

        static AcmeClient()
        {
            var ai = typeof(AcmeClient).GetTypeInfo().Assembly;
            var ver = ai.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            AssemblyName an = ai.GetName();
            httpUserAgentProduct = an.Name;
            httpUserAgentVersion = ver ?? an.Version.ToString();
        }

        private static HttpRequestMessage SetUserAgent(HttpRequestMessage httpReqMsg)
        {
            //httpReqMsg.Headers.UserAgent.Clear();
            httpReqMsg.Headers.UserAgent.Add(new ProductInfoHeaderValue(httpUserAgentProduct, httpUserAgentVersion));
            return httpReqMsg;
        }

        private static JsonSerializer jsonSerializer = JsonSerializer.CreateDefault();

        private readonly SemaphoreSlim directory_guard = new SemaphoreSlim(initialCount: 0, maxCount: 1);
        private readonly SemaphoreSlim nonce_guard = new SemaphoreSlim(initialCount: 0, maxCount: 1);
        private readonly HttpClient httpClient;
        private string replayNonce;
        private readonly ILogger<AcmeClient> logger;
        private readonly AcmeConfiguration configuration;
        private readonly Task<AcmeDirectory> directoryLoadTask;

        public AcmeDirectory Directory => directoryLoadTask.GetAwaiter().GetResult();

        private async Task<TResponse> ReadHttpResponse<TResponse>(Task<HttpResponseMessage> httpRespMsgTask)
        {
            using (var httpRespMsg = await httpRespMsgTask)
            {
                logger?.LogInformation($"HTTP Response {{{nameof(httpRespMsg.StatusCode)}}} {{{nameof(httpRespMsg.ReasonPhrase)}}}", httpRespMsg.StatusCode, httpRespMsg.ReasonPhrase);
                var httpRespStreamTask = httpRespMsg.Content.ReadAsStreamAsync();
                replayNonce = httpRespMsg.Headers.TryGetValues("Replay-Nonce", out var replayNonces) ? replayNonces.FirstOrDefault() : null;
                logger?.LogDebug($"Replay-Nonce: {{{nameof(replayNonce)}}}", replayNonce);
                logger?.LogTrace("Releasing Nonce guard");
                nonce_guard.Release();

                using (var jsonRespRead = new JsonTextReader(new StreamReader(await httpRespStreamTask)))
                {
                    return jsonSerializer.Deserialize<TResponse>(jsonRespRead);
                }
            }
        }

        private Task<AcmeDirectory> LoadDirectoryAsync()
        {
            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, configuration.DirectoryUrl))
            {
                httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                SetUserAgent(httpRequestMessage);

                nonce_guard.Wait();
                logger?.LogTrace("Nonce guard locked.");
                logger?.LogInformation($"Requesting ACME directory from {{{nameof(httpRequestMessage.RequestUri)}}}", httpRequestMessage.RequestUri);
                return ReadHttpResponse<AcmeDirectory>(httpClient.SendAsync(httpRequestMessage));
            }
        }

        public AcmeClient(HttpClient httpClient, AcmeConfiguration configuration, ILogger<AcmeClient> logger = null)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(HttpClient));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.logger = logger;

            directoryLoadTask = LoadDirectoryAsync();
        }
    }
}
