namespace Algorithm.Tests
{
    using Algorithm.Extensions;
    using global::Tests.Common;
    using Mathematics.Abstractions.GroupTheory;
    using Mathematics.IntegralTypes.GroupTheory;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Unit tests for <see cref="EnumerableExtensions"/>
    /// </summary>
    [TestClass]
    public class EnumerableExtensionsTests
    {
        [DataTestMethod]
        [DataRow("[A]", 1, "[[A]]")]
        [DataRow("[A]", 2, "[[A]]")]
        [DataRow("[A]", 3, "[A]")]
        [DataRow("[A,B]", 1, "[[A],[B]]")]
        [DataRow("[A,B]", 2, "[[A,B]]")]
        [DataRow("[A,B]", 3, "[[A,B]]")]
        [DataRow("[A,B,C]", 1, "[[A],[B],[C]]")]
        [DataRow("[A,B,C]", 2, "[[A,B],[C]]")]
        [DataRow("[A,B,C]", 3, "[[A,B,C]]")]
        [DataRow("[A,B,C,D,E,F,G]", 3, "[[A,B,C],[D,E,F],[G]]")]
        public void EnumereableExtensionsChunkByTests(string source_str, int chunkSize, string expectedChunks_str)
        {
            // Arrange

            var source = TestInputStrings.ParseCommaList(source_str);

            // Act

            var chunks = source.ChunkBy(chunkSize);

            // Assert

            var expectedChunks = TestInputStrings.ParseCommaLists(expectedChunks_str);
            Assert.That.EnumerationsDeepEqual(expectedChunks, chunks);
        }

        [DataTestMethod]
        [DataRow("[]", "[]")]
        [DataRow("[1]", "[[1]]")]
        [DataRow("[1,2]", "[[1]]")]
        [DataRow("[1,2,1]", "[[1],[1]]")]
        [DataRow("[2,1]", "[[1]]")]
        [DataRow("[1,2,2,1]", "[[1],[1]]")]
        [DataRow("[1,2,3,4,5]", "[[1],[3],[5]]")]
        [DataRow("[1,2,3,1,3,2,6,2,5,4]", "[[1],[3,1,3],[5]]")]
        public void EnumerableExtensionsSplitByPredicateTests(string source_str, string expected_str)
        {
            // Arrange

            var source = TestInputStrings.ParseCommaList(source_str);

            // Act

            var splits = source.SplitBy((string str) => int.Parse(str) % 2 == 0);

            // Assert

            var expectedSplits = TestInputStrings.ParseCommaLists(expected_str);
            Assert.That.EnumerationsDeepEqual(expectedSplits, splits);
        }

        [DataTestMethod]
        [DataRow("[A]", 0, "[]")]
        [DataRow("[A,B]", 0, "[B]")]
        [DataRow("[A,B]", 1, "[A]")]
        [DataRow("[A,B,C]", 0, "[B,C]")]
        [DataRow("[A,B,C]", 1, "[A,C]")]
        [DataRow("[A,B,C]", 2, "[A,B]")]
        [DataRow("[A,B,C,D,E", 3, "[A,B,C,E]")]
        public void EnumerableExtensionsSkipAtTest(string source_str, int index, string expected_str)
        {
            // Arrange

            var source = TestInputStrings.ParseCommaList(source_str);

            // Act

            var actual = source.SkipAt(index);

            // Assert

            var expected = TestInputStrings.ParseCommaList(expected_str);
            CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
        }

        [DataTestMethod]
        [DataRow("[]", "[]")]
        [DataRow("[A]", "[[A]]")]
        [DataRow("[A,B]", "[[A,B],[B,A]]")]
        [DataRow("[A,B,C]", "[[A,B,C],[A,C,B],[B,A,C],[B,C,A],[C,A,B],[C,B,A]]")]
        public void EnumerableExtensionsPermutationsTest(string source_str, string expected_str)
        {
            // Arrange

            var source = TestInputStrings.ParseCommaList(source_str);

            // Act

            var permutations = source.Permutations();

            // Assert

            var expected = TestInputStrings.ParseCommaLists(expected_str);
            Assert.That.EnumerationsDeepEqual(expected, permutations);
        }

