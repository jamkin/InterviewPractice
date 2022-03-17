namespace Mathematics.IntegralTypes.GroupTheory
{
    using Mathematics.Abstractions.GroupTheory;
    using System;

    /// <summary>
    /// Group definition for <see cref="Int32"/>
    /// </summary>
    public class IntegerGroup : IGroupDefinition<int>
    {
        /// <summary>
        /// The single instance of the Group definition
        /// </summary>
        public static IGroupDefinition<int> Instance => _instance.Value;

        /// <summary>
        /// The lazy-loaded singleton of the group definition
        /// </summary>
        /// <returns>
        /// This instance
        /// </returns>
        private static readonly Lazy<IGroupDefinition<int>> _instance = new Lazy<IGroupDefinition<int>>(() => new IntegerGroup());

        /// <summary>
        /// The identity integer
        /// </summary>
        private const int IDENTITY = 0;

        /// <summary>
        /// The ctor
        /// </summary>
        private IntegerGroup() { }

        /// <summary>
        /// The identity integer
        /// </summary>
        public int Identity => IDENTITY;

        /// <summary>
        /// Get the inverse element in the Group
        /// </summary>
        /// <param name="val">The integer whose inverse to return</param>
        /// <returns>
        /// The inverse of <paramref name="val"/>
        /// </returns>
        public int Invert(int val)
        {
            int inv = val * -1;
            if (this.Operate(val, inv) != this.Identity)
            {
                // This should happen for 2^-31, for example, assuming
                // integers are in the range [-2^31, +2^31 - 1]
                throw new InvalidOperationException($"{val} has no inverse.");
            }
            return inv;
        }

        /// <summary>
        /// Run the Group operation to combine two integers
        /// </summary>
        /// <param name="a">The first integer</param>
        /// <param name="b">The second integer</param>
        /// <returns>
        /// The third integer
        /// </returns>
        public int Operate(int a, int b)
        {
            return a + b;
        }
    }
}
