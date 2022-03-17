namespace Algorithm.Decorations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Describes a dimension of an algorithm (time, space) 
    /// in terms of Big-O
    /// </summary>
    public class BigOAttribute : Attribute
    {
        /// <summary>
        /// The best-case complexity
        /// </summary>
        public string BestCase { get; init; }

        /// <summary>
        /// The average-case complexity
        /// </summary>
        public string AverageCase { get; init; }

        /// <summary>
        /// The worse-case complexity
        /// </summary>
        public string WorstCase { get; init; }
    }
}
