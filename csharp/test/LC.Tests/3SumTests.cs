namespace LC.Tests
{
    using global::Tests.Common;
    using LC.Problems.Medium;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Unit tests for <see cref="_3Sum"/>
    /// </summary>
    [TestClass]
    public class _3SumTests
    {
        [Ignore("TODO: Implement")]
        [DataTestMethod]
        [DataRow("[-1,0,1,2,-1,-4]", "[[-1,-1,2],[-1,0,1]]")]
        [DataRow("[0,0,0]", "[[0,0,0]]")]
        [DataRow("[2,-1,-1]", "[[2,-1,-1]]")]
        [DataRow("[2,-3,-1]", "[]")]
        [DataRow("[2,-3,-1,0,1]", "[[-1,0,1]]")]
        [DataRow("[-2,-3,-1,5,1,-6]", "[[-2,-3,5],[-6,1,5]]")]
        [DataRow("[-2,-3,-1,5,1,-6,3,0]", "[[-2,-3,5],[-6,1,5],[-1,1,0]]")]
        [DataRow("[-4,-3,-2,-1,0,1,2,3,4]", "[[-1,0,1],[-2,0,2],[-3,0,3],[-4,0,4],[-3,1,2],[-1,-2,3]]")]
        public void _3SumReturnsAllListsOf3ElementsSummingToZeroTest(string nums_str, string expected_str)
        {
            // Arrange

            var nums = TestInputStrings.ParseCommaList(nums_str).Select(int.Parse).ToArray();
            var solution = new _3Sum(target: 0);

            // Act

            var actual = solution.Solve(nums);

            // Assert

            var expected = TestInputStrings.ParseCommaLists(expected_str).Select(list_str => list_str.Select(int.Parse));
            Assert.That.EnumerationsDeepEquivalent(expected, actual);
        }
    }
}
