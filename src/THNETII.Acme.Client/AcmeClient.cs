using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using THNETII.Common;

namespace THNETII.Acme.Client
{
    public class AcmeClient : IDisposable
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

        private readonly SemaphoreSlim nonce_guard = new SemaphoreSlim(1);
        private readonly HttpClient httpClient;
        private string replayNonce;
        private readonly ILogger<AcmeClient> logger;
        private AcmeDirectory directory;
        private readonly bool disposeHttpClient = false;

        public Task<AcmeDirectory> InitDirectoryTask { get; }

        public AcmeDirectory Directory => InitDirectoryTask?.GetAwaiter().GetResult();

        private async Task<TResponse> ReadHttpResponse<TResponse>(Task<HttpResponseMessage> httpRespMsgTask, CancellationToken cancelToken = default(CancellationToken))
        {
            using (var httpResponseMessage = await httpRespMsgTask)
            {
                logger?.LogDebug($"Received HTTP Response {{{nameof(httpResponseMessage)}}}", httpResponseMessage);
                var httpRespStreamTask = httpResponseMessage.Content.ReadAsStreamAsync();
                replayNonce = httpResponseMessage.Headers.TryGetValues("Replay-Nonce", out var replayNonces) ? replayNonces.FirstOrDefault() : null;
                logger?.LogTrace("Releasing Nonce guard");
                nonce_guard.Release();

                cancelToken.ThrowIfCancellationRequested();
                using (var jsonRespRead = new JsonTextReader(new StreamReader(await httpRespStreamTask)))
                {
                    cancelToken.ThrowIfCancellationRequested();
                    return jsonSerializer.Deserialize<TResponse>(jsonRespRead);
                }
            }
        }

        public AcmeClient(string directoryUriString, HttpClient httpClient = null, ILogger<AcmeClient> logger = null)
            : this(httpClient, logger)
        {
            InitDirectoryTask = LoadDirectoryAsync(directoryUriString.ThrowIfNullOrWhiteSpace(nameof(directoryUriString)));
        }

        public AcmeClient(Uri directoryUri, HttpClient httpClient = null, ILogger<AcmeClient> logger = null)
            : this(httpClient, logger)
        {
            InitDirectoryTask = LoadDirectoryAsync(directoryUri.ThrowIfNull(nameof(directoryUri)));
        }

        private Task<AcmeDirectory> LoadDirectoryAsync(string directoryUri, CancellationToken cancelToken = default(CancellationToken))
        {
            HttpRequestMessage HttpRequestFromUriString() => new HttpRequestMessage(HttpMethod.Get, directoryUri);
            return LoadDirectoryAsync(HttpRequestFromUriString, cancelToken);
        }

        private Task<AcmeDirectory> LoadDirectoryAsync(Uri directoryUri, CancellationToken cancelToken = default(CancellationToken))
        {
            HttpRequestMessage HttpRequestFromUri() => new HttpRequestMessage(HttpMethod.Get, directoryUri);
            return LoadDirectoryAsync(HttpRequestFromUri, cancelToken);
        }

        private async Task<AcmeDirectory> LoadDirectoryAsync(Func<HttpRequestMessage> requestMessageFactory, CancellationToken cancelToken = default(CancellationToken))
        {
            using (var httpRequestMessage = requestMessageFactory())
            {
                httpRequestMessage.Headers.Accept.ParseAdd("application/json");
                logger?.LogInformation($"Requesting ACME directory from {{{nameof(httpRequestMessage.RequestUri)}}}", httpRequestMessage.RequestUri);
                directory = await LoadDirectoryAsyncWithHttpMessage(httpRequestMessage, cancelToken);
                return directory;
            }
        }

        private Task<AcmeDirectory> LoadDirectoryAsyncWithHttpMessage(HttpRequestMessage httpRequestMessage, CancellationToken cancelToken = default(CancellationToken))
        {
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            SetUserAgent(httpRequestMessage);

            nonce_guard.Wait();
            logger?.LogTrace($"Nonce guard locked");
            logger?.LogDebug($"Sending HTTP Request {{{nameof(httpRequestMessage)}}}", httpRequestMessage);
            return ReadHttpResponse<AcmeDirectory>(httpClient.SendAsync(httpRequestMessage, cancelToken));
        }

        private AcmeClient(HttpClient httpClient = null, ILogger<AcmeClient> logger = null)
        {
            if (httpClient == null)
            {
                this.httpClient = new HttpClient();
                disposeHttpClient = true;
            }
            else
                this.httpClient = httpClient;
            this.logger = logger;
        }

        void IDisposable.Dispose()
        {
            if (disposeHttpClient)
                httpClient.Dispose();
        }
    }
}
