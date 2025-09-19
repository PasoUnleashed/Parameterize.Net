using System;
using System.Collections.Generic;
using System.Linq;

namespace Parameterize.Net
{
    /// <summary>
    /// The genotype of a data strucuture
    /// </summary>
    internal class SingleTypeResolver : Resolver, ISubResolverHolder
    {
        public List<SubResolver> SubResolvers { get; set; } = new List<SubResolver>();

        public override int Length => SubResolvers.Sum(i => i.Resolver.Length);

        public override object Resolve(Type type, Span<float> value)
        {
            var instance = Activator.CreateInstance(type);
            int index = 0;
            foreach (var subGenotype in SubResolvers)
            {
                try
                {
                    var subType = subGenotype.Accessor.GetAccessedType(instance);
                    var subInstance =
                        subGenotype.Resolver.Resolve(subType, value.Slice(index, subGenotype.Resolver.Length));
                    subGenotype.Accessor.Set(instance, subInstance);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                }

                index += subGenotype.Resolver.Length;
            }

            return instance;
        }

        public override void GetRange(Span<Range> rangeSpan)
        {
            int index = 0;

            foreach (var resolver in SubResolvers)
            {
                resolver.Resolver.GetRange(rangeSpan.Slice(index, resolver.Resolver.Length));
                index += resolver.Resolver.Length;
            }
        }

        public override void Get(Span<float> range, object representative)
        {
            int index = 0;
            foreach (var subGenotype in SubResolvers)
            {
                subGenotype.Resolver.Get(range.Slice(index, subGenotype.Resolver.Length),
                    subGenotype.Accessor.Get(representative));
                index += subGenotype.Resolver.Length;
            }

        }
    }
}