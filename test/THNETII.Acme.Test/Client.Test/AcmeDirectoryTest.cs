using System;
using System.Linq;
using Xunit;

namespace THNETII.Acme.Client.Test
{
    public class AcmeDirectoryTest
    {
        [Fact]
        public void NewAcmeDirectoryHasNullDirectoryUriString() => Assert.Null((new AcmeDirectory()).DirectoryUriString);

        [Fact]
        public void NewAcmeDirectoryHasNullDirectoryUri() => Assert.Null((new AcmeDirectory()).DirectoryUri);

        [Fact]
        public void AcmeDirectoryWithBadDirectoryUriStringReturnsNullUriObject()
        {
            var directory = new AcmeDirectory { DirectoryUriString = "This is a badly formatted Uri." };

            Assert.Null(directory.DirectoryUri);
        }

        [Fact]
        public void AcmeDirectoryWithDirectoryUriStringReturnsUriObject()
        {
            const string directoryUriString = "https://example.org/";
            var directory = new AcmeDirectory { DirectoryUriString = directoryUriString };

            var directoryUri = directory.DirectoryUri;
            Assert.NotNull(directoryUri);
            Assert.Equal(directoryUriString, directoryUri.OriginalString);
        }

        [Fact]
        public void AcmeDirectoryWithDirectoryUriReturnsUriString()
        {
            const string directoryUriString = "https://example.org/";
            var directoryUri = new Uri(directoryUriString);
            var directory = new AcmeDirectory { DirectoryUri = directoryUri };

            Assert.Equal(directoryUriString, directory.DirectoryUriString);
        }

        [Fact]
        public void AcmeDirectoryDirectoryUriSameInstance()
        {
            const string directoryUriString = "https://example.org/";
            var directoryUri = new Uri(directoryUriString);
            var directory = new AcmeDirectory { DirectoryUri = directoryUri };

            Assert.Same(directoryUri, directory.DirectoryUri);
            Assert.Same(directoryUri, directory.DirectoryUri);
        }
    }
}
