namespace Tests.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for multi-dimensional arrays, such as <see cref="int[,]"/>
    /// </summary>
    internal static class MultiDimensionalArrayExtensions
    {
        /// <summary>
        /// Convert array or arrays to matrix. The array must not be jagged.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array</typeparam>
        /// <param name="arr">The array of arrays</param>
        /// <returns>
        /// <paramref name="arr"/> as a matrix
        /// </returns>
        /// <exception cref="ArgumentException">If not square matrix</exception>
        internal static T[,]? ToMatrix<T>(this T[][]? arr)
        {
            if (arr is null)
            {
                return null;
            }

            var n = arr.Length;

            if (n == 0)
            {
                return new T[0, 0];
            }

            var m = arr[0].Length;
            if (arr.Skip(1).Any(a => a.Length != m))
            {
                throw new ArgumentException(
                    nameof(arr),
                    "Jagged array cannot convert to multi-dimensional array because it does not have inner arrays all of the same length.");
            }

            T[,] mat = new T[n, m];
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < m; j++)
                    mat[i, j] = arr[i][j];
            return mat;
        }

        /// <summary>
        /// Convert all elements in a matrix
        /// </summary>
        /// <typeparam name="T">The type of elements in the matrix</typeparam>
        /// <typeparam name="U">The type of elements to convert to</typeparam>
        /// <param name="mat">The matrix</param>
        /// <param name="convert">The conversion functions</param>
        /// <returns>
        /// New matrix with elements of type <typeparamref name="U"/>
        /// </returns>
        /// <exception cref="ArgumentNullException">for <paramref name="mat"/></exception>
        internal static U[,]? Convert<T, U>(this T[,]? mat, Func<T, U> convert)
        {
            if (mat is null)
            {
                return null;
            }

            if (convert is null)
            {
                throw new ArgumentNullException(nameof(convert));
            }

            int n = mat.GetLength(0), m = mat.GetLength(1);
            var _mat = new U[n, m];
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < m; ++j)
                    _mat[i, j] = convert(mat[i, j]);
            return _mat;
        }
    }
}
