namespace Parameterize.Net.Builder
{
    public class ArrayPropertyControlStep<T, TElement> : ModelSelectedStep<T>
    {
        public Resolver ElementResolver { get; set; }
        internal ArrayPropertyControlStep(Accessor accessor, Resolver resolver, Resolver elementResolver, int minLength, int maxLength)
        {
            this.ElementResolver = elementResolver;
            this.Resolver = resolver;
            this.ArrayResolver = new ArrayResolver(elementResolver, minLength, maxLength);
            if (resolver is ISubResolverHolder h)
            {
                h.SubResolvers.Add(new SubResolver
                {
                    Accessor = accessor,
                    Resolver = ArrayResolver
                });
            }
        }
    
    
        public ArrayResolver ArrayResolver { get; set; }
    }
}