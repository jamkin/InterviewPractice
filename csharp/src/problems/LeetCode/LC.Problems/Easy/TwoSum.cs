namespace LC.Problems.Easy
{
    using Algorithm;
    using LC.Decorations;
    using Mathematics.IntegralTypes.GroupTheory;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Algorithm.Extensions;

    public class TwoSum
    {
        [LeetCodeProblem(Name = "Two Sum", Difficulty = LeetCodeProblemDifficulty.Medium)]
        public int[] Solve(int[] nums, int target)
        {
            if (nums is null)
            {
                throw new ArgumentNullException(nameof(nums));
            }

            if (!nums
                .SelectTargetPairs(target, IntegerGroup.Instance, (value, index) => index)
                .HasSingleElement(out var indices))
            {
                throw new ArgumentException($"Numbers do not have single pair summing to {target}.", nameof(nums));
            }

            return new int[] { indices.Item1, indices.Item2 };
        }
    }
}
