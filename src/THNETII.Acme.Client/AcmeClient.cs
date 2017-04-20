using System;
using System.Net.Http;

namespace THNETII.Acme.Client
{
    public class AcmeClient : IDisposable
    {
        private AcmeDirectory directory;
        private readonly HttpClient httpClient;

        public AcmeDirectory Directory => directory;

        public AcmeClient(string directoryUriString) : this(new AcmeDirectory { DirectoryUriString = directoryUriString }) { }
        public AcmeClient(Uri directoryUri) : this(new AcmeDirectory { DirectoryUri = directoryUri }) { }
        public AcmeClient(AcmeDirectory directory) : this(directory ?? throw new ArgumentNullException(nameof(directory)), new HttpClient()) { }
        public AcmeClient(AcmeDirectory directory, HttpMessageHandler httpHandler) : this(directory ?? throw new ArgumentNullException(nameof(directory)), new HttpClient(httpHandler)) { }
        public AcmeClient(AcmeDirectory directory, HttpMessageHandler httpHandler, bool disposeHandler) : this(directory ?? throw new ArgumentNullException(nameof(directory)), new HttpClient(httpHandler, disposeHandler)) { }
        private AcmeClient(AcmeDirectory directory, HttpClient httpClient)
        {
            this.directory = directory;
            this.httpClient = httpClient;
        }

        public void Dispose() => httpClient.Dispose();
    }
}
