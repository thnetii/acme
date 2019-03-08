using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using Xunit;

namespace THNETII.Acme.Client.Test
{
    public class LetsEncryptTest
    {
        private static readonly Uri letsEncryptUri = new Uri(LetsEncrypt.DirectoryUri);
        private static readonly HttpMessageHandler httpHandler = new HttpClientHandler();

        [Fact]
        public void IsLetsEncryptAvailable()
        {
            using (var httpClient = new HttpClient(httpHandler, disposeHandler: false))
            {
                var sendTask = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, letsEncryptUri));
                var responseMessage = sendTask.GetAwaiter().GetResult();
                responseMessage.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public void LetsEncryptAcmeClientHasDirectoryWithUriAndHttpClient()
        {
            var acmeClientCreateTask = AcmeClient.CreateAsync(letsEncryptUri, httpHandler, disposeHandler: false);
            using (var acmeClient = acmeClientCreateTask.GetAwaiter().GetResult())
            {
                var acmeDirectory = acmeClient.Directory;
                Assert.NotNull(acmeDirectory.KeyChangeUri);
                Assert.NotNull(acmeDirectory.NewAuthzUri);
                Assert.NotNull(acmeDirectory.NewCertUri);
                Assert.NotNull(acmeDirectory.NewRegistrationUri);
                Assert.NotNull(acmeDirectory.RevokeCertUri);
            }
        }

        [Fact]
        public void LetsEncryptAcmeClientHasDirectoryWithUri()
        {
            var acmeClientCreateTask = AcmeClient.CreateAsync(letsEncryptUri);
            using (var acmeClient = acmeClientCreateTask.GetAwaiter().GetResult())
            {
                var acmeDirectory = acmeClient.Directory;
                Assert.NotNull(acmeDirectory.KeyChangeUri);
                Assert.NotNull(acmeDirectory.NewAuthzUri);
                Assert.NotNull(acmeDirectory.NewCertUri);
                Assert.NotNull(acmeDirectory.NewRegistrationUri);
                Assert.NotNull(acmeDirectory.RevokeCertUri);
            }
        }

        [Fact]
        [SuppressMessage("Usage", "CA2234: Pass system uri objects instead of strings")]
        public void LetsEncryptAcmeClientHasDirectoryWithUriStringAndHttpClient()
        {
            var letsEncryptDirectoryUri = new Uri(letsEncryptUri, "directory");
            var acmeClientCreateTask = AcmeClient.CreateAsync(LetsEncrypt.DirectoryUri, httpHandler, disposeHandler: false);
            using (var acmeClient = acmeClientCreateTask.GetAwaiter().GetResult())
            {
                var acmeDirectory = acmeClient.Directory;
                Assert.NotNull(acmeDirectory.KeyChangeUri);
                Assert.NotNull(acmeDirectory.NewAuthzUri);
                Assert.NotNull(acmeDirectory.NewCertUri);
                Assert.NotNull(acmeDirectory.NewRegistrationUri);
                Assert.NotNull(acmeDirectory.RevokeCertUri);
            }
        }

        [Fact]
        [SuppressMessage("Usage", "CA2234: Pass system uri objects instead of strings")]
        public void LetsEncryptAcmeClientHasDirectoryWithUriString()
        {
            var letsEncryptDirectoryUri = new Uri(letsEncryptUri, "directory");
            var acmeClientCreateTask = AcmeClient.CreateAsync(LetsEncrypt.DirectoryUri);
            using (var acmeClient = acmeClientCreateTask.GetAwaiter().GetResult())
            {
                var acmeDirectory = acmeClient.Directory;
                Assert.NotNull(acmeDirectory.KeyChangeUri);
                Assert.NotNull(acmeDirectory.NewAuthzUri);
                Assert.NotNull(acmeDirectory.NewCertUri);
                Assert.NotNull(acmeDirectory.NewRegistrationUri);
                Assert.NotNull(acmeDirectory.RevokeCertUri);
            }
        }
    }
}
