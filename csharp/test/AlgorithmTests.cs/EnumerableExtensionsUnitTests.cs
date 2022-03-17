namespace AlgorithmTests.cs
{
    using Algorithm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class EnumerableExtensionsUnitTests
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

            var source = ParseCommaList(source_str);

            // Act

            var chunks = source.ChunkBy(chunkSize);

            // Assert

            var expectedChunks = ParseCommaLists(expectedChunks_str);
            AssertEqualSubenumerations(expectedChunks, chunks);
        }

        private static IEnumerable<string> ParseCommaList(string commaList)
        {
            return commaList
                .Trim('[', ']')
                .Split(",", StringSplitOptions.RemoveEmptyEntries);
        }

        private static IEnumerable<IEnumerable<string>> ParseCommaLists(string commaLists)
        {
            return ParseCommaList(commaLists).Select(list => ParseCommaList(list));
        }

        private static void AssertEqualSubenumerations<T>(IEnumerable<IEnumerable<T>> expected, IEnumerable<IEnumerable<T>> actual)
        {
            Assert.AreEqual(expected.Count(), actual.Count());
            foreach (var x in expected.Zip(actual, (e, a) => new { e, a }))
            {
                var innerEx = x.e;
                var innerAct = x.a;
                CollectionAssert.AreEqual(innerEx.ToList(), innerAct.ToList());
            }
        }
    }
}