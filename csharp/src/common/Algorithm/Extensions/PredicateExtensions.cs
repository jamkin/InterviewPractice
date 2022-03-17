namespace Algorithm.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="Predicate{T}"/>
    /// </summary>
    public static class PredicateExtensions
    {
        /// <summary>
        /// Negate a predice
        /// </summary>
        /// <typeparam name="T">The type of element the predicate acts on</typeparam>
        /// <param name="predicate">The predicate to negate</param>
        /// <returns>
        /// The negated product
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        public static Predicate<T> Negate<T>(this Predicate<T> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return (t) => !predicate(t);
        }
    }
}
