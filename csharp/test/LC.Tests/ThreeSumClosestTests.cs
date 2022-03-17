namespace LC.Tests
{
    using global::Tests.Common;
    using LC.Problems.Medium;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;

    /// <summary>
    /// Unit tests for <see cref="ThreeSumClosest"/>
    /// </summary>
    [TestClass]
    public class ThreeSumClosestTests
    {
        [Ignore("TODO: Implement")]
        [TestMethod]      
        [DataRow("[-1,2,1,-4]", int.MaxValue, 2)]
        [DataRow("[-1,2,1,-4]", 3, 2)]
        [DataRow("[-1,2,1,-4]", 2, 2)]
        [DataRow("[-1,2,1,-4]", 1, 2)]
        [DataRow("[-1,2,1,-4]", 0, -1)]
        [DataRow("[-1,2,1,-4]", -1, -1)]
        [DataRow("[-1,2,1,-4]", -2, -1)]
        [DataRow("[-1,2,1,-4]", -3, -3)]
        [DataRow("[-1,2,1,-4]", -4, -4)]
        [DataRow("[-1,2,1,-4]", -5, -4)]
        [DataRow("[-1,2,1,-4]", int.MinValue, -4)]
        [DataRow("[5,1,5]", 8283, 11)]
        [DataRow("[5,1,5]", 8, 11)]
        [DataRow("[5,1,5]", -39, 11)]
        [DataRow("[0,-2,4,5,8,2,0,-4,15,6,2,-5,4]", 10, 10)] // ex. 8,2,0
        [DataRow("[0,-2,4,5,8,2,0,-4,15,6,2,-5,4]", 9, 9)] // ex. -4,15,-2
        [DataRow("[0,-2,4,5,8,2,0,-4,15,6,2,-5,4]", 14, 14)] // ex. 6,4,4
        [DataRow("[0,-2,4,5,8,2,0,-4,15,6,2,-5,4]", 50, 29)] // ex. 8,15,6
        [DataRow("[0,-2,4,5,8,2,0,-4,15,6,2,-5,4]", -12, -11)] // ex. -2,-4,-5
        public void ThreeSumClosestReturnsSumOfThreeNumbersClosestToTargetTest(string numbers_str, int target, int expected)
        {
            // Arrange

            var numbers = TestInputStrings.ParseCommaList(numbers_str).Select(int.Parse).ToArray();
            var solution = new ThreeSumClosest();

            // Act

            var actual = solution.Solve(numbers, target);

            // Assert

            Assert.AreEqual(expected, actual);
        }
    }
}