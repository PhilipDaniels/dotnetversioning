using System;
using System.Collections.Generic;

namespace SetVersion.Lib
{
    /// <summary>
    /// A C# 2 implementation of some LINQ methods.
    /// </summary>
    public static class Linq
    {
        public delegate TResult Func<T, TResult>(T arg);

        /// <summary>
        /// A C# 2 implementation of <c>SingleOrDefault</c>.
        /// </summary>
        /// <typeparam name="T">Type of things in the sequence.</typeparam>
        /// <param name="seq">The sequence.</param>
        /// <param name="predicate">The predicate - finds at most one element satisfying the predicate.</param>
        /// <returns>The only satisfying element, or the corresponding default for T.</returns>
        /// <exception cref="System.InvalidOperationException">More than one matching element found in sequence.</exception>
        public static T SingleOrDefault<T>(IEnumerable<T> seq, Func<T, bool> predicate)
        {
            Val.ThrowIfNull(seq, nameof(seq));
            Val.ThrowIfNull(predicate, nameof(predicate));

            T result = default(T);

            int count = 0;
            foreach (T elem in seq)
            {
                if (predicate(elem))
                {
                    if (count > 0)
                        throw new InvalidOperationException("More than one matching element found in sequence.");
                    result = elem;
                    count++;
                }
            }

            return result;
        }
    }
}
