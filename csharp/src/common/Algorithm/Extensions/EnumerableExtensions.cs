namespace Algorithm.Extensions
{
    using Algorithm.Decorations;
    using Mathematics.Abstractions.GroupTheory;
    using System.Collections;
    using System.Linq;
    using System.Collections.Immutable;
    using Algorithm.Equalities;

    /// <summary>
    /// Extension methods for <see cref="IEnumerable"/> and <see cref="IEnumerable{T}"/>,
    /// implemented as Streaming Algorithms when possible and beneficial.
    /// </summary>
    /// <remarks>
    /// TODO: Maybe split into partial classes by some logical grouping
    /// </remarks>
    public static class EnumerableExtensions
    {
        [AlgorithmImplementationStyle(AlgorithmImplementationStyle.LINQy)]
        [AlgorithmImplementationStyle(AlgorithmImplementationStyle.Elegant)]
        [AlgorithmImplementationStyle(AlgorithmImplementationStyle.Recursion)]
        [TimeComplexity(AverageCase = "O(n!)", BestCase = "O(n!)", WorstCase = "O(n!)")]
        [SpaceComplexity(AverageCase = "O(n!)", BestCase = "O(n!)", WorstCase = "O(n!)")]
        /// <summary>
        /// Get all permutations of an enumeration.
        /// </summary>
        /// <example>
        /// [A,B,C] => [[A,B,C],[A,C,B],[B,A,C],[B,C,A],[C,A,B],[C,B,A]]
        /// [A,B,A] => [[A,B,A],[A,A,B],[B,A,A],[B,A,A],[A,B,A],[A,A,B]]
        /// </example>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <param name="source">The enumeration to permute</param>
        /// <returns>
        /// All permutations of <paramref name="source"/>
        /// </returns>
        /// <exception cref="ArgumentNullException/">
        public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var mover = source.GetEnumerator();
            if (!mover.MoveNext()) // No elements
            {
                yield break;
            }
            T first = mover.Current;
            if (!mover.MoveNext()) // Only 1 element
            {
                yield return Single(first);
                yield break;
            }
            // If here, more than 1 element and we're on the 2nd
            var perms = source.SelectMany((t, i) => source
                .SkipAt(i)
                .Permutations() // each sub-permutation excluding t
                .Select(p => p.Prepend(t))); // add t to the front of the sub-permutation
            foreach (var perm in perms)
            {
                yield return perm;
            }
        }

        [AlgorithmImplementationStyle(AlgorithmImplementationStyle.LINQy)]
        [AlgorithmImplementationStyle(AlgorithmImplementationStyle.MemoryOptimized)]
        [TimeComplexity(AverageCase = "O(m+n)", BestCase = "O(m+n)", WorstCase = "O(m+n)")]
        [SpaceComplexity(AverageCase = "O(m+n)", BestCase = "O(m+n)", WorstCase = "O(m+n")]
        /// <summary>
        /// Merge two sorted enumerations into one sorted enumeration
        /// </summary>
        /// <example>
        /// [1, 1, 3, 4, 5], [1, 2, 8] -> [1, 1, 1, 2, 3, 4, 5, 8]
        /// </example>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <param name="first">One of the sorted enumerations</param>
        /// <param name="second">Another of the sorted enumerations</param>
        /// <returns>
        /// A sorted enumeration of <paramref name="first"/> and <paramref name="second"/> combined
        /// </returns>
        /// <exception cref="ArgumentNullException/">
        public static IEnumerable<T> MergeSorted<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first is null)
            {
                throw new ArgumentNullException(nameof(first));
            }

            if (second is null)
            {
                throw new ArgumentNullException(nameof(second));
            }

            // TODO(?) Validate that each is sorted, while we enumerate them.
            //         Probably note necessary, trust input since the method
            //         name is 'Merge Sorted' after all.

            IComparer<T> comparer = Comparer<T>.Default;

            for (IEnumerator<T> fm = first.GetEnumerator(), sm = second.GetEnumerator(); ;)
            {
                if (fm.MoveNext())
                {
                    if (sm.MoveNext())
                    {
                        T f = fm.Current, s = sm.Current;
                        if (comparer.Compare(f, s) <= 0)
                        {
                            yield return s;
                            yield return f;
                        }
                        else
                        {
                            yield return f;
                            yield return s;
                        }
                    }
                    else
                    {
                        var remainder = fm.ToEnumerable_AlreadyOnFirst();
                        foreach (var f in remainder)
                        {
                            yield return f;
                        }
                        break; // reached end
                    }
                }
                else
                {
                    if (sm.MoveNext())
                    {
                        var remainder = sm.ToEnumerable_AlreadyOnFirst();
                        foreach (var s in remainder)
                        {
                            yield return s;
                        }
                        break; // reached end
                    }
                    else // simultaneously reached end of both
                    {
                        break;
                    }
                }
            }
        }

        [AlgorithmImplementationStyle(AlgorithmImplementationStyle.LINQy)]
        [AlgorithmImplementationStyle(AlgorithmImplementationStyle.MemoryOptimized)]
        [TimeComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        [SpaceComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        /// <summary>
        /// Split enumeration by some element, returning subenumerations in between all
        /// occurrences of the element.
        /// </summary>
        /// <typeparam name="T">The type of element to split by</typeparam>
        /// <example>[A, B, X, A, X, X, C, A, X], X -> [[A, B], [A], [C, A]]</example>
        /// <param name="source">The enumeration to split</param>
        /// <param name="splitter">The element to split by</param>
        /// <param name="removeEmptyEntries">
        /// Whether to remove empty entires. For example, if set to false,
        /// then instead of 
        /// 
        /// [A, B, X, A, X, X, C, A, X], X -> [[A, B], [A], [C, A]]
        /// 
        /// there would be
        /// 
        /// [A, B, X, A, X, X, C, A, X], X -> [[A, B], [A], [], [C, A]]
        /// 
        /// because of the emptiness between the 2 Xs.
        /// </param>
        /// <returns>The subenumerations</returns>
        /// <exception cref="ArgumentNullException"/>
        public static IEnumerable<IEnumerable<T>> SplitBy<T>(this IEnumerable<T> source, T splitter, bool removeEmptyEntries = true)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            Func<T, T, bool> equals = EqualityFactory.GetBestEqualityExpression<T>();

            return source.SplitBy((T t) => equals(t, splitter), removeEmptyEntries);
        }

        [AlgorithmImplementationStyle(AlgorithmImplementationStyle.LINQy)]
        [AlgorithmImplementationStyle(AlgorithmImplementationStyle.MemoryOptimized)]
        [TimeComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        [SpaceComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        /// <summary>
        /// Chunk enumeration into subenumerations of chunks of a specificed size.
        /// </summary>
        /// <param name="source">The enumeration</param>
        /// <param name="chunkSize">The size of the chunks</param>
        /// <typeparam name="T">The type of items in the enumeration</typeparam>
        /// <example>[A, B, C, D, E], 2 -> [[A, B], [C, D], [E]]</example>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <returns>
        /// An enumeration of enumerations, each subenumeration being of the chunk size
        /// except possibly the last.
        /// </returns>
        /// <remarks>
        /// Only pulls in 1 element at a time and only enumerates once, unlike most implements of enumeration chunking.
        /// </remarks>
        public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (chunkSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(chunkSize), $"Chunk size cannot be less than 1. Value provided: {chunkSize}.");
            }

            var k = chunkSize - 1;
            for (var mover = source.GetEnumerator(); mover.MoveNext();)
            {
                yield return mover.ToEnumerable_AlreadyOnFirst().Take(k);
            }
        }

        [AlgorithmImplementationStyle(Flavor = AlgorithmImplementationStyle.Elegant)]
        [SpaceComplexity(AverageCase = "O(m+n)", BestCase = "O(m+n)", WorstCase = "O(m+n)")]
        [TimeComplexity(AverageCase = "O(m+n)", BestCase = "O(m+n)", WorstCase = "O(m+n)")]
        /// <summary>
        /// Check whether one enumeration is a permutation of the other, meaning
        /// that the elements could be rearranged to produce equal sequences (same
        /// element at each index)
        /// </summary>
        /// <exampe>
        /// [A, B, C], [B, A, C] -> true
        /// [A, B, B], [B, A, B] -> true
        /// [A, B, C], [A, B, B] -> false
        /// [A, A, C], [A, C, A] -> true
        /// [A, B, C, B] -> [B, A, C, A] -> false
        /// [A, B, C, B] -> [B, A, C, B] -> true
        /// </exampe>
        /// <typeparam name="T">The type of element in the enumeration</typeparam>
        /// <param name="first">The first enumeration</param>
        /// <param name="second">The second enumeration</param>
        /// <returns>
        /// <see cref="true"/> if enumerations are permutations of each other,
        /// <see cref="false"/> it not
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        public static bool IsPermutationOf<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first is null)
            {
                throw new ArgumentNullException(nameof(first));
            }

            if (second is null)
            {
                throw new ArgumentNullException(nameof(second));
            }

            // Potential optimization
            if (first.HasCountValueStored(out var firstCount)
             && second.HasCountValueStored(out var secondCount)
             && firstCount != secondCount)
            {
                return false;
            }

            IDictionary<T, int> counts = second.Counts();
            foreach (var x in first)
            {
                if (!counts.TryGetValue(x, out var count))
                    return false;

                if (count == 1)
                    counts.Remove(x);
                else
                    --counts[x];
            }

            return !counts.Any();
        }

        [AlgorithmImplementationStyle(AlgorithmImplementationStyle.LINQy)]
        [AlgorithmImplementationStyle(AlgorithmImplementationStyle.MemoryOptimized)]
        [TimeComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        [SpaceComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        /// <summary>
        /// Split enumeration by some element, returning subenumerations in between all
        /// occurrences of the elements matching some predicate.
        /// </summary>
        /// <typeparam name="T">The type of element to split by</typeparam>
        /// <param name="source">The enumeration to split</param>
        /// <param name="splitter">The predicate for splitting</param>
        /// <param name="removeEmptyEntries">
        /// Whether to remove empty entires. For example, if set to false,
        /// then instead of 
        /// 
        /// [A, B, X, A, X, X, C, A, X], X -> [[A, B], [A], [C, A]]
        /// 
        /// there would be
        /// 
        /// [A, B, X, A, X, X, C, A, X], X -> [[A, B], [A], [], [C, A]]
        /// 
        /// because of the emptiness between the 2 Xs.
        /// </param>
        /// <returns>The subenumerations</returns>
        /// <exception cref="ArgumentNullException"/>
        public static IEnumerable<IEnumerable<T>> SplitBy<T>(this IEnumerable<T> source, Predicate<T> splitter, bool removeEmptyEntries = true)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (splitter is null)
            {
                throw new ArgumentNullException(nameof(splitter));
            }

            if (removeEmptyEntries)
            {
                return source.SplitBy_NoEmpties(splitter);
            }
            else
            {
                return source.SplitBy_IncludeEmpties(splitter);
            }
        }

        [TimeComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        [SpaceComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        /// <summary>
        /// Get all pairs of items in a Group that combine to a targetted item. This can
        /// include duplicate pairs if there are duplicate items. The pairs are not returned
        /// in any particular order.
        /// </summary>
        /// <example>
        /// [5, 3, -1, 2, 0], 2 -> [[2, 0], [3, -1]]
        /// </example>
        /// <example>
        /// [-1, 5, 3, -1, 2, 0], 2 -> [[3, -1], [2, 0], [3, -1]]
        /// </example>
        /// <example>
        /// [-1, 5, 3, -1, 2, 0], -2 -> [[-1, -1]]
        /// </example>
        /// <example>
        /// [-1, 5, 3, -1, 2, 0], 6 -> []
        /// </example>
        /// <param name="source">The items in the Group</param>
        /// <param name="target">The target value</param>
        /// <param name="groupDefinition">The definition of the Group</param>
        /// <typeparam name="T">The type of elements in the group</typeparam>
        /// <returns>
        /// Tuples of the values of each pair combining to <paramref name="target"/>
        /// </returns>
        public static IEnumerable<Tuple<T, T>> GetTargetPairs<T>(this IEnumerable<T> source, T target, IGroupDefinition<T> groupDefinition) where T : IEquatable<T>
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (groupDefinition is null)
            {
                throw new ArgumentNullException(nameof(groupDefinition));
            }

            if (source is not T[] arr)
            {
                // Note: similar optimization for IList
                arr = source.ToArray();
            }

            return source
                .SelectTargetPairs(target, groupDefinition, (t, i) => t);
        }

        [TimeComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        [SpaceComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        /// <summary>
        /// Select on all pairs of items in a Group that combine to a targetted item. This can
        /// include duplicate pairs if there are duplicate items. The pairs are not returned
        /// in any particular order. The select operates on the value and index
        /// </summary>
        /// <example>
        /// [5, 3, -1, 2, 0], 2 -> [[3 (1), -1 (2)], [2 (3), 0 (4)]]
        /// </example>
        /// <param name="source">The items in the Group</param>
        /// <param name="target">The target value</param>
        /// <param name="groupDefinition">The definition of the Group</param>
        /// <param name="selector">The selector on item values and indices</param>
        /// <typeparam name="T">The type of elements in the group</typeparam>
        /// <returns>
        /// Result of <paramref name="selector"/> on each pair value-index pair whose values <typeparamref name="T"/> combine to <paramref name="target"/>
        /// </returns>
        public static IEnumerable<Tuple<U, U>> SelectTargetPairs<T, U>(
            this IEnumerable<T> source, T target, IGroupDefinition<T> groupDefinition, Func<T, int, U> selector) where T : IEquatable<T>
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (groupDefinition is null)
            {
                throw new ArgumentNullException(nameof(groupDefinition));
            }

            if (source is not T[] arr)
            {
                // Note: similar optimization if IList
                arr = source.ToArray();
            }

            // create lookup of values to indices where they occur
            var lookup = source
                .Select((t, i) => new { t, i })
                .GroupBy(x => x.t)
                .ToDictionary(g => g.Key, g => (ISet<int>?)g.Select(x => x.i).ToHashSet());

            foreach (var kvp in lookup)
            {
                var element = kvp.Key;
                var indices = kvp.Value;
                if (indices is null) // means already yielded it, that it was invalidated
                {
                    continue;
                }

                var inverse = groupDefinition.Invert(element);
                var other = groupDefinition.Operate(target, inverse);
                // When here, it means
                // "element + other = target"
                // or equivalently
                // "target + (-element) = other"
                if (element.Equals(other)) // Special case like 4+4=8
                {
                    if (indices.Count > 1)
                    {
                        // Means there are 2 or more of the number, e.g. 2 4s when target is 8.
                        // Return all unique pairs
                        foreach (var tuple in indices.UniquePairs())
                        {
                            yield return Tuple.Create(
                                item1: selector(element, tuple.Item1),
                                item2: selector(element, tuple.Item2));
                        }
                        lookup[element] = null;

                        // Note: We could optimize and continue rest of enumeration
                        //       w/out the element=other check, since we already
                        //       passed the element and can only pass it once.
                    }
                }
                else if (lookup.TryGetValue(other, out var otherIndices)) // normal case like 5+3=8 or -5+13=8
                {
                    foreach (var index in indices)
                        foreach (var otherIndex in otherIndices)
                        {
                            yield return Tuple.Create(
                                item1: selector(element, index),
                                item2: selector(other, otherIndex));
                        }
                    lookup[element] = lookup[other] = null;
                }
            }
        }

        /// <summary>
        /// Find and replace all occurrences of a subenumeration within an enumeration
        /// </summary>
        /// <example>
        /// [A, B, C, B, C, A, B, D, B, A], [A, B], [X, Y, Z] -> [X, Y, Z, C, B, C, X, Y, Z, D, B, A]
        /// </example>
        /// <example>
        /// [A, B, C, B, C, A, B, D, B, A], [A, B], [X] -> [X, C, B, C, X, D, B, A]
        /// </example>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <param name="source">The enumeration</param>
        /// <param name="find">The subenumeration to replace with <paramref name="replace"/></param>
        /// <param name="replace">The subenumeration to replace <paramref name="find"/></param>
        /// <returns>
        /// Enumeration that is the same as <paramref name="source"/> except for all subenumerations
        /// equalling <paramref name="find"/> and instead <see cref="replace"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public static IEnumerable<T> FindAndReplace<T>(this IEnumerable<T> source, IEnumerable<T> find, IEnumerable<T> replace)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (find is null)
            {
                throw new ArgumentNullException(nameof(find));
            }

            if (replace is null)
            {
                throw new ArgumentNullException(nameof(replace));
            }

            throw new NotImplementedException("TODO: Implement LINQy find-and-replace");
        }

        [AlgorithmImplementationStyle(AlgorithmImplementationStyle.LINQy)]
        [AlgorithmImplementationStyle(AlgorithmImplementationStyle.MemoryOptimized)]
        [TimeComplexity(AverageCase = "O(min(k,n))", BestCase = "O(1)", WorstCase = "O(n)")]
        [SpaceComplexity(AverageCase = "O(1)", BestCase = "O(1)", WorstCase = "O(1)")]
        /// <summary>
        /// Check whether the number of elements in an enumeration equals or
        /// exceeds some given amount amount.
        /// </summary>
        /// <example>
        /// [A, B, A, C, D], 4 -> true
        /// </example>
        /// <example>
        /// [A, B, A, C, D], 5 -> false
        /// </example>
        /// <example>
        /// [A, B, A, C, D], 6 -> false
        /// </example>
        /// <remarks>
        /// This is primarily to optimze for the case where <see cref="IEnumerable{T}.Count"/>
        /// would enumerate over more elements than needed. For example, if we want to check
        /// that there are at least 10 elements in some enumeration which has 1,000,000 elements
        /// and doesn't have property which keeps track of its length (like <see cref="ICollection{T}"/> does).
        /// </remarks>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <param name="source">The enumeration</param>
        /// <param name="k">The number of elements to check for</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException/">
        /// <exception cref="ArgumentOutOfRangeException/">
        public static bool HasAtLeast<T>(this IEnumerable<T> source, int k)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (k < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(k), k.ToString());
            }

            if (source.HasCountValueStored(out int count))
                return count >= k;
            else
            {
                --k;
                return source.SkipWhile((t, i) => i < k).Any();
            }
                
        }

        /// <summary>
        /// Implementation of 'N Choose K' or getting all distinct subenumerations
        /// of a given size.
        /// </summary>
        /// <example>
        /// [A, B, C, D], 3 -> [[A, B, C], [A, B, D], [A, C, D], [B, C, D]] 
        /// [A, B, C, D], 2 -> [[A, B], [A, C], [A, D], [B, C], [B, D], [C, D]]
        /// </example>
        /// <typeparam name="T">The type of items in the enumeration</typeparam>
        /// <param name="source">The enumeration</param>
        /// <param name="k">The number of elements to choose</param>
        /// <returns>
        /// The distinct subenumerations of size <paramref name="k"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="NotImplementedException"/>
        public static IEnumerable<IEnumerable<T>> Choose<T>(this IEnumerable<T> source, int k)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (k < 0)
            {
                throw new ArgumentOutOfRangeException(k.ToString(), nameof(k));
            }
            else if (k == 0)
            {
                return Enumerable.Empty<IEnumerable<T>>();
            }
            else if (k == 1)
            {
                return source.Select(t => Single(t));
            }

            throw new NotImplementedException($"TODO: N choose K = {k}, leveraging {nameof(SelectUniquePairs)}");
        }

        public static IEnumerable<Tuple<T, T>> UniquePairs<T>(this IEnumerable<T> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source is not T[] arr)
            {
                arr = source.ToArray();
            }

            return source.SelectUniquePairs((t, i) => t);
        }

        public static IEnumerable<Tuple<U, U>> SelectUniquePairs<T, U>(this IEnumerable<T> source, Func<T, int, U> selector)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (!source.HasCountValueStored(out int count))
            {
                count = source.Count();
            }

            // backtracking:
            // e.g. [A,B,C,D,E] => [[A,B],[A,C],[A,D],[A,E],[B,C],[B,D],[B,E],[C,D],[C,E],[D,E]]
            //                     [[0,1],[0,2],[0,3],[0,4],[1,2],[1,3],[1,4],[2,3],[2,4],[3,4]]
            // i => [0, 3]
            // j => [i+1, 4]
            return source
                .SelectMany((ti, i) =>
                    source
                    .Select((tj, j) => Tuple.Create(selector(ti, i), selector(tj, j)))
                    .Skip(i + 1)
                    .Take(count - i - 1).ToList());
        }

        [AlgorithmImplementationStyle(AlgorithmImplementationStyle.LINQy)]
        [AlgorithmImplementationStyle(AlgorithmImplementationStyle.MemoryOptimized)]
        [TimeComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        [SpaceComplexity(AverageCase = "O(1)", BestCase = "O(1)", WorstCase = "O(1)")]
        /// <summary>
        /// Get the element that represents the strict majority (more than half)
        /// of elements in the enumeration, or <see cref="null"/> if no element
        /// is the majority.
        /// </summary>
        /// <example>
        /// [A, B, A, C, A] -> A
        /// [A, B, A, C] -> null
        /// [A, B, A, B] -> null
        /// [A, B, B, B] -> B
        /// [A, B, C, D] -> null
        /// </example>
        /// <param name="source">The enumeration</param>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <exception cref="ArgumentNullException/">
        /// <remarks>
        /// The 1st part of the algorithm is a little hard to understand
        /// and be convinced without a formal mathematical proof. 
        /// See some of the remarks in <see cref="MajorityElementCandidateOrNull"/>
        /// </remarks>
        public static T? MajorityElementOrNull<T>(this IEnumerable<T> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var mover = source.GetEnumerator();
            if (!mover.MoveNext())
            {
                return default(T);
            }

            T majCand = mover
                .ToEnumerable_AlreadyOnFirst()
                .MajorityElementCandidateOrNull();

            // Note: There is an optimization we can add.
            //       In the case where there is a majority
            //       that is "left-weighted", like
            // 
            //       [A, A, B, A, A, C] 
            //
            //       , the the counter for A stays above 0
            //       and we don't need to make another pass
            //       to verify whether this is the majority
            //       element, because we know it is.

            Func<T, T, bool> equals = EqualityFactory.GetBestEqualityExpression<T>();

            var majCandCount = source.Count(t => equals(t, majCand), out int totalCount);

            return totalCount / 2 < majCandCount
                ? majCand
                : default(T);
        }

        [AlgorithmImplementationStyle(Flavor = AlgorithmImplementationStyle.MemoryOptimized)]
        [AlgorithmImplementationStyle(Flavor = AlgorithmImplementationStyle.LINQy)]
        [SpaceComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        [TimeComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        /// <summary>
        /// Replace element at a given index
        /// </summary>
        /// <example>
        /// [A, B, C, A, D, A, A, E], X, 3 -> [A, B, C, X, D, A, A, E]
        /// </example>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <param name="source">The enumeration</param>
        /// <param name="replacement">The element to replace</param>
        /// <param name="index">The index of the element to replace</param>
        /// <returns>
        /// The enumeration with the element replaced
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> does not lie in the enumeration</exception>
        public static IEnumerable<T> ReplaceAt<T>(this IEnumerable<T> source, T replacement, int index)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index.ToString());
            }

            if (source.HasCountValueStored(out int count) && count <= index)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is off the end of the enumeration which has only {count} elements.");
            }

            IEnumerator<T> mover;

            // (1) Yield elements before
            for (mover = source.GetEnumerator(); ;)
            {
                if (mover.MoveNext())
                {
                    if (index == 0) // We're on the element to replace
                        break;
                    else
                    {
                        yield return mover.Current;
                        --index;
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is off the end of the enumeration.");
                }
            }

            // (2) Yield the replace; we are on it now
            yield return mover.Current;

            // (3) Yield the rest
            foreach (var current in mover.ToEnumerable())
            {
                yield return current;
            }
        }

        [AlgorithmImplementationStyle(Flavor = AlgorithmImplementationStyle.MemoryOptimized)]
        [AlgorithmImplementationStyle(Flavor = AlgorithmImplementationStyle.LINQy)]
        [TimeComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        [SpaceComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        /// <summary>
        /// Skip element at a given index in an enumeration
        /// </summary>
        /// <example>
        /// [A, B, E, A, B, B, A, C, B], 3 -> [A, B, E, B, B, A, C, B], 3
        /// </example>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <param name="source">The enumeration</param>
        /// <param name="index">The index of the element to skip</param>
        /// <returns>
        /// A new enumeration which has the <typeparamref name="T"/> at index <paramref name="index"/> removed
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException/">If <paramref name="index"/> does not lie in the enumeration</exception>
        public static IEnumerable<T> SkipAt<T>(this IEnumerable<T> source, int index)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(index.ToString(), nameof(index));
            }

            if (source.HasCountValueStored(out int count) && count <= index)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(index),
                    $"Cannot skip element at index {index} because it is off the end of the enumeration with {count} elements.");
            }

            var mover = source.GetEnumerator();

            // The elements before
            for (int i = 0; ; )
            {
                if (mover.MoveNext())
                {
                    if (i == index) // We're on the element we want to skip
                        break;
                    else
                    {
                        yield return mover.Current;
                        ++i;
                    }                
                }
                else
                    throw new ArgumentOutOfRangeException(index.ToString(), $"Cannot skip element at index {index} because it falls off the enumeration.");
            }

            // The elements after
            // Note from the 'break' above that here we are on the element we want to skip

            for ( ; mover.MoveNext(); )
                yield return mover.Current; 
        }

        /// <summary>
        /// Get all distinct permutations of an enumeration. Different
        /// than <see cref="Permutations{T}(IEnumerable{T})"/>, this means
        /// if some elements appear more than once (same value at different
        /// positions) we won't return any sequences which are equivalent.
        /// See how the middle example differents from non-distinct permutations.
        /// </summary>
        /// <example>
        /// [A, B] -> [[A, B], [B, A]]
        /// [A, B, A] -> [[A, A, B], [A, B, A], [B, A, A]]
        /// [A, B, C] -> [[A, B, C], [A, C, B], [B, A, C], [B, C, A], [C, A, B], [C, B, A]]
        /// </example>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <param name="source">The enumeration</param>
        /// <returns>All distinct permutations of <paramref name="source"/></returns>
        /// <exception cref="ArgumentNullException/">
        /// <exception cref="NotImplementedException"/>
        public static IEnumerable<IEnumerable<T>> DistinctPermutations<T>(this IEnumerable<T> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            throw new NotImplementedException();
        }

        [AlgorithmImplementationStyle(Flavor = AlgorithmImplementationStyle.MemoryOptimized)]
        [AlgorithmImplementationStyle(Flavor = AlgorithmImplementationStyle.LINQy)]
        [TimeComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        [SpaceComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        /// <summary>
        /// Get all subenumerations which represent strictly increasing intervals
        /// </summary>
        /// <example>
        /// [1, 0, 1, 5, 5, 3, 3, 6, -3, 1, 0] -> [[0, 1, 5], [3, 6], [-3, 1]]
        /// </example>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <param name="source">The enumeration</param>
        /// <returns>
        /// All increasing intervals, in their original order
        /// </returns>
        /// <exception cref="ArgumentNullException/">
        /// <exception cref="NotImplementedException"/>
        public static IEnumerable<IEnumerable<T>> IncreasingSubenumerations<T>(this IEnumerable<T> source) where T : IComparable<T>
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            // Note: this is optimized for the case where there are reasonably
            //       sized runs of increasing and decreasing intervals. If the
            //       enumeration switches frequently then a simpler algorithm
            //       will be just as good.
            IEnumerator<T> mover;
            bool reachedEnd;
            for (mover = source.GetEnumerator(), reachedEnd = false; !reachedEnd; )
            {
                if (CollectWhileIncreasing(mover, out reachedEnd, out IEnumerable<T> increasing))
                {
                    yield return increasing;
                }
                else // Didn't increase
                {
                    if (!reachedEnd)
                    {
                        MoveWhileNonIncreasing(mover, out reachedEnd);
                    }
                }
            }
        }

        [SpaceComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        [TimeComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        /// <summary>
        /// Put all unique elements of an enumeration into a set
        /// </summary>
        /// <example>
        /// [A, B, A, C, D, A, A, D, C] -> {A, C, B, D}
        /// </example>
        /// <typeparam name="T">The type of elements</typeparam>
        /// <param name="source">The enumeration</param>
        /// <returns>
        /// A new set with the unique elements
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        public static ISet<T> ToSet<T>(this IEnumerable<T> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return new HashSet<T>(source);
        }

        [AlgorithmImplementationStyle(Flavor = AlgorithmImplementationStyle.LINQy)]
        [AlgorithmImplementationStyle(Flavor = AlgorithmImplementationStyle.MemoryOptimized)]
        [SpaceComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        [TimeComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        /// <summary>
        /// Partition enumeration by some predicate. The elements that don't
        /// match the predicate are yielded all before the elements that do
        /// match the prediate.
        /// </summary>
        /// <example>
        /// [1, 4, 2, 4, 3, 6, 7, 1, 0, 2], Is Even => [1, 3, 7, 1, 4, 2, 4, 6, 0, 2]
        /// </example>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <param name="source">The enumeration</param>
        /// <param name="predicate">The predicate</param>
        /// <returns>
        /// The partioned enumeration
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        public static IEnumerable<T> PartitionBy<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return source
                .Where(t => !predicate(t))
                .Concat(source.Where(t => predicate(t)));
        }

        [SpaceComplexity(AverageCase = "O(1)", BestCase = "O(1)", WorstCase = "O(1)")]
        [TimeComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        /// <summary>
        /// Append a single element to the end of an enumeration
        /// </summary>
        /// <example>
        /// [B, A, C, A], X => [B, A, C, A, X]
        /// </example>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <param name="source">The enumeration</param>
        /// <param name="t">The element to append</param>
        /// <returns>
        /// Enumeration that is the same as <paramref name="source"/>
        /// except with a single element <paramref name="t"/> added
        /// to the end.
        /// </returns>
        /// <exception cref="ArgumentNullException/">
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T t)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            foreach (var item in source)
            {
                yield return item;
            }

            yield return t;
        }

        [SpaceComplexity(AverageCase = "O(1)", BestCase = "O(1)", WorstCase = "O(1)")]
        [TimeComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        /// <summary>
        /// Prepend a single element to the beginning of an enumeration
        /// </summary>
        /// <example>
        /// [B, A, C, A], X => [X, B, A, C, A]
        /// </example>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <param name="source">The enumeration</param>
        /// <param name="t">The element to prepend</param>
        /// <returns>
        /// Enumeration that is the same as <paramref name="source"/>
        /// except with a single element <paramref name="t"/> added
        /// to the beginning.
        /// </returns>
        /// <exception cref="ArgumentNullException/">
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T t)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            yield return t;

            foreach (var item in source)
            {
                yield return item;
            }
        }

        [AlgorithmImplementationStyle(Flavor = AlgorithmImplementationStyle.LINQy)]
        [TimeComplexity(AverageCase = "O(n)", BestCase = "O(1)", WorstCase = "O(n)")]
        [SpaceComplexity(AverageCase = "O(n)", BestCase = "O(1)", WorstCase = "O(n)")]
        /// <summary>
        /// Shift elements in enumeration over (to the "right")
        /// by some amount, wrapping around so that any pushed
        /// off the end are moved to the beginning in the same sequence
        /// </summary>
        /// <example>
        /// [A,B,C,D], 1 => [D,A,B,C]
        /// </example>
        /// <example>
        /// [A,B,C,D], 2 => [C,D,A,B]
        /// </example>
        /// <example>
        /// [A,B,C,D], -1 => [B,C,D,A]
        /// </example>
        /// <typeparam name="T">The type of element in the enumeration</typeparam>
        /// <param name="source">The enumeration</param>
        /// <param name="shifts">The number of shifts, negatives allowed for "left" shift (see examples)</param>
        /// <returns>
        /// The shifted enumeration
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        public static IEnumerable<T> Shift<T>(this IEnumerable<T> source, int shifts = 1)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (shifts == 0)
                return source;

            source.NormalizeShifts(ref shifts, out int count);

            if (shifts == count)
                return source;

            count -= shifts; // e.g. if 5 elements and shifting 1, this is 4
            return source.Skip(count).Concat(source.Take(count));
        }

        [SpaceComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        [TimeComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        /// <summary>
        /// Get counts of elements in enumration
        /// </summary>
        /// <example>
        /// [C, A, B, A, C, E, D, E, A, A] => {A: 4, B: 1, C: 2, D: 1, E: 2}
        /// </example>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <param name="source">The enumeration itself</param>
        /// <returns>
        /// Dictionary whose keys are the elements and whose values are the counts
        /// </returns>
        /// <exception cref="ArgumentNullException/">
        public static IDictionary<T, int> Counts<T>(this IEnumerable<T> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.GroupBy(t => t).ToDictionary(g => g.Key, g => g.Count());
        }

        public static bool HasSingleElement<T>(this IEnumerable<T> source, out T single)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var mover = source.GetEnumerator();
            if (!mover.MoveNext())
            {
                single = default;
                return false;
            }
            single = mover.Current;
            if (mover.MoveNext())
            {
                single = default;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check whether enumerations consists of all unique (non-repeated) elements
        /// </summary>
        /// <example>
        /// [A, B, D, C] => true
        /// [A, B, A, C] => false
        /// </example>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <param name="source">The enumeration</param>
        /// <returns>
        /// <see cref="true"/> if unique elements, <see cref="false"/> otherwise
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        [SpaceComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        [TimeComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        public static bool IsUnique<T>(this IEnumerable<T> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source is ISet<T> || source is IImmutableSet<T>)
            {
                return true;
            }

            ISet<T> set = new HashSet<T>();
            foreach (var t in source)
            {
                if (set.Contains(t))
                    return false;
                else
                    set.Add(t);
            }
            return true;
        }

        [SpaceComplexity(AverageCase = "O(n)", BestCase = "O(1)", WorstCase = "O(n)")]
        [TimeComplexity(AverageCase = "O(n)", BestCase = "O(n)", WorstCase = "O(n)")]
        /// <summary>
        /// Remove leading or trailing occurrences of an item in an enumeration
        /// </summary>
        /// <param name="source">The enumeration</param>
        /// <param name="items">The items to trim</param>
        /// <typeparam name="T">The type of items in the enumeration</typeparam>
        /// <returns>
        /// The trimmed enumeration
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        public static IEnumerable<T> Trim<T>(this IEnumerable<T> source, params T[] items)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (items.Length == 0)
            {
                return source;
            }

            return source.TrimInner();
        }

        public static IEnumerable<KeyValuePair<T, int>> AdjacentCounts<T>(this IEnumerable<T> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var mover = source.GetEnumerator();
            if (!mover.MoveNext())
            {
                yield break;
            }

            Func<T, T, bool> equals = EqualityFactory.GetBestEqualityExpression<T>();
            // We're already on the 1st element here
            for (bool reachedEnd = false; !reachedEnd; )
            {
                T cur = mover.Current;
                var neighborCount = mover.CountWhile((T t) => equals(t, cur), out reachedEnd);
                yield return new KeyValuePair<T, int>(cur, neighborCount + 1);
            }
        }

        public static bool IsRotationOf<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first is null)
            {
                throw new ArgumentNullException(nameof(first));
            }

            if (second is null)
            {
                throw new ArgumentNullException(nameof(second));
            }

            if (object.ReferenceEquals(first, second))
            {
                return true;
            }

            int count = first.Count();
            if (count != second.Count())
            {
                return false;
            }

            // TODO(?) Time optimization like check first that
            //         is permutation.
            //         Below is for memory over time.

            return Enumerable.Range(0, count)
                .AsParallel()
                .Select(i => first.SequenceEqual(second.Shift(i)))
                .Any();
        }

        #region private methods

        /// <summary>
        /// Create enumeration consisting of single element
        /// </summary>
        /// <typeparam name="T">The type of element</typeparam>
        /// <param name="t">The element</param>
        /// <returns>
        /// The enumeration
        /// </returns>
        private static IEnumerable<T> Single<T>(T t)
        {
            yield return t;
        }

        /// <summary>
        /// Move enumerator while enumeration is increasing and capture
        /// the increasing subenumeration in an output.
        /// </summary>
        /// <typeparam name="T">The type of elements in the enumeration, which have some order relation</typeparam>
        /// <param name="mover">The enumerator that is moving over the sequence</param>
        /// <param name="reachedEnd">Whether the end of the enumeration was reached</param>
        /// <param name="increasings">The increasing sequence of elements, if there was one starting where we are with <paramref name="mover"/></param>
        /// <returns>
        /// <see cref="true"/> if enumeration was increasing, <see cref="false"/> if not.
        /// This value corresponds to whether there was anything outputted to <paramref name="increasings"/>.
        /// </returns>
        private static bool CollectWhileIncreasing<T>(IEnumerator<T> mover, out bool reachedEnd, out IEnumerable<T> increasings) where T : IComparable<T>
        {
            T last = mover.Current;
            reachedEnd = !mover.MoveNext();
            if (reachedEnd)
            {
                increasings = Enumerable.Empty<T>();
                return false; // we did not increase
            }
            else
            {
                T cur = mover.Current;
                if (last.CompareTo(cur) < 0)
                {
                    IList<T> list = new List<T>(capacity: 8); // speculate that still increasing, at leat some
                    list.Add(last);
                    list.Add(cur);
                    for (bool stillIncreasing = true; (reachedEnd = !mover.MoveNext()) && stillIncreasing;)
                    {
                        last = cur;
                        cur = mover.Current;
                        if (stillIncreasing = last.CompareTo(cur) < 0)
                        {
                            list.Add(cur);
                        }
                    }
                    increasings = list;
                    return true; // we did increase
                }
                else
                {
                    increasings = Enumerable.Empty<T>();
                    reachedEnd = false;
                    return false; // we did not increase
                }
            }
        }

        /// <summary>
        /// Move enumerator while enumeration is non-increasing.
        /// When the method is finished executing, the enumerator
        /// will either be off the end of the enumeration or at the
        /// point where it began increasing; this is indicated by
        /// the value outputted for <paramref name="reachedEnd"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <param name="mover">The enumerator</param>
        /// <param name="reachedEnd">Whether or not the end of the enumeration was reached</param>
        /// <exception cref="NotImplementedException"/>
        private static void MoveWhileNonIncreasing<T>(IEnumerator<T> mover, out bool reachedEnd) where T : IComparable<T>
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get an enumeration starting at an enumerator and including
        /// the element the enumerator is current moved on, while the
        /// elements match some predicate.
        /// </summary>
        /// <typeparam name="T">Type type of elements if the enumeration</typeparam>
        /// <param name="mover">
        /// The enumerator, which should already have <see cref="IEnumerator.MoveNext"/>'d
        /// so that there is a <see cref="IEnumerator{T}.Current"/>.
        /// </param>
        /// <param name="predicate">
        /// The predicate to determine whether the enumeration should terminate once it
        /// reaches a value that does not match.
        /// </param>
        /// <returns>
        /// An enumeration
        /// </returns>
        private static IEnumerable<T> TakeWhile_AlreadyOnFirst<T>(this IEnumerator<T> mover, Predicate<T> predicate)
        {
            return Single(mover.Current)
                .Concat(mover.ToEnumerable())
                .TakeWhile(t => predicate(t));
        }

        /// <summary>
        /// Move an enumerator while its current value matches a predicate,
        /// while the enumeraor is already moved on to the first value, and
        /// as long as the current value matches a predicate.
        /// </summary>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <param name="mover">
        /// The enumertor which is already on a value (does not need <see cref="IEnumerator.MoveNext"/>
        /// to get the first <see cref="IEnumerator{T}.Current"/>.)
        /// </param>
        /// <param name="reachedEnd">
        /// <see cref="true"/> if the end of the enumeration was reached, meaning every element till the
        /// end matched the predicate <paramref name="predicate"/>; <see cref="false"/> if did not reach
        /// the end, meaning some element enumerated did not match the predicate <paramref name="predicate"/>
        /// </param>
        /// <param name="predicate">The predicate on the element to determine whether to keep moving</param>
        private static void MoveWhile_AlreadyOnFirst<T>(this IEnumerator<T> mover, Predicate<T> predicate, out bool reachedEnd)
        {
            bool match;
            reachedEnd = false;
            do
            {
                T cur = mover.Current;
                match = predicate(cur);
                reachedEnd = !mover.MoveNext();
            } while (match && !reachedEnd);
        }

        private static int CountWhile<T>(this IEnumerator<T> mover, Predicate<T> predicate, out bool reachedEnd)
        {
            int count;
            for (count = 0; !(reachedEnd = !mover.MoveNext()) && predicate(mover.Current); ++count);
            return count;
        }

        /// <summary>
        /// Split enumeration by predicate into subenumerations, not including any empty
        /// subenumerations, meaning that the "enumeration between" elements matching the
        /// predicate is not a valid concept.
        /// </summary>
        /// <example>
        /// [4, 3, 1, 2, 1, 5, 3, 6, 2, 1, 9], Is Even => [[3, 1], [1, 5, 3], [1, 9]]
        /// </example>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <param name="source">The enumeration</param>
        /// <param name="predicate">The predicate to split by, to use enumerations to left and right when it returns <see cref="true"/></param>
        /// <returns>
        /// An enumeration of subenumerations produced by the splitting
        /// </returns>
        private static IEnumerable<IEnumerable<T>> SplitBy_NoEmpties<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            var notPredicate = predicate.Negate();
            for (var mover = source.GetEnumerator(); mover.MoveNext();)
            {
                if (predicate(mover.Current))
                {
                    yield return mover.TakeWhile_AlreadyOnFirst(predicate);
                }
                else
                {
                    mover.MoveWhile_AlreadyOnFirst(notPredicate, out bool reachedEnd);
                    // TODO: process reachedEnd output
                }
            }
        }

        /// <summary>
        /// Split enumeration by predicate into subenumerations, including any empty
        /// subenumerations, meaning that the "enumeration between" elements matching
        /// the predicate is a valid concept. See example.
        /// </summary>
        /// <example>
        /// [4, 3, 1, 2, 1, 5, 3, 6, 2, 1, 9], Is Even => [[], [3, 1], [1, 5, 3], [], [1, 9]]
        /// </example>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <param name="source">The enumeration</param>
        /// <param name="predicate">The predicate to split by, to use enumerations to left and right when it returns <see cref="true"/></param>
        /// <returns>
        /// An enumeration of subenumerations produced by the splitting
        /// </returns>
        private static IEnumerable<IEnumerable<T>> SplitBy_IncludeEmpties<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            var notPredicate = predicate.Negate();
            for (var mover = source.GetEnumerator(); mover.MoveNext();)
            {
                if (predicate(mover.Current))
                {
                    yield return mover.TakeWhile_AlreadyOnFirst(predicate);
                }
                else
                {
                    yield return Enumerable.Empty<T>();
                }
            }
        }

        /// <summary>
        /// Convert enumerator to enumerable, starting where the enumerator is currently.
        /// is currently.
        /// </summary>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <param name="mover">The enumerator which is already <see cref="IEnumerator.MoveNext"/>'d onto an element <see cref="IEnumerator{T}.Current"/></param>
        /// <returns>
        /// The enumereation.
        /// </returns>
        private static IEnumerable<T> ToEnumerable_AlreadyOnFirst<T>(this IEnumerator<T> mover)
        {
            T cur = mover.Current;
            return Single(cur).Concat(mover.ToEnumerable());
        }

        /// <summary>
        /// Convert enumerator to enumeration.
        /// </summary>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <param name="mover">The enumerator</param>
        /// <returns>
        /// The enumeration
        /// </returns>
        private static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> mover)
        {
            for (; mover.MoveNext();)
            {
                yield return mover.Current;
            }
        }

        /// <summary>
        /// Get a candidate for the majority element in an enumeration.
        /// This is an element such that no other could possibly be the
        /// majority.
        /// </summary>
        /// <remarks>
        /// Below examples are to help convince of correctness.
        /// 
        /// Ex. 1
        /// 
        ///     [ A ]
        /// 
        ///     =======================
        ///       i | majCnt | majVal
        ///     =======================
        ///       0 |   1    |   A
        ///    
        /// Ex. 2
        ///     
        ///     [ A, B ] 
        /// 
        ///     =======================
        ///       i | majCnt | majVal
        ///     =======================
        ///       0 |   1    |   A
        ///     -----------------------
        ///       1 |   0    |   A
        ///       
        ///  Ex. 3
        ///       
        ///     [ A, B, C ] 
        ///  
        ///     =======================
        ///       i | majCnt | majVal
        ///     =======================
        ///       0 |   1    |   A
        ///     -----------------------
        ///       1 |   0    |   A
        ///     -----------------------
        ///       2 |   1    |   B
        ///       
        ///  Ex. 4
        ///       
        ///     [ A, A, B ] 
        ///  
        ///     =======================
        ///       i | majCnt | majVal
        ///     =======================
        ///       0 |   1    |   A
        ///     -----------------------
        ///       1 |   2    |   A
        ///     -----------------------
        ///       2 |   1    |   A
        ///       
        ///  Ex. 5
        ///       
        ///     [ A, B, B ] 
        ///  
        ///     =======================
        ///       i | majCnt | majVal
        ///     =======================
        ///       0 |   1    |   A
        ///     -----------------------
        ///       1 |   0    |   A
        ///     -----------------------
        ///       2 |   1    |   B
        ///       
        ///  Ex. 6
        ///       
        ///     [ A, B, A ] 
        ///  
        ///     =======================
        ///       i | majCnt | majVal
        ///     =======================
        ///       0 |   1    |   A
        ///     -----------------------
        ///       1 |   0    |   A
        ///     -----------------------
        ///       2 |   1    |   A
        ///       
        ///  Ex. 7
        ///       
        ///     [ A, B, B, C, B ] 
        ///  
        ///     =======================
        ///       i | majCnt | majVal
        ///     =======================
        ///       0 |   1    |   A
        ///     -----------------------
        ///       1 |   0    |   A
        ///     -----------------------
        ///       2 |   1    |   B
        ///     -----------------------
        ///       3 |   0    |   B
        ///     -----------------------
        ///       4 |   1    |   B
        ///
        ///  Ex. 8
        ///       
        ///     [ A, A, B, C, A, B ] 
        ///  
        ///     =======================
        ///       i | majCnt | majVal
        ///     =======================
        ///       0 |   1    |   A
        ///     -----------------------
        ///       1 |   2    |   A
        ///     -----------------------
        ///       2 |   1    |   A
        ///     -----------------------
        ///       3 |   0    |   A
        ///     -----------------------
        ///       4 |   1    |   B
        ///       
        ///  The majVal at the end cannot be any value other than what
        ///  is POSSIBLY the majority. It's the only candidate.
        ///  
        ///  Here is an informal "proof".
        ///  
        ///  Think of the subenumeration as being "A and other elements":
        ///  
        ///  [ A, A, ?, ?, A, ?, A, A, ?]
        ///    |        |              |
        ///   i=0      i=3            i=8
        ///            (A-0)          (A-1)
        /// 
        ///  When we get to i=3, we know that no element in the range [0,3]
        ///  is the majority IN THAT RANGE. 
        ///  
        ///  When we get to i=8, we know that A is the majority in the
        ///  range [4, 8].
        ///  
        ///  Could some other element besides A be the majority? No, because
        ///  to outnumber A it would need to be the majority over [0,3].
        ///  
        ///  Q.E.D.
        ///       
        /// </remarks>
        /// <param name="source">The enumeration to check for majority candidate</param>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <returns>
        /// The element which has the only possibility of being the majority,
        /// or <see cref="null"/> if there is no such element.
        /// </returns>
        private static T? MajorityElementCandidateOrNull<T>(this IEnumerable<T> source)
        {
            T majVal = default(T);
            int majCount = 0;
            Func<T, T, bool> equals = EqualityFactory.GetBestEqualityExpression<T>();

            foreach (T t in source)
            {
                if (majCount == 0)
                {
                    majVal = t;
                    ++majCount;
                }
                else
                {
                    if (equals(t, majVal))
                        ++majCount;
                    else
                        --majCount;
                }
            }
            return majVal;
        }

        /// <summary>
        /// Count elements in an enumeration matching a predicate, while
        /// also keeping track of the total number of elements.
        /// </summary>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <param name="source">The enumeration</param>
        /// <param name="pred">The predicate to count matching elements</param>
        /// <param name="totalItems">The total number of elements in the enumeration</param>
        /// <returns>
        /// The number of elements satisfying <paramref name="pred"/>
        /// </returns>
        private static int Count<T>(this IEnumerable<T> source, Predicate<T> pred, out int totalItems)
        {
            int count = 0;
            totalItems = source.AsParallel().Count((t) =>
            {
                if (pred(t))
                {
                    Interlocked.Increment(ref count);
                }

                return true;
            });
            return count;
        }

        /// <summary>
        /// Check whether enumeration has it's number of elements readily
        /// available, as is assumed if it has a property such as <see cref="ICollection{T}.Count"/>.
        /// This is an optimization in case <see cref="Enumerable.Count{TSource}(IEnumerable{TSource})"/>
        /// might enumerate all the elements when we want the count ONLY IF
        /// we don't have to enumeration the elements.
        /// </summary>
        /// <typeparam name="T">The type of elements in the enumeration</typeparam>
        /// <param name="source">The enumeration</param>
        /// <param name="count">The count of elements in the enumeration, if method returns <see cref="true"/></param>
        /// <returns>
        /// <see cref="true"/> if <paramref name="count"/> provided, <see cref="false"/> if not
        /// </returns>
        private static bool HasCountValueStored<T>(this IEnumerable<T> source, out int count)
        {
            if (source is ICollection<T> col)
            {
                count = col.Count;
                return true;
            }
            else if (source is T[] arr)
            {
                count = arr.Length;
                return true;
            }
            else
            {
                count = -1;
                return false;
            }
        }

        private static void NormalizeShifts<T>(this IEnumerable<T> source, ref int shifts, out int count)
        {
            count = source.Count();
            shifts = shifts % count;
            if (shifts < 0) // e.g. shift -2 same as shift +3 for enumeration of length 5
                shifts = count + shifts;
        }

        /// <summary>
        /// Trim leading and trailing items from an enumeration
        /// </summary>
        /// <param name="source">The enumeration</param>
        /// <param name="items">The items to trim</param>
        /// <typeparam name="T">The type of items</typeparam>
        /// <returns>
        /// The trimmed enumeration
        /// </returns>
        private static IEnumerable<T> TrimInner<T>(this IEnumerable<T> source, params T[] items)
        {
            var set = items.ToImmutableHashSet();
            // TODO: Make more memory-efficient as Reverse() might put all in to memory
            //       even if its an array or list
            return source
                .TrimLeft(set).Reverse()
                .TrimLeft(set).Reverse();
        }

        /// <summary>
        /// Trim leading items from an enumeration
        /// </summary>
        /// <param name="source">The enumeration</param>
        /// <param name="items">The items to trim</param>
        /// <typeparam name="T">The type of items in the enumeration</typeparam>
        /// <returns>
        /// The trimmed enumeration
        /// </returns>
        private static IEnumerable<T> TrimLeft<T>(this IEnumerable<T> source, IImmutableSet<T> items)
        {
            return source.SkipWhile(t => items.Contains(t));
        }

        #endregion
    }
}