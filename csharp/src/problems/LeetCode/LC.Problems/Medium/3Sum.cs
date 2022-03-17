namespace LC.Problems.Medium
{
    using LC.Decorations;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class _3Sum
    {
        private readonly int _target;

        public _3Sum(int target)
        {
            this._target = target;
        }

        [LeetCodeProblem(Name = "3Sum", Difficulty = LeetCodeProblemDifficulty.Medium)]
        public IList<IList<int>> Solve(int[] nums)
        {
            throw new NotImplementedException($"TODO: return all triplets summing to {nameof(this._target)}");
        }
    }
}
