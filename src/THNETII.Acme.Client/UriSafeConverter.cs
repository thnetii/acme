using System;

namespace THNETII.Acme.Client
{
    internal static class UriSafeConverter
    {
        internal static Uri StringToUriSafe(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            try { return new Uri(value); }
            catch (UriFormatException) { return null; }
        }

        internal static string UriToStringSafe(Uri value) => value?.ToString();
    }
}
