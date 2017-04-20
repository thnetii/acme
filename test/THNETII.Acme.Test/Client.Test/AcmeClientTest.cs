using System;
using System.Linq;
using Xunit;

namespace THNETII.Acme.Client.Test
{
    public class AcmeClientTest
    {
        [Fact]
        public void CanCreateAcmeClientWithUriString()
        {
            const string directoryUriString = @"https://example.org/acme/directory";
            using (var client = new AcmeClient(directoryUriString))
            {
                Assert.NotNull(client.Directory);
                Assert.Equal(directoryUriString, client.Directory.DirectoryUriString);
            }
        }

        [Fact]
        public void CanCreateAcmeClientWithUri()
        {
            Uri directoryUri = new Uri(@"https://example.org/acme/directory");
            using (var client = new AcmeClient(directoryUri))
            {
                Assert.NotNull(client.Directory);
                Assert.Same(directoryUri, client.Directory.DirectoryUri);
            }
        }
    }
}
