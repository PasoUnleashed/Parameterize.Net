using System;
using System.Collections.Generic;
using System.Linq;

namespace Parameterize.Net
{
    internal class SingleTypeExtensionResolver : Resolver, ISubResolverHolder
    {
        public Resolver BaseResolver;
        public List<SubResolver> SubResolvers { get; set; } = new();
        public override int Length => BaseResolver.Length + SubResolvers.Sum(i => i.Resolver.Length);

        public SingleTypeExtensionResolver(Resolver baseResolver, List<SubResolver> subResolvers)
        {
            this.BaseResolver = baseResolver;
            this.SubResolvers = subResolvers;
        }


        public override object Resolve(Type type, Span<float> value)
        {
            var ret = BaseResolver.Resolve(type, value);
            var extendedSpan = value.Slice(BaseResolver.Length, SubResolvers.Sum(i => i.Resolver.Length));
            var subIndex = 0;
            for (int i = 0; i < extendedSpan.Length; )
            {
                var sub = SubResolvers[subIndex];
                var slice = extendedSpan.Slice(i,sub.Resolver.Length);
                sub.Accessor.Set(ret, sub.Resolver.Resolve(sub.Accessor.GetAccessedType(ret),slice));
                i += sub.Resolver.Length;
                subIndex += 1;
            }

            return ret;
        }

        public override void GetRange(Span<Range> rangeSpan)
        {
            BaseResolver.GetRange(rangeSpan.Slice(0,BaseResolver.Length));
            int index = BaseResolver.Length;
            foreach (var resolver in SubResolvers)
            {
                resolver.Resolver.GetRange(rangeSpan.Slice(index,resolver.Resolver.Length));
                index += resolver.Resolver.Length;
            }
        }

        public override void Get(Span<float> range, object representative)
        {
            BaseResolver.Get(range,representative);
            var extendedSpan = range.Slice(BaseResolver.Length, SubResolvers.Sum(i => i.Resolver.Length));
            var subIndex = 0;
            for (int i = 0; i < extendedSpan.Length; )
            {
                var sub = SubResolvers[subIndex];
                var slice = extendedSpan.Slice(i,sub.Resolver.Length);
                var rep = sub.Accessor.Get(representative);
                sub.Resolver.Get(slice,rep);
                i += sub.Resolver.Length;
                subIndex += 1;
            }
        }
    }
}