using System;

namespace Parameterize2.Net.Builder
{
    public class NumericArrayPropertyControlStep<T,TElement> : ArrayPropertyControlStep<T,TElement>
    {
        internal NumericArrayPropertyControlStep(Accessor accessor,Resolver resolver, int minLength, int maxLength) : base(accessor,resolver,new BasicNumericResolver(),minLength,maxLength)
        {
        
        }

        public NumericArrayPropertyControlStep<T,TElement> WithRange(TElement min, TElement max)
        {
            (this.ElementResolver as BasicNumericResolver).Min =(float) Convert.ChangeType(min, typeof(float));
            (this.ElementResolver as BasicNumericResolver).Max=(float) Convert.ChangeType(max, typeof(float));
            return this;
        }

        public NumericArrayPropertyControlStep<T, TElement> NoClamp()
        {
            (this.ElementResolver as BasicNumericResolver).Clamped = false;
            return this;
        }
    }
}