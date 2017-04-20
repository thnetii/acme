using System;
using System.Collections.Generic;

namespace THNETII.Acme.Client
{
    public class AcmeDirectory
    {
        private string directoryUriString;
        private Tuple<string, Uri> directoryUriCache;

        public string DirectoryUriString
        {
            get => directoryUriString;
            set => directoryUriString = value;
        }

        public Uri DirectoryUri
        {
            get => GetOrUpdateCachedValue(directoryUriString, ref directoryUriCache, StringToUriSafe);
            set
            {
                directoryUriCache = Tuple.Create(value?.ToString(), value);
                directoryUriString = directoryUriCache.Item1;
            }
        }

        private TOut GetOrUpdateCachedValue<TIn, TOut>(TIn value, ref Tuple<TIn, TOut> cache, Func<TIn, TOut> conversion, IEqualityComparer<TIn> comparer = null)
        {
            var localCache = cache;
            if (localCache == null || !(comparer != null ? comparer.Equals(value, cache.Item1) : ReferenceEquals(value, localCache.Item1)))
            {
                localCache = Tuple.Create(value, conversion != null ? conversion(value) : default(TOut));
                cache = localCache;
            }
            return localCache.Item2;
        }

        private Uri StringToUriSafe(string value)
        {
            try { return new Uri(value); }
            catch (UriFormatException) { return null; }
            catch (ArgumentNullException) { return null; }
        }
    }
}
