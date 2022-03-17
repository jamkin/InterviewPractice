namespace LC.Decorations
{
    [AttributeUsage(AttributeTargets.Method)]
    public class LeetCodeProblemAttribute : Attribute
    {
        public string Name { get; init; }

        public LeetCodeProblemDifficulty Difficulty { get; init; }
    }
}