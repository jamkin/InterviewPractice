namespace Tests.Common
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections;

    /// <summary>
    /// Extension methods for <see cref="Assert"/>
    /// </summary>
    public static class AssertExtensions
    {
        /// <summary>
        /// Assert that 2 enumerations are sequence-equal, checking
        /// for sequence-equality of any inner sequences as well
        /// </summary>
        /// <example>
        /// [[A,B],[D,A],[C]], [[A,B],[D,A],[C]] => true
        /// [[A,B],[D,B],[C]], [[A,B],[D,A], C]] => false
        /// [[A,B],[D,A],[C]], [[A,B],[C]] => false
        /// </example>
        /// <param name="assert">The <see cref="Assert"/></param>
        /// <param name="expected">The exected enumeration</param>
        /// <param name="actual">The actual enumeration</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="AssertFailedException">If enumerations are not (deep-)equal</exception>
        public static void EnumerationsDeepEqual(this Assert assert, IEnumerable expected, IEnumerable actual)
        {
            if (assert is null)
            {
                throw new ArgumentNullException(nameof(assert));
            }

            if (expected is null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            if (actual is null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            if (!object.ReferenceEquals(expected, actual) && !EnumerationsDeepEqual(expected, actual))
            {
                Assert.Fail($"Enumerations not equal. Expected: {expected.RecursiveStringify()}. Actual: {actual.RecursiveStringify()}.");
            }
        }

        /// <summary>
        /// Assert that enumerations are deep-equivalent, meaning they have the same
        /// items not necessarily in the same order, and any items that are themselves
        /// enumerations would be checked for deep-equivalent.
        /// </summary>
        /// <example>
        /// [[A,B],[C,D]], [[D,C],[A,B]] are equivalent
        /// </example>
        /// <param name="assert">The <see cref="Assert"/></param>
        /// <param name="expected">The expected enumeration</param>
        /// <param name="actual">The actual enumeration</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="AssertFailedException">If enumerations are not (deep-)equivalent</exception>
        public static void EnumerationsDeepEquivalent(this Assert assert, IEnumerable expected, IEnumerable actual)
        {
            if (assert is null)
            {
                throw new ArgumentNullException(nameof(assert));
            }

            if (expected is null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            if (actual is null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            if (!object.ReferenceEquals(expected, actual) && !EnumerationsDeepEquivalent(expected, actual))
            {
                Assert.Fail($"Enumerations not equivalent. Expected: {expected.RecursiveStringify()}. Actual: {actual.RecursiveStringify()}.");
            }
        }

        /// <summary>
        /// Assert that two enumerations are equivalent, meaning they have the
        /// same elements in the same frequency; in other words, meaning one could
        /// be rearranged to be sequence-equal with the other if they are not already
        /// sequence-equal.
        /// </summary>
        /// <example>
        /// [A,A,B,C] and [B,A,C,A] are equivalent
        /// </example>
        /// <param name="assert">The <see cref="Asserts"/></param>
        /// <param name="expected">The expected enumeration</param>
        /// <param name="actual">The actual enumeration</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="AssertFailedException">If enumerations are not equivalent</exception>
        public static void EnumerationsEquivalent(this Assert assert, IEnumerable expected, IEnumerable actual)
        {
            if (assert is null)
            {
                throw new ArgumentNullException(nameof(assert));
            }

            if (expected is null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            if (actual is null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            if (!EnumerationsEquivalent(expected, actual))
            {
                Assert.Fail($"Enumerations not equivalent. Expected: {expected.RecursiveStringify()}. Actual: {actual.RecursiveStringify()}.");
            }
        }

        /// <summary>
        /// Assert that 2 martrices are equivalent, meaning equal values
        /// at every position.
        /// </summary>
        /// <typeparam name="T">The type of elements in the matrices</typeparam>
        /// <param name="assert">The <see cref="Assert"/></param>
        /// <param name="expected">The expected matrix</param>
        /// <param name="actual">The actual matrix</param>
        /// <exception cref="ArgumentNullException/">
        /// <exception cref="AssertFailedException">if not equal</exception>
        public static void MatricesEqual<T>(this Assert assert, T[,] expected, T[,] actual)
        {
            if (assert is null)
            {
                throw new ArgumentNullException(nameof(assert));
            }

            if (expected is null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            if (actual is null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            int n = expected.GetLength(0);
            Assert.AreEqual(n, actual.GetLength(0));

            int m = expected.GetLength(1);
            Assert.AreEqual(m, actual.GetLength(1));

            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    Assert.AreEqual(expected[i, j], actual[i, j]);
        }

        /// <summary>
        /// Check whether 2 enumerations are sequence-equal, checking
        /// for sequence-equality of any inner sequences as well.
        /// </summary>
        /// <param name="expected">The expected enumeration</param>
        /// <param name="actual">The actual enumeration</param>
        /// <returns>
        /// <see cref="true"/> if equal, <see cref="false"/> if not
        /// </returns>
        private static bool EnumerationsDeepEqual(IEnumerable expected, IEnumerable actual)
        {
            if (object.ReferenceEquals(expected, actual))
            {
                return true;
            }

            var mover_exepected = expected.GetEnumerator();
            var mover_actual = actual.GetEnumerator();
            for (; ; )
            {
                if (mover_exepected.MoveNext())
                {
                    if (mover_actual.MoveNext())
                    {
                        var current_expected = mover_exepected.Current;
                        var current_actual = mover_actual.Current;
                        if (current_expected is IEnumerable innerExpected && current_actual is IEnumerable innerActual)
                        {
                            return EnumerationsDeepEqual(innerExpected, innerActual);
                        }
                        else
                        {
                            if (!object.Equals(current_expected, current_actual))
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return !mover_actual.MoveNext();
                }
            }
        }

        /// <summary>
        /// Check whether enumerations are deep-equivalent, meaning they have the same
        /// items not necessarily in the same order, and any items that are themselves
        /// enumerations would be checked for deep-equivalent.
        /// </summary>
        /// <example>
        /// [[A,B],[C,D]], [[D,C],[A,B]] => true
        /// </example>
        /// <param name="expected">The expected enumeration</param>
        /// <param name="actual">The actual enumeration</param>
        /// <returns>
        /// <see cref="true"/> if equivalent, <see cref="false"/> if not
        /// </returns>
        private static bool EnumerationsDeepEquivalent(IEnumerable expected, IEnumerable actual)
        {
            if (object.ReferenceEquals(expected, actual))
            {
                return true;
            }

            var actualList = actual.ToGenericEnumeration().ToList();

            foreach (var expectedItem in expected)
            {
                if (expectedItem is IEnumerable expectedInner)
                {
                    var actualMatch = actual
                        .ToGenericEnumeration()
                        .FirstOrDefault(a => a is IEnumerable actualInner && EnumerationsDeepEqual(expectedInner, actualInner));
                    if (actualMatch == default)
                        return false;
                    else
                        actualList.Remove(actualMatch);
                }
            }

            return !actualList.Any();
        }

        /// <summary>
        /// Check whether two enumerations are equivalent, meaning that they have
        /// the same items but not necessarily in the same order
        /// </summary>
        /// <param name="expected">The expected enumeration</param>
        /// <param name="actual">The actual enumeration</param>
        /// <returns>
        /// <see cref="true"/> if equal, <see cref="false"/> if not equal
        /// </returns>
        private static bool EnumerationsEquivalent(IEnumerable expected, IEnumerable actual)
        {
            if (object.ReferenceEquals(expected, actual))
            {
                return true;
            }

            var actualCounts = actual
                .ToGenericEnumeration()
                .GroupBy(a => a)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var expectedItem in expected)
            {
                if (actualCounts.TryGetValue(expectedItem, out int count))
                {
                    if (count == 1)
                        actualCounts.Remove(expectedItem);
                    else
                        actualCounts[expectedItem] = --count;
                }
                else
                {
                    return false;
                }
            }

            return !actualCounts.Any();
        }

        /// <summary>
        /// Print an enumeration as a comma list, printing any
        /// enumeration as a comma list as well and putting brackets
        /// around andy inner and outer enumerations.
        /// </summary>
        /// <param name="source">The enumeration</param>
        /// <returns>
        /// The <see cref="string"/>
        /// </returns>
        private static string RecursiveStringify(this IEnumerable source)
        {
            var inner = source
                .ToGenericEnumeration()
                .Select(x => x is IEnumerable e ? e.RecursiveStringify() : x.ToString());
            return $"[{string.Join(",", inner)}]";
        }

        /// <summary>
        /// Convert <see cref="IEnumerable"/> to generic enumeration <see cref="IEnumerable{Object}"/>
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable"/></param>
        /// <returns>
        /// The <see cref="IEnumerable{Object}"/>
        /// </returns>
        private static IEnumerable<object> ToGenericEnumeration(this IEnumerable source)
        {
            for (var mover = source.GetEnumerator(); mover.MoveNext(); )
            {
                yield return mover.Current;
            }
        }
    }
}