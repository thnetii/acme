using System;

namespace THNETII.Acme.Client
{
    internal static class StringArgumentExceptionExtensions
    {
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        internal static void ThrowIfNullOrWhiteSpace(this string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw value == null ? new ArgumentNullException(nameof(name)) : new ArgumentException("value must neither be empty, nor null, nor whitespace-only.", name);
        }
    }
}
