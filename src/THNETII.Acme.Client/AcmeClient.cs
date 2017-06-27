using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static THNETII.Acme.Client.UriSafeConverter;

namespace THNETII.Acme.Client
{
    public partial class AcmeClient : IDisposable
    {
        private static readonly string httpUserAgentProduct;
        private static readonly string httpUserAgentVersion;

        static AcmeClient()
        {
            var ai = typeof(AcmeClient).GetTypeInfo().Assembly;
            var ver = ai.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            httpUserAgentProduct = ai.FullName;
            httpUserAgentVersion = ver ?? ai.GetName().Version.ToString();
        }

        private static HttpRequestMessage SetUserAgent(HttpRequestMessage httpReqMsg)
        {
            //httpReqMsg.Headers.UserAgent.Clear();
            httpReqMsg.Headers.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue(httpUserAgentProduct, httpUserAgentVersion));
            return httpReqMsg;
        }

        private static JsonSerializer jsonSerializer = JsonSerializer.CreateDefault();

        public static Task<AcmeClient> CreateAsync(string directoryUriString)
        {
            directoryUriString.ThrowIfNullOrWhiteSpace(nameof(directoryUriString));
            Uri directoryUri;
            try { directoryUri = new Uri(directoryUriString); }
            catch (UriFormatException uriFmtExcept) { throw new ArgumentException(uriFmtExcept.Message, nameof(directoryUriString), uriFmtExcept); }
            return CreateInternalAsync(new HttpClient(), directoryUri);
        }

        public static Task<AcmeClient> CreateAsync(Uri uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            var httpClient = new HttpClient();
            return CreateInternalAsync(httpClient, uri);
        }

        public static Task<AcmeClient> CreateAsync(HttpClient httpClient, string directoryUriString)
        {
            directoryUriString.ThrowIfNullOrWhiteSpace(nameof(directoryUriString));
            Uri directoryUri;
            try { directoryUri = new Uri(directoryUriString); }
            catch (UriFormatException uriFmtExcept) { throw new ArgumentException(uriFmtExcept.Message, nameof(directoryUriString), uriFmtExcept); }
            return CreateInternalAsync(httpClient ?? throw new ArgumentNullException(nameof(httpClient)), directoryUri);
        }

        public static Task<AcmeClient> CreateAsync(HttpClient httpClient, Uri directoryUri) => CreateInternalAsync(httpClient ?? throw new ArgumentNullException(nameof(httpClient)), directoryUri ?? throw new ArgumentNullException(nameof(directoryUri)));

        private static Task<AcmeClient> CreateInternalAsync(HttpClient httpClient, Uri uri)
        {
            throw new NotImplementedException();
        }

        private readonly SemaphoreSlim nonce_guard = new SemaphoreSlim(initialCount: 0, maxCount: 1);
        private readonly HttpClient httpClient;
        private AcmeDirectory directory;
        private string replayNonce;

        public AcmeDirectory Directory => directory;

        async Task<TResponse> ReadHttpResponse<TResponse>(Task<HttpResponseMessage> httpRespMsgTask)
        {
            using (var httpRespMsg = await httpRespMsgTask)
            {
                var httpRespStreamTask = httpRespMsg.Content.ReadAsStreamAsync();
                replayNonce = httpRespMsg.Headers.TryGetValues("Replay-Nonce", out var replayNonces) ? replayNonces.FirstOrDefault() : null;
                nonce_guard.Release();


                using (var jsonRespRead = new JsonTextReader(new StreamReader(await httpRespStreamTask)))
                {
                    return jsonSerializer.Deserialize<TResponse>(jsonRespRead);
                }
            }
        }

        private async Task LoadDirectory(Uri directoryUri)
        {
            var httpReqMsg = new HttpRequestMessage(HttpMethod.Get, directoryUri);
            SetUserAgent(httpReqMsg);
            directory = await ReadHttpResponse<AcmeDirectory>(httpClient.SendAsync(httpReqMsg));
        }

        /// <summary>
        /// Releases the unmanaged resources and disposes of the managed resources used by
        /// the <see cref="AcmeClient"/> and the underlying <see cref="HttpClient"/>.
        /// </summary>
        public void Dispose() => httpClient.Dispose();
    }
}