        [DataTestMethod]
        [DataRow("[]", "[]", true)]
        [DataRow("[A]", "[]", false)]
        [DataRow("[]", "[A]", false)]
        [DataRow("[A]", "[A]", true)]
        [DataRow("[A,B]", "[A]", false)]
        [DataRow("[A]", "[A,B]", false)]
        [DataRow("[A,B]", "[A,B]", true)]
        [DataRow("[A,B]", "[B,A]", true)]
        [DataRow("[A,B]", "[A,C]", false)]
        [DataRow("[A,A]", "[A,A]", true)]
        [DataRow("[A,A]", "[A,A,A]", false)]
        [DataRow("[A,B,B]", "[A,B,B]", true)]
        [DataRow("[A,B,B]", "[B,A,B]", true)]
        [DataRow("[A,B,B,C,D,D,E]", "[B,A,C,D,B,E,D]", true)]
        [DataRow("[A,B,B,C,D,D,E]", "[B,A,A,D,B,E,D]", false)]
        [DataRow("[A,B,B,C,D,D,E]", "[B,A,C,D,E,D]", false)]
        public void EnumerableExtensionsIsPermutationOfTest(string first_str, string second_str, bool expected)
        {
            // Arrange

            var first = TestInputStrings.ParseCommaList(first_str);
            var second = TestInputStrings.ParseCommaList(second_str);

            // Act

            bool actual = first.IsPermutationOf(second);

            // Assert

            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow("[]", "[]")]
        [DataRow("[1]", "[]")]
        [DataRow("[1,2]", "[[1,2]]")]
        [DataRow("[1,1]", "[]")]
        [DataRow("[2,1]", "[]")]
        [DataRow("[1,1,2]", "[[1,2]]")]
        [DataRow("[1,2,2]", "[[1,2]]")]
        [DataRow("[2,1,2]", "[[1,2]]")]
        [DataRow("[1,2,3,1,2]", "[[1,2,3],[1,2]]")]
        [DataRow("[1,2,3,1,2,1]", "[[1,2,3],[1,2]]")]
        [DataRow("[1,2,3,2,1,2,1]", "[[1,2,3],[1,2]]")]
        public void EnumerableExtensionsIncreasingSubenumerations(string source_str, string expected_str)
        {
            // Arrange

            var source = TestInputStrings.ParseCommaList(source_str);

            // Act

            var increasings = source.IncreasingSubenumerations();

            // Assert

            var expected = TestInputStrings.ParseCommaLists(expected_str);
            Assert.That.EnumerationsDeepEqual(expected, increasings);
        }

        [DataTestMethod]
        [DataRow("[]", "[]", "[]", "[]")]
        [DataRow("[]", "[A]", "[]", "[]")]
        [DataRow("[A]", "[A]", "[]", "[]")]
        [DataRow("[A]", "[B]", "[]", "[A]")]
        [DataRow("[A]", "[A]", "[B]", "[B]")]
        [DataRow("[A,B]", "[A]", "[B]", "[B,B]")]
        [DataRow("[A,B]", "[C]", "[B]", "[A,B]")]
        [DataRow("[A,B]", "[A]", "[]", "[B]")]
        [DataRow("[A,B]", "[B]", "[C]", "[A,C]")]
        [DataRow("[A,B,A,C,A,B,C,C,A]", "[C,A]", "[D]", "[A,B,A,D,B,C,D]")]
        [DataRow("[A,B,A,C,A,B,C,C,A]", "[A,B,A,C,A,B,C,C,A]", "[X]", "[X]")]
        [DataRow("[A,B,A,C,A,B,C,C,A]", "[A,B,A,C,A,B,C,C]", "[X]", "[X,A]")]
        // TODO: More tests
        public void EnumerableExtensionsFindAndReplaceTest(string source_str, string find_str, string replace_str, string expected_str)
        {
            // Arrange

            var source = TestInputStrings.ParseCommaList(source_str);
            var find = TestInputStrings.ParseCommaList(find_str);
            var replace = TestInputStrings.ParseCommaList(replace_str);

            // Act

            var actual = source.FindAndReplace(find, replace);

            // Assert

            var expected = TestInputStrings.ParseCommaList(expected_str);
            CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
        }

