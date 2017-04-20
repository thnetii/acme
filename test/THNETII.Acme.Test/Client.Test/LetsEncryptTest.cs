using System;
using System.Linq;
using System.Net.Http;
using Xunit;

namespace THNETII.Acme.Client.Test
{
    public class LetsEncryptTest
    {
        private static readonly Uri letsEncryptUri = new Uri(@"https://acme-v01.api.letsencrypt.org/");

        [Fact]
        public void IsLetsEncryptAvailable()
        {
            using (var httpClient = new HttpClient() { BaseAddress = letsEncryptUri })
            {
                var sendTask = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, "/"));
                sendTask.Wait();
                var responseMessage = sendTask.Result;
                responseMessage.EnsureSuccessStatusCode();
            }
        }
    }
}
