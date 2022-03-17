namespace Algorithm.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="char"/>
    /// </summary>
    internal static class CharacterExtensions
    {
        /// <summary>
        /// Check whether character represents an english letter [a-Z]
        /// </summary>
        /// <param name="c">The character</param>
        /// <returns>
        /// <see cref="true"/> if is English letter, <see cref="false"/> if not
        /// </returns>
        internal static bool IsEnglishLetter(this char c)
        {
            return c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z';
        }
    }
}
