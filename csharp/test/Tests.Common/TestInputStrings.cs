namespace Tests.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Utilities for test data, to convert to/from comma list of strings
    /// </summary>
    public static class TestInputStrings
    {
        /// <summary>
        /// Square brackets
        /// </summary>
        private static readonly Brackets SQUARE_BRACKETS = new Brackets { Left = '[', Right = ']' };

        /// <summary>
        /// Curely braces
        /// </summary>
        private static readonly Brackets CURLY_BRACES = new Brackets { Left = '{', Right = '}' };

        /// <summary>
        /// Comma character
        /// </summary>
        private const char COMMA = ',';

        /// <summary>
        /// Newline character
        /// </summary>
        private const char NEW_LINE = '\n';

        /// <summary>
        /// Space character
        /// </summary>
        private const char SPACE = ' ';

        /// <summary>
        /// Parse comma list
        /// </summary>
        /// <param name="commaList">The comma-separated list, can be wraped with square brackets like array notation</param>
        /// <returns>
        /// Enumeration of items in the list
        /// </returns>
        public static IEnumerable<string> ParseCommaList(string commaList)
        {
            // SLOPPY IMPL! Improve.

            return commaList
                .TrimStart(SQUARE_BRACKETS.Left)
                .TrimEnd(SQUARE_BRACKETS.Right)
                .Split(COMMA, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Parse comma list of lists.
        /// </summary>
        /// <param name="commaLists">The list of lists. Square brackets should be around inner lists.</param>
        /// <returns>
        /// Enumeration of enumeration of items in the list
        /// </returns>
        public static IEnumerable<IEnumerable<string>> ParseCommaLists(string commaLists)
        {
            // e.g. "[[A,B],[C,D],[E,F]]"

            // SLOPPY IMPL! Improve.

            return commaLists
                .TrimStart(SQUARE_BRACKETS.Left)
                .TrimEnd(SQUARE_BRACKETS.Right)
                .Split($"{SQUARE_BRACKETS.Right}{COMMA}{SQUARE_BRACKETS.Left}", StringSplitOptions.RemoveEmptyEntries)
                .Select(str => ParseCommaList(str));
        }

        /// <summary>
        /// Parse key-value pairs
        /// </summary>
        /// <param name="keyVaulePair">Comma-separated items where each item is wraped in curly braces with colon separating key and value</param>
        /// <returns>
        /// The key-value pairs
        /// </returns>
        public static KeyValuePair<string, string> ParseKeyValuePair(string keyVaulePair)
        {
            // SLOPPY IMPL! Improve.

            var parts = keyVaulePair
                .TrimStart(CURLY_BRACES.Left)
                .TrimEnd(CURLY_BRACES.Right)
                .Split(COMMA, StringSplitOptions.RemoveEmptyEntries);
            return new KeyValuePair<string, string>(parts[0], parts[1]);
        }

        /// <summary>
        /// Parse 2-d matrix represented as spaces beetween columns
        /// and newlines between rows.
        /// </summary>
        /// <param name="lines">The lines (rows) each with spaces (columns)</param>
        /// <returns>
        /// The matrix
        /// </returns>
        public static string[,] Parse2DMatrix(string lines)
        {
            // SLOPPY IMPL! Improve.

            return lines
                .Split(NEW_LINE, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line
                    .Split(SPACE, StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => e.Trim())
                    .ToArray())
                .ToArray()
                .ToMatrix();
        }

        /// <summary>
        /// Parse 2-d matrix represented as spaces beetween columns
        /// and newlines between rows.
        /// </summary>
        /// <param name="lines">The lines (rows) each with spaces (columns)</param>
        /// <param name="parser">How to parse each element in the matrix</param>
        /// <returns>
        /// The matrix
        /// </returns>
        public static T[,] Parse2DMatrix<T>(string lines, Func<string, T> parser)
        {
            return Parse2DMatrix(lines).Convert(parser);
        }

        /// <summary>
        /// Represets left and right brackets of some sort
        /// </summary>
        private readonly struct Brackets
        {
            /// <summary>
            /// The left bracket character
            /// </summary>
            public char Left { get; init; }

            /// <summary>
            /// The right bracket character
            /// </summary>
            public char Right { get; init; }
        }
    }
}
