namespace Parameterize.Net.Builder
{
    public class RangeSetNumericPropertyControlStep<T, TPropType> : ModelSelectedStep<T>
    {
        public float Min;
        public float Max;

        internal BasicNumericResolver NumericResolver;

        public RangeSetNumericPropertyControlStep(Resolver parentResolver, BasicNumericResolver numericNumericResolver,
            float min, float max)
        {
            this.Resolver = parentResolver;
            this.NumericResolver = numericNumericResolver;
            NumericResolver.Min = min;
            NumericResolver.Max = max;
            Min = min;
            Max = max;
        }
    }
}