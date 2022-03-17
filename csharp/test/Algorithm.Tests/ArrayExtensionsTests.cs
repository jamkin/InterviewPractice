namespace Algorithm.Tests
{
    using global::Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Algorithm.Extensions;

    /// <summary>
    /// Unit tests for <see cref="ArrayExtensions"/>
    /// </summary>
    [TestClass]
    public class ArrayExtensionsTests
    {
        [DataTestMethod]
        [DataRow("[]", 7, "[]")]
        [DataRow("[A]", 0, "[A]")]
        [DataRow("[A]", 1, "[A]")]
        [DataRow("[A]", 4, "[A]")]
        [DataRow("[A]", -274, "[A]")]
        [DataRow("[A,B]", 0, "[A,B]")]
        [DataRow("[A,B]", 1, "[B,A]")]
        [DataRow("[A,B]", 2, "[A,B]")]
        [DataRow("[B,A]", 3, "[A,B]")]
        [DataRow("[B,A]", -1, "[A,B]")]
        [DataRow("[B,A]", -2, "[B,A]")]
        [DataRow("[B,A]", -3, "[A,B]")]
        [DataRow("[A,B,C]", 0, "[A,B,C]")]
        [DataRow("[A,B,C]", 1, "[C,A,B]")]
        [DataRow("[A,B,C]", 2, "[B,C,A]")]
        [DataRow("[A,B,C]", 3, "[A,B,C]")]
        [DataRow("[A,B,C]", 4, "[C,A,B]")]
        [DataRow("[A,B,C]", -1, "[B,C,A]")]
        [DataRow("[A,B,C]", -2, "[C,A,B]")]
        [DataRow("[A,B,C]", -3, "[A,B,C]")]
        [DataRow("[A,B,C]", -5, "[C,A,B]")]
        [DataRow("[A,B,C]", 0, "[A,B,C]")]
        [DataRow("[A,B,C,D,E,F]", 2, "[E,F,A,B,C,D]")]
        [DataRow("[A,B,C,D,E,F]", 3, "[D,E,F,A,B,C]")]
        [DataRow("[A,B,C,D,E,F]", 4, "[C,D,E,F,A,B]")]
        [DataRow("[A,B,C,D,E,F]", 5, "[B,C,D,E,F,A]")]
        [DataRow("[A,B,C,D,E,F]", 14, "[E,F,A,B,C,D]")]
        [DataRow("[A,B,C,D,E,F]", -97, "[B,C,D,E,F,A]")]
        [DataRow("[A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z]", 5, "[V,W,X,Y,Z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U]")]
        public void ArrayExtensionsShiftTest(string input_str, int shifts, string expected_str)
        {
            // Arrange

            var input = TestInputStrings.ParseCommaList(input_str).ToArray();

            // Act 

            input.Shift(shifts);

            // Assert

            var expected = TestInputStrings.ParseCommaList(expected_str);
            CollectionAssert.AreEqual(expected.ToList(), input.ToList());
        }
    }
}
