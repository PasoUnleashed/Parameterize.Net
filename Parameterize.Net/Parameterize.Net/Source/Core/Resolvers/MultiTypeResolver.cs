using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Parameterize2.Net
{
    public class MultiTypeResolverEntry
    {
        public Resolver Resolver;
        public bool IsDirectlyUsable;
    }
    public class MultiTypeResolver : Resolver
    {
        public Dictionary<string,MultiTypeResolverEntry> SubtypeResolvers=new();
        public List<string> SubtypeNames=new();

        public override int Length => 1+SubtypeResolvers.Max(i=>i.Value.Resolver.Length);
        [JsonIgnore]
        public BasicNumericResolver TypeIndexResolver { get; set; }

        public MultiTypeResolver()
        {
            
        }

        public MultiTypeResolver(Dictionary<Type, Resolver> resolvers)
        {
            SubtypeNames = resolvers.Select(i => i.Key.VersionOkName()).ToList();
            SubtypeResolvers =
                new Dictionary<string, MultiTypeResolverEntry>(resolvers.Select(i => new KeyValuePair<string, MultiTypeResolverEntry>( i.Key.VersionOkName(), new MultiTypeResolverEntry
                {
                    Resolver = i.Value ,
                    IsDirectlyUsable = !i.Key.IsAbstract
                })));
        }


        public override object Resolve(Type type, Span<float> value)
        {
            if (TypeIndexResolver == null)
            {
                this.TypeIndexResolver = new BasicNumericResolver() { Min = 0, Max = SubtypeNames.Count - 1 };
            }
            var instanceTypeName = SubtypeNames[TypeIndexResolver.Resolve<int>(value)];
            var instanceType = Type.GetType(instanceTypeName);

            var resolver = SubtypeResolvers[instanceTypeName];
            return resolver.Resolver.Resolve(instanceType, value.Slice(1, resolver.Resolver.Length));
        }

        public override void GetRange(Span<Range> rangeSpan)
        {
       
            Range[] rel = new Range[rangeSpan.Length];
            for (int i = 0; i < rangeSpan.Length; i++)
            {
                rel[i] = new Range(float.MaxValue, float.MinValue);
            }
            foreach (var resolversValue in SubtypeResolvers.Values)
            {
                resolversValue.Resolver.GetRange(rangeSpan.Slice(1));
                for (int i = 0; i < rangeSpan.Length; i++)
                {
                    rel[i] = rangeSpan[i].Combine(rel[i]);
                }
            }
            for (int i = 0; i < rangeSpan.Length; i++)
            {
                rangeSpan[i] = rel[i];
            }

            rangeSpan[0] = new Range(0, SubtypeNames.Count);
        }

        public override void Get(Span<float> range, object representative)
        {
            if (TypeIndexResolver == null)
            {
                this.TypeIndexResolver = new BasicNumericResolver() { Min = 0, Max = SubtypeNames.Count - 1 };
            }
            var instanceTypeNameIndex = SubtypeNames.IndexOf(representative.GetType().VersionOkName());
            range[0] = instanceTypeNameIndex;
            SubtypeResolvers[SubtypeNames[instanceTypeNameIndex]].Resolver.Get(range.Slice(1),representative);
        }
    }

    public static class TypeNameExtensions
    {
        public static string VersionOkName(this Type type)
        {
            var versionOkName = type.AssemblyQualifiedName.Replace(type.Assembly.GetName().Version.ToString(), "0.0.0.0");
            return versionOkName;
        }
    }
}