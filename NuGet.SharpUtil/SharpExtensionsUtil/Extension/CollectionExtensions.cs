using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpExtensionsUtil.Extension
{
    public static class CollectionExtensions
    {
        public static TV GetFirstMatchedValue<T, TV>(this IEnumerable<T> list, Func<T, bool> predicate, Func<T, TV> selector, TV defaultVal)
        {
            if (list == null) return defaultVal;
            foreach (var item in list)
            {
                var matched = predicate(item);
                if (matched) return selector(item);
            }
            return defaultVal;
        }

        public static List<T> TrimStartWhenFalse<T>(this List<T> list, Func<T, bool> predicate)
        {
            if (list == null || list.Count == 0) return list;
            while (list.Count > 0)
            {
                if (predicate(list[0])) return list;
                list.RemoveAt(0);
            }
            return list;
        }

        public static bool RemoveByPredicate<T>(this IList<T> list, Predicate<T> p, bool removeAll = false)
        {
            var result = false;
            for (var i = 0; i < list.Count; i++)
            {
                if (!p(list[i])) continue;
                list.RemoveAt(i);
                if (!removeAll) return true;
                result = true;
                i--;
            }
            return result;
        }

        #region DistinctBy
        public static IEnumerable<T> DistinctBy<T, TIdentity>(this IEnumerable<T> source, Func<T, TIdentity> identitySelector)
        {
            return source.Distinct(By(identitySelector));
        }

        public static IEqualityComparer<TSource> By<TSource, TIdentity>(Func<TSource, TIdentity> identitySelector)
        {
            return new DelegateComparer<TSource, TIdentity>(identitySelector);
        }

        private class DelegateComparer<T, TIdentity> : IEqualityComparer<T>
        {
            private readonly Func<T, TIdentity> identitySelector;

            public DelegateComparer(Func<T, TIdentity> identitySelector)
            {
                this.identitySelector = identitySelector;
            }

            public bool Equals(T x, T y)
            {
                return Equals(identitySelector(x), identitySelector(y));
            }

            public int GetHashCode(T obj)
            {
                return identitySelector(obj).GetHashCode();
            }
        }
        #endregion
    }
}