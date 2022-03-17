namespace HR.Decorations
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HackerRankProblemAttribute : Attribute
    {
        public string Name { get; set; }

        public HackerRankProblemDifficulty Difficulty { get; set; }
    }
}