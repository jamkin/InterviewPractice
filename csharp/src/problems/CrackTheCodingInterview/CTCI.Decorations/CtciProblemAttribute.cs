namespace CTCI.Decorations
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CtciProblemAttribute : Attribute
    {
        public string Name { get; init; }

        public int Chapter { get; init; }

        public int Problem { get; init; }

        public string Description { get; init; }
    }
}