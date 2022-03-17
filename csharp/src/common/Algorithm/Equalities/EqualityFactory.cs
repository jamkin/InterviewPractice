namespace Algorithm.Equalities
{
    using Algorithm.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Creates equality expressions
    /// </summary>
    internal static class EqualityFactory
    {
        /// <summary>
        /// Empty parameter modifiers
        /// </summary>
        private static readonly ParameterModifier[] NO_PARAMETER_MODIFIERS = new ParameterModifier[] { };

        /// <summary>
        /// Get the "best" equality expression to use for some
        /// type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <returns>
        /// Lambda expression for comparing to instances of type <typeparamref name="T"/>
        /// </returns>
        /// <remarks>
        /// This seeks to solve 2 problems that arise from writing a routine that
        /// needs to compare elements of some generic type <typeparamref name="T"/> to
        /// each other without some specified <see cref="IEqualityComparer{T}"/>.
        /// 
        /// Problem (1): In case <typeparamref name="T"/> is nullable, <see cref="object.Equals(object?)"/>
        ///              of <see cref="IEquatable{T}.Equals(T?)"/> can throw<see cref="NullReferenceException"/>
        /// Problem (2): You can prevent (1) by using <see cref="object.Equals(object?, object?)"/>, but if
        ///              <typeparamref name="T"/> is not nullable, for example <see cref="int"/>, you pay a
        ///              performance penalty from Boxing. You ideally want to hit the <see cref="IEquatable{T}.Equals(T?)"/>
        ///              which is expected to compile to efficient comparison.
        /// </remarks>
        internal static Func<T, T, bool> GetBestEqualityExpression<T>()
        {
            if (typeof(T).IsNullable())
                return (T x, T y) => object.Equals(x, y);
            else if (typeof(IEquatable<T>).IsAssignableFrom(typeof(T)))
                return CreateGenericEqualsExpression<T>().Compile();
            else
                return (T x, T y) => x.Equals(y);
        }

        /// <summary>
        /// Generates lambda expression for <see cref="IEquatable{T}.Equals(T?)"/>
        /// </summary>
        /// <typeparam name="T">The type of elements to be compared for equality</typeparam>
        /// <returns>
        /// The lambda expression generated
        /// </returns>
        private static Expression<Func<T, T, bool>> CreateGenericEqualsExpression<T>()
        {
            var param0 = Expression.Parameter(typeof(T));
            var param1 = Expression.Parameter(typeof(T));
            var method = typeof(T).GetMethod(
                nameof(IEquatable<T>.Equals),
                0,
                BindingFlags.Instance | BindingFlags.Public,
                Type.DefaultBinder,
                new Type[] { typeof(T) },
                NO_PARAMETER_MODIFIERS);
            var body = Expression.Call(param0, method, param1);
            var lambda = Expression.Lambda<Func<T, T, bool>>(body, param0, param1); ;
            return lambda;
        }
    }
}
