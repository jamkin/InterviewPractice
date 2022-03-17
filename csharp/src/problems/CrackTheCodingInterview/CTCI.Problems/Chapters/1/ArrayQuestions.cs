namespace CTCI.Problems.Chapters._1
{
    using Algorithm.Extensions;
    using CTCI.Decorations;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Array questions from chapter 1
    /// </summary>
    public static class ArrayQuestions
    {
        [CtciProblem(
            Name = "Rotate Matrix",
            Chapter = 1, Problem = 7,
            Description =
        @"Given an image represented by an NxN matrix, where each pixel in the imave is 4 bytes, 
          write a method to rotate the image by 90 degrees. Can you do this in place?")]
        public static void RotateMatrix(int[,] mat)
        {
            if (mat is null)
            {
                throw new ArgumentNullException(nameof(mat));
            }

            mat.Rotate(1);
        }

        [CtciProblem(
            Name = "Zero Matrix",
            Chapter = 1, Problem = 8,
            Description =
        @"Write an algorithm such that if an element in an NxN matrix is 0, its entire row and
          column are set to 0.")]
        public static void ZeroMatrix(int[,] mat)
        {
            if (mat is null)
            {
                throw new ArgumentNullException(nameof(mat));
            }

            throw new NotImplementedException();
        }
    }
}
