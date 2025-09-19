using System;

namespace Parameterize.Net
{
    /// <summary>
    /// The accessor of a property
    /// </summary>
    internal abstract class Accessor : IParameterizeSerialize
    {
        public abstract Type GetAccessedType(object target);
        public abstract object Get(object target);

        public abstract void Set(object target, object value);
    }
}