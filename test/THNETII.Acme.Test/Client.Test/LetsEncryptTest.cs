using System;
using System.Linq;
using System.Net.Http;
using Xunit;

namespace THNETII.Acme.Client.Test
{
    public class LetsEncryptTest
    {
        private static readonly Uri letsEncryptUri = new Uri(@"https://acme-v01.api.letsencrypt.org/");
        private static readonly HttpClient httpClient = new HttpClient();

        [Fact]
        public void IsLetsEncryptAvailable()
        {
            var sendTask = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, "/"));
            sendTask.Wait();
            var responseMessage = sendTask.Result;
            responseMessage.EnsureSuccessStatusCode();
        }

        [Fact]
        public void LetsEncryptAcmeClientHasDirectoryWithUriAndHttpClient()
        {
            var letsEncryptDirectoryUri = new Uri(letsEncryptUri, "directory");
            var acmeClientCreateTask = AcmeClient.CreateAsync(httpClient, letsEncryptDirectoryUri);
            acmeClientCreateTask.Wait();
            var acmeClient = acmeClientCreateTask.Result;
            var acmeDirectory = acmeClient.Directory;
            Assert.NotNull(acmeDirectory.KeyChangeUri);
            Assert.NotNull(acmeDirectory.NewAuthzUri);
            Assert.NotNull(acmeDirectory.NewCertUri);
            Assert.NotNull(acmeDirectory.NewRegistrationUri);
            Assert.NotNull(acmeDirectory.RevokeCertUri);
        }

        [Fact]
        public void LetsEncryptAcmeClientHasDirectoryWithUri()
        {
            var letsEncryptDirectoryUri = new Uri(letsEncryptUri, "directory");
            var acmeClientCreateTask = AcmeClient.CreateAsync(letsEncryptDirectoryUri);
            acmeClientCreateTask.Wait();
            var acmeClient = acmeClientCreateTask.Result;
            var acmeDirectory = acmeClient.Directory;
            Assert.NotNull(acmeDirectory.KeyChangeUri);
            Assert.NotNull(acmeDirectory.NewAuthzUri);
            Assert.NotNull(acmeDirectory.NewCertUri);
            Assert.NotNull(acmeDirectory.NewRegistrationUri);
            Assert.NotNull(acmeDirectory.RevokeCertUri);
        }

        [Fact]
        public void LetsEncryptAcmeClientHasDirectoryWithUriStringAndHttpClient()
        {
            var letsEncryptDirectoryUri = new Uri(letsEncryptUri, "directory");
            var acmeClientCreateTask = AcmeClient.CreateAsync(httpClient, letsEncryptDirectoryUri.ToString());
            acmeClientCreateTask.Wait();
            var acmeClient = acmeClientCreateTask.Result;
            var acmeDirectory = acmeClient.Directory;
            Assert.NotNull(acmeDirectory.KeyChangeUri);
            Assert.NotNull(acmeDirectory.NewAuthzUri);
            Assert.NotNull(acmeDirectory.NewCertUri);
            Assert.NotNull(acmeDirectory.NewRegistrationUri);
            Assert.NotNull(acmeDirectory.RevokeCertUri);
        }

        [Fact]
        public void LetsEncryptAcmeClientHasDirectoryWithUriString()
        {
            var letsEncryptDirectoryUri = new Uri(letsEncryptUri, "directory");
            var acmeClientCreateTask = AcmeClient.CreateAsync(letsEncryptDirectoryUri.ToString());
            acmeClientCreateTask.Wait();
            var acmeClient = acmeClientCreateTask.Result;
            var acmeDirectory = acmeClient.Directory;
            Assert.NotNull(acmeDirectory.KeyChangeUri);
            Assert.NotNull(acmeDirectory.NewAuthzUri);
            Assert.NotNull(acmeDirectory.NewCertUri);
            Assert.NotNull(acmeDirectory.NewRegistrationUri);
            Assert.NotNull(acmeDirectory.RevokeCertUri);
        }
    }
}
