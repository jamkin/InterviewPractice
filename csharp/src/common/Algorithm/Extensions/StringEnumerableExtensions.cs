namespace Algorithm.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class StringEnumerableExtensions
    {
        public static string Combine(this IEnumerable<string> strings)
        {
            if (strings is null)
            {
                throw new ArgumentNullException(nameof(strings));
            }

            return string.Join(string.Empty, strings);
        }
    }
}
