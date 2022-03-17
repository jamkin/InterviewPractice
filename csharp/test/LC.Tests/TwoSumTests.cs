namespace LC.Tests
{
    using global::Tests.Common;
    using LC.Problems.Easy;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Unit tests for <see cref="TwoSum"/>
    /// </summary>
    [TestClass]
    public class TwoSumTests
    {
        [DataTestMethod]
        [DataRow("[1,1]", 2, "[0,1]")]
        [DataRow("[2,7,11,15]", 9, "[0,1]")]
        [DataRow("[-5,1,5,-3,6,1,0]", 3, "[3,4]")]
        [DataRow("[-5,1,5,-3,6,1,0]", 0, "[0,2]")]
        [DataRow("[-5,1,5,-3,6,1,0]",5, "[2,6]")]
        public void TwoSumReturnsSinglePairSummingToTargettedNumberTest(string source_str, int target, string expected_str)
        {
            // Arrange 

            var source = TestInputStrings.ParseCommaList(source_str).Select(int.Parse).ToArray();
            var solution = new TwoSum();

            // Act

            var actual = solution.Solve(source, target);

            // Assert

            var expected = TestInputStrings.ParseCommaList(expected_str).Select(int.Parse);
            Assert.That.EnumerationsEquivalent(expected, actual);
        }

        [DataTestMethod]
        [DataRow("[1,1]", 3)]
        [DataRow("[-5,1,5,-3,6,1,2,0]", 3)]
        [DataRow("[-5,1,5,-3,6,1,2,0]", -6)]
        public void TwoSumThrowsArgumentExceptionIfZeroOrMultipleSolutionsTest(string source_str, int target)
        {
            // Arrange 

            var source = TestInputStrings.ParseCommaList(source_str).Select(int.Parse).ToArray();
            var solution = new TwoSum();

            // Act/Assert

            Assert.ThrowsException<ArgumentException>(() =>
            {
                solution.Solve(source, target);
            });
        }
    }
}
