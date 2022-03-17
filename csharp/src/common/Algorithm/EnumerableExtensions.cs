namespace Algorithm
{
    public static class EnumerableExtensions
    {
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
      
            for (var mover = source.GetEnumerator(); mover.MoveNext(); )
            {
                var first = mover.Current;
                var rest = mover.Move(chunkSize - 1);
                yield return rest.Append(first);
            }
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T t)
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

        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T t)
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

        private static IEnumerable<T> Move<T>(this IEnumerator<T> mover, int moves)
        {
            for ( ; moves > 0 && mover.MoveNext(); )
            {
                yield return mover.Current;
            }
        }
    }
}