        [DataTestMethod]
        [DataRow("[]", 0, true)]
        [DataRow("[]", 1, false)]
        [DataRow("[]", 2, false)]
        [DataRow("[A]", 0, true)]
        [DataRow("[A]", 1, true)]
        [DataRow("[A]", 2, false)]
        [DataRow("[A]", 3, false)]
        [DataRow("[A,B]", 0, true)]
        [DataRow("[A,B]", 1, true)]
        [DataRow("[A,B]", 2, true)]
        [DataRow("[A,B]", 3, false)]
        [DataRow("[A,B,C,E,F,G,H]", 6, true)]
        [DataRow("[A,B,C,E,F,G,H]", 7, true)]
        [DataRow("[A,B,C,E,F,G,H]", 8, false)]
        public void EnumerableExtensionsHasAtLeastTest(string source_str, int k, bool expected)
        {
            // Arrange

            var source = TestInputStrings.ParseCommaList(source_str);

            // Act

            var actual = source.HasAtLeast(k);

            // Assert

            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow("[]", "[]", "[]")]
        [DataRow("[1]", "[]", "[1]")]
        [DataRow("[]", "[1]", "[1]")]
        [DataRow("[1,2]", "[]", "[1,2]")]
        [DataRow("[]", "[1,2]", "[1,2]")]
        [DataRow("[1,2]", "[1]", "[1,1,2]")]
        [DataRow("[1]", "[1,2]", "[1,1,2]")]
        [DataRow("[3]", "[1,2]", "[1,2,3]")]
        [DataRow("[2]", "[1,3]", "[1,2,3]")]
        [DataRow("[2]", "[1,2]", "[1,2,2]")]
        [DataRow("[1,2,2,5]", "[0,2,3,6,10]", "[0,1,2,2,2,3,5,6,10]")]
        public void EnumerableExtensionsMergeSortedTest(string first_str, string second_str, string expected_str)
        {
            // Arrange

            var first = TestInputStrings.ParseCommaList(first_str);
            var second = TestInputStrings.ParseCommaList(second_str);

            // Act

            var merged = first.MergeSorted(second);

            // Assert

            var expected = TestInputStrings.ParseCommaList(expected_str);
            CollectionAssert.AreEqual(expected.ToList(), merged.ToList());
        }

