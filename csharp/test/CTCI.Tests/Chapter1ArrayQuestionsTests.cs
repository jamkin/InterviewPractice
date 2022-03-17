namespace CTCI.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CTCI.Problems.Chapters._1;
    using global::Tests.Common;

    /// <summary>
    /// Unit tests for <see cref="ArrayQuestions"/>
    /// </summary>
    [TestClass]
    public class Chapter1ArrayQuestionsTests
    {
        [Ignore("TODO: Fix bug")]
        [DataTestMethod]
        [DataRow(
@"1",
@"1")]
        [DataRow(
@"1 2
  3 4",
@"3 1
  4 2")]
        [DataRow(
@"1 2 3
  4 5 6
  7 8 9",
@"7 4 1
  8 5 2
  9 6 3")]
        [DataRow(
@"1  2  3  4
  5  6  7  8
  9  10 11 12
  13 14 15 16",
@"13 9  5 1
  14 10 6 2
  15 11 7 3
  16 12 8 4")]
        [DataRow(
@"A B C D E
  F G H I J
  K L M N O
  P Q R S T
  U V W X Y",
@"U P K F A
  B Q L G B
  W R M H C
  C S N I D
  Y T O J E")]
        public void RotateMatrixTests(string input_str, string output_str)
        {
            // Arrange

            var mat = TestInputStrings.Parse2DMatrix(input_str, int.Parse);

            // Act

            ArrayQuestions.RotateMatrix(mat);

            // Assert

            var expected = TestInputStrings.Parse2DMatrix(output_str, int.Parse);
            Assert.That.MatricesEqual(expected, mat);
        }
    }
}
