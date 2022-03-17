namespace Mathematics.Abstractions.GroupTheory
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents the definition of a mathematical Group for a type <typeparamref name="T"/>.
    /// For example, <see cref="int"/> is a group because it has an identity (0), has an
    /// operator (+) and so on.
    /// 
    /// NOTE that in some cases the implementation may be for an "approximate" Group, such
    /// as with <see cref="int"/>. In that case the numbers span the range [-2^31, 2^31 - 1],
    /// so there is NOT an inverse for -2^31.
    /// </summary>
    /// <typeparam name="T">The type of elements that form a Group</typeparam>
    public interface IGroupDefinition<T> where T : IEquatable<T>
    {
        /// <summary>
        /// The Identity element in the Group
        /// </summary>
        T Identity { get; }

        /// <summary>
        /// The group operation, must be Associative
        /// </summary>
        /// <param name="a">An element in the Group</param>
        /// <param name="b">An element in the Group</param>
        /// <returns>
        /// The resulting element int the Group
        /// </returns>
        T Operate(T a, T b);

        /// <summary>
        /// Get the inverse of an element in the Group
        /// </summary>
        /// <param name="t">The element in the Group</param>
        /// <returns>
        /// The inverse
        /// </returns>
        T Invert(T t);
    }
}
