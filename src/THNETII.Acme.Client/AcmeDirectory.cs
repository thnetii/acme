using System;
using System.Runtime.Serialization;
using THNETII.Common;

using static THNETII.Acme.Client.UriSafeConverter;

namespace THNETII.Acme.Client
{
    [DataContract]
    public class AcmeDirectory
    {
        private readonly DuplexConversionTuple<string, Uri> keyChange;
        private readonly DuplexConversionTuple<string, Uri> newAuthz;
        private readonly DuplexConversionTuple<string, Uri> newCert;
        private readonly DuplexConversionTuple<string, Uri> newReg;
        private readonly DuplexConversionTuple<string, Uri> revokeCert;

        [DataMember(Name = "key-change")]
        public string KeyChangeUriString
        {
            get => keyChange.RawValue;
            set => keyChange.RawValue = value;
        }

        [IgnoreDataMember]
        public Uri KeyChangeUri
        {
            get => keyChange.ConvertedValue;
            set => keyChange.ConvertedValue = value;
        }

        [DataMember(Name = "new-authz")]
        public string NewAuthzUriString
        {
            get => newAuthz.RawValue;
            set => newAuthz.RawValue = value;
        }

        [IgnoreDataMember]
        public Uri NewAuthzUri
        {
            get => newAuthz.ConvertedValue;
            set => newAuthz.ConvertedValue = value;
        }

        [DataMember(Name = "new-cert")]
        public string NewCertUriString
        {
            get => newCert.RawValue;
            set => newCert.RawValue = value;
        }

        [IgnoreDataMember]
        public Uri NewCertUri
        {
            get => newCert.ConvertedValue;
            set => newCert.ConvertedValue = value;
        }

        [DataMember(Name = "new-reg")]
        public string NewRegistrationUriString
        {
            get => newReg.RawValue;
            set => newReg.RawValue = value;
        }

        [IgnoreDataMember]
        public Uri NewRegistrationUri
        {
            get => newReg.ConvertedValue;
            set => newReg.ConvertedValue = value;
        }

        [DataMember(Name = "revoke-cert")]
        public string RevokeCertUriString
        {
            get => revokeCert.RawValue;
            set => revokeCert.RawValue = value;
        }

        [IgnoreDataMember]
        public Uri RevokeCertUri
        {
            get => revokeCert.ConvertedValue;
            set => revokeCert.ConvertedValue = value;
        }

        [DataMember(Name = "meta", IsRequired = false, EmitDefaultValue = false)]
        public AcmeDirectoryMetadata Metadata { get; set; }

        [IgnoreDataMember]
        public string TermOrServiceUriString => Metadata?.TermOrServiceUriString;

        [IgnoreDataMember]
        public Uri TermsOfServiceUri => Metadata?.TermsOfServiceUri;

        public AcmeDirectory()
        {
            keyChange = new DuplexConversionTuple<string, Uri>(StringToUriSafe, UriToStringSafe);
            newAuthz = new DuplexConversionTuple<string, Uri>(StringToUriSafe, UriToStringSafe);
            newCert = new DuplexConversionTuple<string, Uri>(StringToUriSafe, UriToStringSafe);
            newReg = new DuplexConversionTuple<string, Uri>(StringToUriSafe, UriToStringSafe);
            revokeCert = new DuplexConversionTuple<string, Uri>(StringToUriSafe, UriToStringSafe);
        }
    }
}
