namespace Algorithm.Decorations
{
    /// <summary>
    /// Attribute to tag a method with different implementation styles.
    /// 
    /// For example, an implementation might be efficient but naturally
    /// difficult to read compared to a less efficient algorithm that
    /// is more fluent. In that case, it might be tagged with
    /// <see cref="AlgorithmImplementationStyle.MicroOptimized"/> but
    /// not <see cref="AlgorithmImplementationStyle.Elegant"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class AlgorithmImplementationStyleAttribute : Attribute
    {
        /// <summary>
        /// The algorithm implementation style
        /// </summary>
        public AlgorithmImplementationStyle Flavor { get; set; }

        /// <summary>
        /// The constructor which sets the <see cref="AlgorithmImplementationStyle"/>
        /// </summary>
        /// <param name="flavor">The <see cref="AlgorithmImplementationStyle"/></param>
        public AlgorithmImplementationStyleAttribute(AlgorithmImplementationStyle flavor)
        {
            this.Flavor = flavor;
        }

        /// <summary>
        /// The default constructor
        /// </summary>
        public AlgorithmImplementationStyleAttribute() : base() { }
    }
}