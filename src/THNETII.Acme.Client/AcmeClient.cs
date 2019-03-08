using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using THNETII.Networking.Http;

namespace THNETII.Acme.Client
{
    public class AcmeClient : IDisposable
    {
        private static readonly MediaTypeWithQualityHeaderValue jsonAcceptHeader = new MediaTypeWithQualityHeaderValue(HttpWellKnownMediaType.ApplicationJson);
        private static JsonSerializer jsonSerializer = JsonSerializer.CreateDefault();

        private readonly HttpClient httpClient;
        private AcmeDirectory directory;
        private readonly AcmeClientNonceHttpHandler nonceHandler;

        public string ReplayNonce => nonceHandler.ReplayNonce;
        public AcmeDirectory Directory => directory;

        public AcmeClient(HttpClient httpClient, AcmeClientNonceHttpHandler nonceHandler, AcmeDirectory directory)
            : base()
        {
            this.httpClient = httpClient;
            this.nonceHandler = nonceHandler;
            this.directory = directory;
        }

        [SuppressMessage("Usage", "CA2234: Pass system uri objects instead of strings")]
        public static Task<AcmeClient> CreateAsync(string directoryUriString,
            CancellationToken cancelToken = default) =>
            CreateAsync(directoryUriString, httpHandler: default, cancelToken);

        public static Task<AcmeClient> CreateAsync(Uri directoryUri,
            CancellationToken cancelToken = default) =>
            CreateAsync(directoryUri, httpHandler: default, cancelToken);

        [SuppressMessage("Usage", "CA2234: Pass system uri objects instead of strings")]
        public static Task<AcmeClient> CreateAsync(string directoryUriString,
            HttpMessageHandler httpHandler, CancellationToken cancelToken = default) =>
            CreateAsync(directoryUriString, httpHandler, disposeHandler: true, cancelToken);

        public static Task<AcmeClient> CreateAsync(Uri directoryUri,
            HttpMessageHandler httpHandler, CancellationToken cancelToken = default) =>
            CreateAsync(directoryUri, httpHandler, disposeHandler: true, cancelToken);

        [SuppressMessage("Usage", "CA2234: Pass system uri objects instead of strings")]
        public static Task<AcmeClient> CreateAsync(string directoryUriString,
            HttpMessageHandler httpHandler, bool disposeHandler,
            CancellationToken cancelToken = default) =>
            CreateAsync(directoryUriString, (s => new HttpRequestMessage(HttpMethod.Get, s)),
                httpHandler, disposeHandler, cancelToken);

        public static Task<AcmeClient> CreateAsync(Uri directoryUri,
            HttpMessageHandler httpHandler, bool disposeHandler,
            CancellationToken cancelToken = default) =>
            CreateAsync(directoryUri, (u => new HttpRequestMessage(HttpMethod.Get, u)),
                httpHandler, disposeHandler, cancelToken);

        private static async Task<AcmeClient> CreateAsync<TUri>(TUri directoryUri,
            Func<TUri, HttpRequestMessage> httpGetFactory,
            HttpMessageHandler httpHandler, bool disposeHandler,
            CancellationToken cancelToken = default)
        {
            if (httpHandler is null)
            {
                httpHandler = new HttpClientHandler();
                disposeHandler = true;
            }

            var nonceHandler = new AcmeClientNonceHttpHandler(httpHandler);
            var userAgentHandler = new AcmeClientHttpUserAgentHandler(nonceHandler);
            var httpClient = new HttpClient(userAgentHandler, disposeHandler);

            var directory = await GetAcmeDirectoryAsync(directoryUri, httpClient,
                httpGetFactory, cancelToken).ConfigureAwait(false);

            return new AcmeClient(httpClient, nonceHandler, directory);
        }

        private static async Task<TResponse> ReadHttpResponseAsync<TResponse>(
            Task<HttpResponseMessage> responseTask, CancellationToken cancelToken = default)
        {
            using (var msg = await responseTask.ConfigureAwait(false))
            {
                if (!msg.Content.IsJson())
                    throw new HttpRequestException($"Invalid Content-Type in HTTP Response message. Expected JSON content, but got: {msg.Content.Headers.ContentType}");
                using (var textReader = await msg.Content.ReadAsStreamReaderAsync(Encoding.UTF8).ConfigureAwait(false))
                using (var jsonReader = new JsonTextReader(textReader))
                {
                    cancelToken.ThrowIfCancellationRequested();
                    return jsonSerializer.Deserialize<TResponse>(jsonReader);
                }
            }
        }

        private static async Task<AcmeDirectory> GetAcmeDirectoryAsync<TUri>(
            TUri directoryUri, HttpClient httpClient,
            Func<TUri, HttpRequestMessage> httpGetFactory,
            CancellationToken cancelToken = default)
        {
            using (var requestMsg = httpGetFactory(directoryUri))
            {
                requestMsg.Headers.Accept.Add(jsonAcceptHeader);

                return await ReadHttpResponseAsync<AcmeDirectory>(
                    httpClient.SendAsync(requestMsg, cancelToken)
                    ).ConfigureAwait(false);
            }
        }

        private async Task<AcmeDirectory> GetAcmeDirectoryAsync<TUri>(
            TUri directoryUri, Func<TUri, HttpRequestMessage> httpGetFactory,
            CancellationToken cancelToken = default)
        {
            var directory = await GetAcmeDirectoryAsync(directoryUri,
                httpClient, httpGetFactory, cancelToken).ConfigureAwait(false);
            this.directory = directory;
            return directory;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                httpClient.Dispose();

                disposedValue = true;
            }
        }

        ~AcmeClient()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
