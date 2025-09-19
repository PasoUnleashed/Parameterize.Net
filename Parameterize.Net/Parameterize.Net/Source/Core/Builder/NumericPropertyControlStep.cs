using System;

namespace Parameterize2.Net.Builder
{
    public class NumericPropertyControlStep<T, TPropType> : ModelSelectedStep<T>
    {
        private readonly BasicNumericResolver _basicNumericResolver;

        internal NumericPropertyControlStep(Accessor accessor, Resolver resolver)
        {
            Resolver = resolver;
            if (resolver is ISubResolverHolder s)
            {
                this. _basicNumericResolver = new BasicNumericResolver();
                s.SubResolvers.Add(new SubResolver()
                {
                    Accessor = accessor,
                    Resolver = _basicNumericResolver
                });
            }
        }

        public RangeSetNumericPropertyControlStep<T, TPropType> WithRange(TPropType min, TPropType max)
        {
            return new RangeSetNumericPropertyControlStep<T, TPropType>(this.Resolver, _basicNumericResolver,
                (float)Convert.ChangeType(min, typeof(float)), (float)Convert.ChangeType(max, typeof(float)));
        }
    }
}