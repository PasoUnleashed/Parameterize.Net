using System;

namespace Parameterize2.Net.Builder
{
    public class StringPropertyControlStep<T> : ModelSelectedStep<T>
    {
        public StringResolver StringResolver { get; set; }
        internal StringPropertyControlStep(Accessor accessor, Resolver baseResolverHolder, int minLength, int maxLength, Span<char> charset)
        {
            this.Resolver = baseResolverHolder;
            this.StringResolver = new StringResolver(charset.ToArray(),minLength,maxLength);
            if (Resolver is ISubResolverHolder h)
            {
                h.SubResolvers.Add(new SubResolver()
                {
                    Accessor = accessor,
                    Resolver = StringResolver
                });
            }
        }



    
    }
}