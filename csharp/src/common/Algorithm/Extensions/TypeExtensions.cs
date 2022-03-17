namespace Algorithm.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="Type"/>
    /// </summary>
    internal static class TypeExtensions
    {
        /// <summary>
        /// Check whether the type is nullable, i.e.
        /// whether it can take on value <see cref="null"/>
        /// </summary>
        /// <param name="type">The check to check for nullability</param>
        /// <returns>
        /// <see cref="true"/> is nullable, <see cref="false"/> if not
        /// </returns>
        /// <exception cref="ArgumentNullException/">
        internal static bool IsNullable(this Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return Nullable.GetUnderlyingType(type) != null;
        }
    }
}
