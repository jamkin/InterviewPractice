namespace CTCI.Tests
{
    using CTCI.Problems.Chapters._1;
    using global::Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="StringQuestions"/>
    /// </summary>
    [TestClass]
    public class Chapter1StringQuestsionsTests
    {
        [DataTestMethod]
        [DataRow("", true)]
        [DataRow("A", true)]
        [DataRow("BA", true)]
        [DataRow("AB", true)]
        [DataRow("AA", false)]
        [DataRow("ABC", true)]
        [DataRow("ABA", false)]
        [DataRow("AECADB", false)]
        [DataRow("FECADB", true)]
        public void IsUniqueTest(string input, bool expected)
        {
            // Act

            var actual = StringQuestions.IsUnique(input);

            // Assert

            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow("", "", true)]
        [DataRow("A", "", false)]
        [DataRow("A", "A", true)]
        [DataRow("A,B", "A", false)]
        [DataRow("A,B", "A,B", true)]
        [DataRow("A,B", "B,A", true)]
        [DataRow("A,C", "B,A", false)]
        [DataRow("A,B,A", "B,A", false)]
        [DataRow("A,B,A", "B,A,A", true)]
        [DataRow("A,C,B", "A,B,C", true)]
        [DataRow("A,C,D", "A,B,C", false)]
        [DataRow("A,B,A,A,D,C,E,C,C,D", "E,D,A,A,B,C,A,C,C,D", true)]
        [DataRow("A,B,A,A,D,C,E,C,C,D", "A,D,A,A,B,B,E,C,C,D", false)]
        public void CheckPermutationTest(string first, string second, bool expected)
        {
            // Act

            var actual = StringQuestions.CheckPermutation(first, second);

            // Assert

            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow("A", "A")]
        [DataRow("AA", "AA")]
        [DataRow("AB", "AB")]
        [DataRow("ABB", "ABB")]
        [DataRow("ABA", "ABA")]
        [DataRow("AAB", "AAB")]
        [DataRow("AAA", "3A")]
        [DataRow("AABC", "AABC")]
        [DataRow("AABA", "AABA")]
        [DataRow("AAAB", "3AB")]
        [DataRow("ABEBCAE", "ABEBCAE")]
        [DataRow("ABDDDDAE", "AB4DAE")]
        [DataRow("ABAAACCCCCBABBCCCEABBBBBBBB", "AB3A5CBABB3CEA8B")]
        [DataRow("ABAAACCCCCBABBCCCEABBBBBBBBD", "AB3A5CBABB3CEA8BD")]
        [DataRow("ABAAACCCCCBABBCCCCCCCCCCCEABBBBBBBBD", "AB3A5CBABB11CEA8BD")]
        public void StringCompressionTest(string input, string expected)
        {
            // Act

            var actual = StringQuestions.StringCompression(input);

            // Assert

            Assert.AreEqual(expected, actual);
        }
    }
}