using System.Collections.Generic;
using System.Xml.XPath;

namespace Parameterize2.Net.Builder
{
    public static class ResolverBuilder
    {
        public static ModelSelectedStep<T> Model<T>()
        {
            return new ModelSelectedStep<T>(){Resolver = new SingleTypeResolver()};
        }

        public static ModelSelectedStep<TSub> Model<TSub>(Resolver baseResolver)
        {
            var res = new SingleTypeExtensionResolver(baseResolver, new());
            return new ModelSelectedStep<TSub>(){Resolver = res};
        }

        public static MultiTypeStep<TBase> Many<TBase>()
        {
            return new MultiTypeStep<TBase>()
            {
                Resolver = new MultiTypeResolver()
            };
        
        }
    }
}