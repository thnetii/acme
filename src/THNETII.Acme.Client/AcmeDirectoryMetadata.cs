using System;
using System.Runtime.Serialization;
using THNETII.Common;
using static THNETII.Acme.Client.UriSafeConverter;

namespace THNETII.Acme.Client
{
    public class AcmeDirectoryMetadata
    {
        private readonly DuplexConversionTuple<string, Uri> termsofservice =
            new DuplexConversionTuple<string, Uri>(StringToUriSafe, UriToStringSafe);

        [DataMember(Name = "terms-of-service", EmitDefaultValue = false)]
        public string TermOrServiceUriString
        {
            get => termsofservice.RawValue;
            set => termsofservice.RawValue = value;
        }

        [IgnoreDataMember]
        public Uri TermsOfServiceUri
        {
            get => termsofservice.ConvertedValue;
            set => termsofservice.ConvertedValue = value;
        }
    }
}