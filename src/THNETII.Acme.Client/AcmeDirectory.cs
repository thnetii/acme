using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using THNETII.Common;

using static THNETII.Acme.Client.UriSafeConverter;

namespace THNETII.Acme.Client
{
    [DataContract]
    public class AcmeDirectory
    {
        private DuplexConversionTuple<string, Uri> keyChange;
        private DuplexConversionTuple<string, Uri> newAuthz;
        private DuplexConversionTuple<string, Uri> newCert;
        private DuplexConversionTuple<string, Uri> newReg;
        private DuplexConversionTuple<string, Uri> revokeCert;

        [DataMember(Name = "key-change")]
        public string KeyChangeUriString
        {
            get => keyChange.RawValue;
            set => keyChange.RawValue = value;
        }

        [DataMember(Name = "new-authz")]
        public string NewAuthzUriString
        {
            get => newAuthz.RawValue;
            set => newAuthz.RawValue = value;
        }

        [DataMember(Name = "new-cert")]
        public string NewCertUriString
        {
            get => newCert.RawValue;
            set => newCert.RawValue = value;
        }

        [DataMember(Name = "new-reg")]
        public string NewRegistrationUriString
        {
            get => newReg.RawValue;
            set => newReg.RawValue = value;
        }

        [DataMember(Name = "revoke-cert")]
        public string RevokeCertUriString
        {
            get => revokeCert.RawValue;
            set => revokeCert.RawValue = value;
        }

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
