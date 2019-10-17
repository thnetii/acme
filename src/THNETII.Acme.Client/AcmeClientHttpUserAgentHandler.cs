using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace THNETII.Acme.Client
{
    public class AcmeClientHttpUserAgentHandler : DelegatingHandler
    {
        private static readonly string httpUserAgentProduct;
        private static readonly string httpUserAgentVersion;

        public static ProductInfoHeaderValue UserAgent { get; }

        [SuppressMessage("Performance", "CA1810: Initialize reference type static fields inline")]
        static AcmeClientHttpUserAgentHandler()
        {
            var ai = typeof(AcmeClient)
#if NETSTANDARD1_3
                .GetTypeInfo()
#endif // NETSTANDARD1_3
                .Assembly;
            var ver = ai.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            AssemblyName an = ai.GetName();
            httpUserAgentProduct = an.Name;
            httpUserAgentVersion = ver ?? an.Version.ToString();
            UserAgent = new ProductInfoHeaderValue(httpUserAgentProduct, httpUserAgentVersion);
        }

        public AcmeClientHttpUserAgentHandler(HttpMessageHandler innerHandler) : base(innerHandler)
        { }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request?.Headers.UserAgent.Add(UserAgent);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
