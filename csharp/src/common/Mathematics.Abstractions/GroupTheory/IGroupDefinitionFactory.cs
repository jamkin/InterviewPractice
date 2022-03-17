namespace Mathematics.Abstractions.GroupTheory
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IGroupDefinitionFactory
    {
        bool TryGetDefinition<T>(out IGroupDefinition<T> definition) where T : IEquatable<T>;
    }
}
