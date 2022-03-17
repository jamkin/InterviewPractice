namespace Algorithm.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Methods for <see cref="IEnumerable{T}"/> which are not a fit
    /// for extension methods <see cref="EnumerableExtensions"/> because
    /// they are not natural on instances of a single enumeration.
    /// </summary>
    /// <typeparam name="T">The type of element in the enumeration</typeparam>
    public static class Enumerable<T>
    {
        /// <summary>
        /// Merge N number of sorted enumerations
        /// </summary>
        /// <example>
        /// [1, 2, 2, 4], [-1, 0, 0, 1, 2], [-1, 3, 5] -> [-1, -1, 0, 0, 1, 1, 2, 2, 2, 3, 4, 5]
        /// </example>
        /// <param name="sources">The individual sorted enumerations</param>
        /// <returns>
        /// Enumeration which amounts to a sorted union of the the enumerations in <paramref name="sources"/>
        /// </returns>
        /// <exception cref="ArgumentNullException/">
        public static IEnumerable<T> MergeSorted(params IEnumerable<T>[] sources)
        {
            if (sources is null)
            {
                throw new ArgumentNullException(nameof(sources));
            }

            IEnumerable<T> result = Enumerable.Empty<T>();
            foreach (var source in sources)
            {
                if (source is null)
                {
                    throw new ArgumentNullException(nameof(sources), "Sorted enumeration cannot be null.");
                }

                result = result.MergeSorted(source);
            }

            return result;
        }

        /// <summary>
        /// Create enumeration from parameters listed
        /// at compile time
        /// </summary>
        /// <param name="items">The parameters</param>
        /// <returns>Enumeration</returns>
        /// <exception cref="ArgumentNullException"/>
        public static IEnumerable<T> CreateFrom(params T[] items)
        {
            return items ?? throw new ArgumentNullException(nameof(items));
        }
    }
}
