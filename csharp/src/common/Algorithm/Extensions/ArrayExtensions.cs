namespace Algorithm.Extensions
{
    using Algorithm.Decorations;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods on <see cref="Array"/>
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Partition array by predicate, so that elements that
        /// match the predicate appear after those that don't.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array</typeparam>
        /// <param name="arr">The array</param>
        /// <param name="predicate">The predicate that defines the partition</param>
        /// <exception cref="ArgumentNullException"/>
        public static void PartitionBy<T>(this T[] arr, Func<T, bool> predicate)
        {
            if (arr is null)
            {
                throw new ArgumentNullException(nameof(arr));
            }

            for (int i = 0, j = arr.Length - 1; i < j; )
            {
                if (predicate(arr[i])) ++i;
                else if (predicate(arr[j])) --j;
                else
                {
                    arr.Swap(i, j);
                    ++i; --j;
                }
            }
        }

        [AlgorithmImplementationStyle(Flavor = AlgorithmImplementationStyle.Parallelized)]
        [TimeComplexity(AverageCase = "O(n)", BestCase = "O(1)", WorstCase = "O(n)")]
        [SpaceComplexity(AverageCase = "O(1)", BestCase = "O(1)", WorstCase = "O(1)")]
        /// <summary>
        /// Rorate a square matrix a given number of times
        /// </summary>
        /// <typeparam name="T">The type of elements in the matrix</typeparam>
        /// <param name="matrix">The square (equal length dimensions) matrix</param>
        /// <param name="rotations">The number of rotations in the clockwise direction, can be negative for counter-clockwise</param>
        /// <exception cref="ArgumentNullException/">
        /// <exception cref="ArgumentException">If not square matrix</exception>
        public static void Rotate<T>(this T[,] matrix, int rotations)
        {
            if (matrix is null)
            {
                throw new ArgumentNullException(nameof(matrix));
            }

            int n = matrix.GetLength(0);
            int m = matrix.GetLength(1);
            if (n != m)
            {
                throw new ArgumentException(nameof(matrix), $"Only square matrix can be rotated clockwise in-place. Dimensions of matrix provided: {n}x{m}.");
            }

            // normalize to [0,3]
            NormalizeSquareRotations(ref rotations);

            // only really need [1,3]
            if (n < 1) 
            {
                return;
            }

            // TODO: Optimize by not doing multiple (1-3) rotations.
            //       Can have 3 different implementations that each move
            //       the elements straight to their desired position.
            //       E.g.
            //           +1 => RotateClockwise
            //           +2 => RotateCounterClockwise
            //           +3 => FlipUpsideDown
            for (int r = 0; r < rotations; ++r)
            {
                matrix.RotateClockwise();
            }        
        }

        [AlgorithmImplementationStyle(AlgorithmImplementationStyle.MicroOptimized)]
        [AlgorithmImplementationStyle(AlgorithmImplementationStyle.MemoryOptimized)]
        [TimeComplexity(AverageCase = "O(n)", BestCase = "O(1)", WorstCase = "O(n)")]
        [SpaceComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        /// <summary>
        /// Shift elements in the array, with wrap-around, a certain
        /// number of positions, with negative positions meaning to
        /// shift to the left/back and positive positions meaning to
        /// shift to the right/front. See examples for clarifications.
        /// times.
        /// </summary>
        /// <example>
        /// [A,B,C,D,E], 1 => [E,A,B,C,D]
        /// </example>
        /// <example>
        /// [A,B,C,D,E], 2 => [D,E,A,B,C]
        /// </example>
        /// <example>
        /// [A,B,C,D,E], 8 => [C,D,E,A,B]
        /// </example>
        /// <example>
        /// [A,B,C,D,E], -1 => [B,C,D,E,A]
        /// </example>
        /// <example>
        /// [A,B,C,D,E], -2 => [C,D,E,A,B]
        /// </example>
        /// <example>
        /// [A,B,C,D,E], -4 => [E,A,B,C,D]
        /// </example>
        /// <typeparam name="T">The type of elements in the array</typeparam>
        /// <param name="arr">The array to shift</param>
        /// <param name="shifts">The number of shifts</param>
        /// <exception cref="ArgumentNullException"/>
        public static void Shift<T>(this T[] arr, int shifts = 1)
        {
            if (arr is null)
            {
                throw new ArgumentNullException(nameof(arr));
            }

            int n = arr.Length;
            if (n <= 1)
                return;

            NormalizeShifts(shifts: ref shifts, arrLen: n);

            if (shifts == 0)
                return;

            // here, shifts is in [1, n) for n > 1

            /* Example: For array of length 5 and 2 shifts
             *
             * arr[i] -> arr[(i+2)%5]
             * 
             * (1) Replace arr[2] with arr[0]
             * (2) Replace arr[4] with previous arr[2]
             * (3) Replace arr[1] with previous arr[4]
             * (4) Replace arr[3] with previous arr[1]
             * (5) Replace arr[0] with previous arr[3]
             * 
             *     arr     | i | j | prev arr[j] | 
             * ===================================
             * [A,B,C,D,E] | 0 | 2 |    
             * -----------------------------------
             * [A,B,A,D,E] | 2 | 4 |     C       |
             * -----------------------------------
             * [A,B,A,D,C] | 4 | 1 |     E       |
             * -----------------------------------
             * [A,E,A,D,C] | 1 | 3 |     B       |
             * -----------------------------------
             * [A,E,A,B,C] | 3 | 0 |     D       |
             * -----------------------------------
             * [D,E,A,B,C] | 0 | 2 |     A       |
             * 
             * Note that if array was length 6 then the same algorithm
             * would loop infinitely:
             * 
             * i = 0 -> 2 -> 4 -> 0 -> 2 -> ..
             * 
             * Need to therefore do 2 loops:
             * 
             * 0 -> 2 -> 4 -> 0
             * 1 -> 3 -> 5 -> 1
             * 
             * And if shift was 2 then 3 loops:
             * 
             * 0 -> 3 -> 0
             * 1 -> 4 -> 1
             * 2 -> 5 -> 2
             * 
             * The breakdown of loops needed is like
             * 
             * n | s |          loops
             * ===================================
             * 2 | 1 | A->B->A
             * -----------------------------------
             * 3 | 1 | A->B->C->A
             * -----------------------------------
             * 3 | 2 | A->C->B->A
             * -----------------------------------
             * 4 | 1 | A->B->C->D->A
             * -----------------------------------
             * 4 | 2 | A->C->A, B->D->B
             * -----------------------------------
             * 4 | 3 | A->D->C->B->A
             * -----------------------------------
             * 5 | 1 | A->B->C->D->E->A
             * -----------------------------------
             * 5 | 2 | A->C->E->B->D->A
             * -----------------------------------
             * 5 | 3 | A->D->B->E->C->A
             * -----------------------------------
             * 5 | 4 | A->E->D->C->B->A
             * -----------------------------------
             * 6 | 1 | A->B->C->D->E->F->A
             * -----------------------------------
             * 6 | 2 | A->C->E->A, B->D->F->B
             * -----------------------------------
             * 6 | 3 | A->D->A, B->E->B, C->F->C
             * -----------------------------------
             * 6 | 4 | A->E->C->A, B->F->D->B
             * -----------------------------------
             * 6 | 5 | A->F->E->D->C->B->A
             * 
             * Using this, we could actually rewrite
             * the bottom to do each loop in parallel.
             */

            int start = 0;
            int i = start, j;
            T ti = arr[i], tj;
            int count = 0;
            for (bool done = false; !done; )
            {
                j = (i + shifts) % n;
                tj = arr[j];
                arr[j] = ti;
                ++count;
                ti = tj;
                i = j;
                if (i == start)
                {
                    if (count == arr.Length)
                        done = true;
                    else
                        ti = arr[start = ++i];
                }
            }
        }

        /// <summary>
        /// Rotate a square matrix clockwise 90 degrees
        /// </summary>
        /// <typeparam name="T">The type of elements in the matrix</typeparam>
        /// <param name="matrix">The matrix to rotate</param>
        private static void RotateClockwise<T>(this T[,] matrix)
        {
            int n = matrix.GetLength(0);
            Parallel.ForEach(Enumerable.Range(0, n / 2), (i) =>
            {
                /*   Example:
                 * 
                 *   A * * D       M * * A
                 *   * * * *   =>  * * * *
                 *   * * * *       * * * *
                 *   M * * P       P * * D
                 *   
                 *   A = matrix[0,0] => matrix[0,3]
                 *   D = matrix[0,3] => matrix[3,3]
                 *   P = matrix[3,3] => matrix[3,0]
                 *   M = matrix[3,0] => matrix[0,0]
                 *  
                 *   * B * *       * I * * 
                 *   * * * H   =>  * * * B
                 *   I * * *       O * * *
                 *   * * O *       * * H *
                 *   
                 *   B = matrix[0,1] => matrix[1,3]
                 *   H = matrix[1,3] => matrix[2,3]
                 *   O = matrix[2,3] => matrix[2,0]
                 *   I = matrix[2,0] => matrix[0,1]
                 *   
                 *   * * C *       * * E * 
                 *   E * * *   =>  N * * *
                 *   * * * L       * * * C
                 *   * N * *       * L * *
                 *   
                 *   C = matrix[0,2] => matrix[2,3]
                 *   L = matrix[2,3] => matrix[1,3]
                 *   N = matrix[1,3] => matrix[1,0]
                 *   E = matrix[1,0] => matrix[0,2]
                 *   
                 *   Meaning for i=0, k=0,1,2 and m=n-i-1
                 *    
                 *   matrix[i,i+k]  =>  matrix[k,m]
                 *   matrix[k,m]    =>  matrix[m-k,m]
                 *   matrix[m-k,m]  =>  matrix[m-k,i]
                 *   matrix[m-k,i]  =>  matrix[i,i+k]
                 */

                // matrix[i,i] is upper-left of the inner square
                // having sides of length i.

                // We rotate the permieter of this square.

                for (int k = i, m = n - i - 1; k < m; ++k)
                {
                    /* "up":    matrix[i,i+k]
                       "right": matrix[k,m]
                       "down":  matrix[m-k,m]
                       "left":  matrix[m-k,i]

                       up => right
                       right => down
                       down => left
                       left => up
                    */

                    // TODO(?) variables for duplicate index computations
                    //         such as "m - k" 4 times.

                    var r = matrix[k, m]; // right
                    var l = matrix[m - k, i]; // left

                    matrix[k, m] = matrix[i, i + k]; // up => right
                    matrix[m - k, i] = matrix[m - k, m]; // down => left
                    matrix[i, i + k] = l; // left => up
                    matrix[m - k, m] = r; // right => down
                }
            });
        }

        /// <summary>
        /// Swap 2 elements in an array
        /// </summary>
        /// <typeparam name="T">The type of elements in the array</typeparam>
        /// <param name="arr">The array</param>
        /// <param name="i">The index of one element to swap with the other at index <paramref name="j"/></param>
        /// <param name="j">The index of the other element to swap with the one at index <paramref name="i"/></param>
        private static void Swap<T>(this T[] arr, in int i, in int j)
        {
            T temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }

        /// <summary>
        /// Normalize a number of rotations for a sqaure matrix into a positive
        /// number and reduce if wraps around. See examples.
        /// </summary>
        /// <examples>
        /// Rotating -7 is same as rotating +1
        /// </examples>
        /// <examples>
        /// Rotating -2 is same as rotating +2
        /// </examples>
        /// <example>
        /// Rotating 8 is same as rotating +3
        /// </example>
        /// <example>
        /// Rotating 18 is same as rotating +2
        /// </example>
        /// <param name="rotations">The number of rotations to possibly be normalized</param>
        private static void NormalizeSquareRotations(ref int rotations)
        {
            /*
             * .
             * .
             * -9 => 3
             * -8 => 0
             * -7 => 1
             * -6 => 2
             * -5 => 3
             * -4 => 0
             * -3 => 1
             * -2 => 2
             * -1 => 3
             *  0 => 0
             * +1 => 1
             * +2 => 2
             * +3 => 3
             * +4 => 0
             * +5 => 1
             * +6 => 2
             * +7 => 3
             * +8 => 0
             * +9 => 1
             * .
             * .
             */

            // More elegant formula for the above?
            const int SQUARE_EDGES = 4;
            rotations %= SQUARE_EDGES; // [-3, 3]
            if (rotations < 0) // [-3,-1]
                rotations += SQUARE_EDGES;
        }

        private static void NormalizeShifts(ref int shifts, in int arrLen)
        {
            shifts = shifts % arrLen;
            if (shifts < 0) // e.g. shift -2 same as shift +3 for array of length 5
                shifts = arrLen + shifts;
        }
    }
}
