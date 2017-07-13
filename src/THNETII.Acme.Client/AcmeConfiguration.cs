using Microsoft.Extensions.Configuration;

namespace THNETII.Acme.Client
{
    public class AcmeConfiguration
    {
        private object p;

        public AcmeConfiguration(IConfiguration configuration)
        {
            configuration?.Bind(this);
        }

        public string DirectoryUrl { get; set; }
    }
}