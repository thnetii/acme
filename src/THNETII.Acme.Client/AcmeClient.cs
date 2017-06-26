using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using static THNETII.Acme.Client.UriSafeConverter;

namespace THNETII.Acme.Client
{
    public class AcmeClient : IDisposable
    {
        private static JsonSerializer jsonSerialiser = JsonSerializer.CreateDefault();

        private static Task<AcmeClient> CreateAsync(string directoryUriString)
        {
            directoryUriString.ThrowIfNullOrWhiteSpace(nameof(directoryUriString));
            return CreateAsync(StringToUriSafe(directoryUriString));
        }

        private async static Task<AcmeClient> CreateAsync(Uri uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            var httpClient = new HttpClient();
            throw new NotImplementedException();
        }

        private readonly HttpClient httpClient;
        private AcmeDirectory dircetory;

        public AcmeDirectory Directory => Directory;

        /// <summary>
        /// Releases the unmanaged resources and disposes of the managed resources used by
        /// the <see cref="AcmeClient"/> and the underlying <see cref="HttpClient"/>.
        /// </summary>
        public void Dispose() => httpClient.Dispose();
    }
}
