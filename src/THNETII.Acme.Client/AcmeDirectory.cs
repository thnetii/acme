using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using THNETII.Common;

using static THNETII.Acme.Client.UriSafeConverter;

namespace THNETII.Acme.Client
{
    [DataContract]
    public class AcmeDirectory
    {
        private readonly DuplexConversionTuple<string, Uri> keyChange =
            new DuplexConversionTuple<string, Uri>(StringToUriSafe, UriToStringSafe);
        private readonly DuplexConversionTuple<string, Uri> newAuthz =
            new DuplexConversionTuple<string, Uri>(StringToUriSafe, UriToStringSafe);
        private readonly DuplexConversionTuple<string, Uri> newCert =
            new DuplexConversionTuple<string, Uri>(StringToUriSafe, UriToStringSafe);
        private readonly DuplexConversionTuple<string, Uri> newReg =
            new DuplexConversionTuple<string, Uri>(StringToUriSafe, UriToStringSafe);
        private readonly DuplexConversionTuple<string, Uri> revokeCert =
            new DuplexConversionTuple<string, Uri>(StringToUriSafe, UriToStringSafe);

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
        [SuppressMessage("Design", "CA1056: Uri properties should not be strings", Justification = nameof(TermsOfServiceUri))]
        public string TermsOfServiceUriString => Metadata?.TermOrServiceUriString;

        [IgnoreDataMember]
        public Uri TermsOfServiceUri => Metadata?.TermsOfServiceUri;
    }
}
