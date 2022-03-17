namespace Algorithm.Decorations
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents styles of implementations to an algorithm.
    /// </summary>
    public enum AlgorithmImplementationStyle
    {
        /// <summary>
        /// Code is compact/condensed
        /// </summary>
        Compact,

        /// <summary>
        /// Makes heavy use of existing extensions for <see cref="IEnumerable{T}"/>
        /// </summary>
        LINQy,

        /// <summary>
        /// Anti-Knuth implementation, trivially shaves off a few extra operations
        /// </summary>
        MicroOptimized,

        /// <summary>
        /// Implementation seeks for elegance/readability, possibly
        /// at the expense of performance.
        /// </summary>
        Elegant,

        /// <summary>
        /// Implementation minimizes memory usage, possibily at the
        /// expense of elegance/readability.
        /// </summary>
        MemoryOptimized,

        /// <summary>
        /// Implementation makes use of parallel programming.
        /// </summary>
        Parallelized,

        /// <summary>
        /// Implementation uses DP
        /// </summary>
        DynamicProgramming,

        /// <summary>
        /// Implementation uses recursive function(s)
        /// </summary>
        Recursion
    }
}
