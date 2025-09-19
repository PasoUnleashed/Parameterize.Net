using System;

namespace Parameterize2.Net.Builder
{
    public class MultiTypeStep<T>
    {
        public MultiTypeResolver Resolver;
        public MultiTypeStep<T> ForType<TSub>(Resolver resolver)
        {
            var t = typeof(TSub);
            return ForType(t, resolver);
        }

        public MultiTypeStep<T> ForType(Type tSub,Resolver resolver)
        {
        
            Resolver.SubtypeNames.Add(tSub.VersionOkName());
            Resolver.SubtypeResolvers.Add(tSub.VersionOkName(),new MultiTypeResolverEntry
            {
                Resolver = resolver,
                IsDirectlyUsable = !tSub.IsAbstract
            });
            return this;
        }

        public Resolver Get()
        {
            return Resolver;
        }
    }
}