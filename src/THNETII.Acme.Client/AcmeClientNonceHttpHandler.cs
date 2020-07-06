using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace THNETII.Acme.Client
{
    public class AcmeClientNonceHttpHandler : DelegatingHandler
    {
        private static SemaphoreSlim nonceGuard = new SemaphoreSlim(1);
        private volatile string replayNonce;

        public string ReplayNonce => replayNonce;

        [DebuggerStepThrough]
        public AcmeClientNonceHttpHandler(HttpMessageHandler innerHandler)
            : base(innerHandler) { }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancelToken)
        {
            await nonceGuard.WaitAsync(cancelToken)
                .ConfigureAwait(continueOnCapturedContext: false);
            cancelToken.ThrowIfCancellationRequested();
            var response = await base.SendAsync(request, cancelToken)
                .ConfigureAwait(continueOnCapturedContext: false);
            try
            {
                cancelToken.ThrowIfCancellationRequested();
                replayNonce = response.Headers.TryGetValues("Replay-Nonce", out var replayNonces)
                    ? replayNonces.FirstOrDefault()
                    : null;
            }
            finally { nonceGuard.Release(); }
            cancelToken.ThrowIfCancellationRequested();
            return response;
        }
    }
}