        [DataTestMethod]
        [DataRow("[]", null)]
        [DataRow("[A]", "A")]
        [DataRow("[A,B]", null)]
        [DataRow("[A,B,A]", "A")]
        [DataRow("[A,B,B]", "B")]
        [DataRow("[A,A,B]", "A")]
        [DataRow("[A,A,A]", "A")]
        [DataRow("[A,B,B,B]", "B")]
        [DataRow("[A,B,A,B]", null)]
        [DataRow("[A,B,B,A]", null)]
        [DataRow("[A,B,A,A]", "A")]
        [DataRow("[A,B,A,A,B]", "A")]
        [DataRow("[A,A,B,B,A]", "A")]
        [DataRow("[A,C,A,B,B,C,D,A,A,B]", null)]
        [DataRow("[A,C,A,B,A,C,D,A,A,B]", null)]
        [DataRow("[A,C,A,B,A,C,A,A,D,A]", "A")]
        public void EnumerableExtensionsMajorityCandidateOrNullTest(string source_str, string expected)
        {
            // Arrange

            var source = TestInputStrings.ParseCommaList(source_str);

            // Act

            var actual = source.MajorityElementOrNull();

            // Assert

            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow("[]", "[]")]
        [DataRow("[A]", "[{A:1}]")]
        [DataRow("[A,A]", "[{A:2}]")]
        [DataRow("[A,A,A]", "[{A:3}]")]
        [DataRow("[A,A,B]", "[{A:2},{B:1}]")]
        [DataRow("[A,B,A]", "[{A:1},{B:1},{A:1}]")]
        [DataRow("[A,B,A,A,C]", "[{A:1},{B:1},{A:2},{C:1}]")]
        [DataRow("[A,B,B,B,A,A,C,B,B]", "[{A:1},{B:3},{A:2},{C:1},{B:2}]")]
        public void EnumerableExtensionsAdjacentCountsTest(string source_str, string expected_str)
        {
            // Arrange

            var expected = TestInputStrings.ParseCommaList(expected_str)
                .Select(kvp_str => TestInputStrings.ParseKeyValuePair(kvp_str))
                .Select(kvp => new KeyValuePair<char, int>(kvp.Key.Single(), int.Parse(kvp.Value)))
                .ToList();

            // Act

            var actual = source_str.AdjacentCounts().ToList();

            // Asset

            CollectionAssert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow("[]","[]", true)]
        [DataRow("[A]","[A]", true)]
        [DataRow("[A,B]", "[A]", false)]
        [DataRow("[A,B]", "[A,C]", false)]
        [DataRow("[A,B]", "[B,A]", true)]
        [DataRow("[A,B]", "[A,A]", false)]
        [DataRow("[A,B,C,D]", "[A,B,C,D]", true)]
        [DataRow("[A,B,C,D]", "[D,A,B,C]", true)]
        [DataRow("[A,B,C,D]", "[C,D,A,B]", true)]
        [DataRow("[A,B,C,D]", "[B,C,D,A]", true)]
        [DataRow("[A,B,C,D]", "[B,C,D,E,A]", false)]
        [DataRow("[A,B,C,D]", "[B,E,D,A]", false)]
        [DataRow("[A,B,A,A]", "[A,A,A,B]", true)]
        [DataRow("[A,B,A,C]", "[A,A,C,B]", false)]
        [DataRow("[A,B,A,C]", "[C,A,B,A]", true)]
        [DataRow("[A,B,A,C]", "[A,C,A,B]", true)]
        public void EnumerableExtensionsIsRotationOfTest(string first_str, string second_str, bool expected)
        {
            // Arrange

            var first = TestInputStrings.ParseCommaList(first_str);
            var second = TestInputStrings.ParseCommaList(second_str);

            // Act

            var actual = first.IsRotationOf(second);

            // Assert

            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow("[]", "[]")]
        [DataRow("[A]", "[]")]
        [DataRow("[A,B]", "[[0,1]]")]
        [DataRow("[A,B,C]", "[[0,1],[0,2],[1,2]]")]
        [DataRow("[A,B,C,D]", "[[0,1],[0,2],[0,3],[1,2],[1,3],[2,3]]")]
        [DataRow("[A,B,C,D,E]", "[[0,1],[0,2],[0,3],[0,4],[1,2],[1,3],[1,4],[2,3],[2,4],[3,4]]")]
        public void EnumerableExtensionsSelectUniquePairsIndicesTest(string source_str, string expected_str)
        {
            // Arrange

            var source = TestInputStrings.ParseCommaList(source_str);

            // Act

            var actual = source
                .SelectUniquePairs((val, index) => index)
                .Select(t => new int[] { t.Item1, t.Item2 })
                .ToList();

            // Assert

            var expected = TestInputStrings.ParseCommaLists(expected_str)
                .Select(strs => strs.Select(int.Parse))
                .ToList();
            Assert.That.EnumerationsDeepEquivalent(expected, actual);
        }

        [DataTestMethod]
        [DataRow("[X,Y,X,A,B,X,C,X]", "[A,B,X,C]", "X", "Y")]
        [DataRow("[A,B,X,C,X]", "[A,B,X,C]", "X", "Y")]
        [DataRow("[X,Y,X,A,B,X,C,X]", "[Y,A,B,X,C]", "X")]
        [DataRow("[X,Y,X,A,B,X,C,X]", "[X,Y,X,A,B,X,C,X]", "Y")]
        // TODO: More tests
        public void EnumerableExtensionsTrimTest(string source_str, string expected_str, params string[] items)
        {
            // Arrange

            var source = TestInputStrings.ParseCommaList(source_str);

            // Act

            var actual = source.Trim(items).ToList();

            // Assert

            var expected = TestInputStrings.ParseCommaList(expected_str).ToList();
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}