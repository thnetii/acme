using System.Runtime.Serialization;

namespace THNETII.Acme.Client
{
    /// <summary>
    /// A well-known ACME Error Type as defined by <a href="https://tools.ietf.org/id/draft-ietf-acme-acme-02.html#rfc.section.5.5">Section 5.5. Error</a> of the <a href="https://tools.ietf.org/id/draft-ietf-acme-acme-02.html">Automatic Certificate Management Environment (ACME) specification</a>.
    /// </summary>
    /// <remarks>
    /// Errors can be reported in ACME both at the HTTP layer and within ACME payloads. ACME servers can return responses with an HTTP error response code (4XX or 5XX). For example: If the client submits a request using a method not allowed in this enumeration, then the server MAY return status code 405 (Method Not Allowed).
    /// <para>To facilitate automatic response to errors, this enumeration defines standard error types for use in the &quot;type&quot; field (within the &quot;urn:ietf:params:acme:error:&quot; namespace).</para>
    /// <para>This error types specified in this enumeration are not exhaustive. The server MAY return errors whose &quot;type&quot; field is set to a type other than those defined here. Servers MUST NOT use the ACME URN namespace for errors other than the standard types. Clients SHOULD display the &quot;detail&quot; field of such errors.</para>
    /// </remarks>
    public enum AcmeErrorType
    {
        Unknown = 0,

        /// <summary>
        /// The CSR is unacceptable (e.g., due to a short key)
        /// </summary>
        [EnumMember(Value = "urn:acme:error:badCSR")]
        BadCsr,

        /// <summary>
        /// The client sent an unacceptable anti-replay nonce
        /// </summary>
        [EnumMember(Value = "urn:acme:error:badNonce")]
        BadNonce,

        /// <summary>
        /// The server could not connect to the client for validation
        /// </summary>
        [EnumMember(Value = "urn:acme:error:connection")]
        Connection,

        /// <summary>
        /// The server could not validate a DNSSEC signed domain
        /// </summary>
        [EnumMember(Value = "urn:acme:error:dnssec")]
        DnsSec,

        /// <summary>
        /// The request message was malformed
        /// </summary>
        [EnumMember(Value = "urn:acme:error:malformed")]
        Malformed,

        /// <summary>
        /// The server experienced an internal error
        /// </summary>
        [EnumMember(Value = "urn:acme:error:serverInternal")]
        ServerInternal,

        /// <summary>
        /// The server experienced a TLS error during validation
        /// </summary>
        [EnumMember(Value = "urn:acme:error:tls")]
        Tls,

        /// <summary>
        /// The client lacks sufficient authorization
        /// </summary>
        [EnumMember(Value = "urn:acme:error:unauthorized")]
        Unauthorized,

        /// <summary>
        /// The server could not resolve a domain name
        /// </summary>
        [EnumMember(Value = "urn:acme:error:unknownHost")]
        UnknownHost,

        /// <summary>
        /// The request exceeds a rate limit
        /// </summary>
        [EnumMember(Value = "urn:acme:error:rateLimited")]
        RateLimited,

        /// <summary>
        /// The provided contact URI for a registration was invalid
        /// </summary>
        [EnumMember(Value = "urn:acme:error:invalidContact")]
        InvalidContact
    }
}