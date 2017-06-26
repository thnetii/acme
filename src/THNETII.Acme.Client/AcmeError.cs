using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using THNETII.Common;

namespace THNETII.Acme.Client
{
    [DataContract]
    public class AcmeError
    {
        private static readonly IDictionary<AcmeErrorType, string> errorTypeToStringDict = new Dictionary<AcmeErrorType, string>();
        private static readonly IDictionary<string, AcmeErrorType> errorStringToTypeDict = new Dictionary<string, AcmeErrorType>();

        static AcmeError()
        {
            foreach (var fi in typeof(AcmeErrorType).GetTypeInfo().DeclaredFields)
            {
                var enumMemberAttr = fi.GetCustomAttribute<EnumMemberAttribute>();
                if (string.IsNullOrWhiteSpace(enumMemberAttr?.Value))
                    continue;
                AcmeErrorType type;
                try { type = (AcmeErrorType)fi.GetValue(null); }
                catch (InvalidCastException) { continue; }
                errorTypeToStringDict[type] = enumMemberAttr.Value;
                errorStringToTypeDict[enumMemberAttr.Value] = type;
            }
        }

        private DuplexConversionTuple<string, AcmeErrorType> type = new DuplexConversionTuple<string, AcmeErrorType>(
            s =>
            {
                AcmeErrorType v;
                if (!errorStringToTypeDict.TryGetValue(s, out v))
                    v = AcmeErrorType.Unknown;
                return v;
            },
            v =>
            {
                string s;
                if (!errorTypeToStringDict.TryGetValue(v, out s))
                    s = null;
                return s;
            }
            );

        [DataMember(Name = "type")]
        public string TypeString
        {
            get => type.RawValue;
            set => type.RawValue = value;
        }

        [IgnoreDataMember]
        public AcmeErrorType Type
        {
            get => type.ConvertedValue;
            set => type.ConvertedValue = value;
        }

        [DataMember(Name = "detail")]
        public string Detail { get; set; }

        [DataMember(Name = "status")]
        public HttpStatusCode StatusCode { get; set; }
    }
}
