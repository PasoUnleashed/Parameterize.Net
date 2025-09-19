using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Parameterize2.Net.Builder;

namespace Parameterize2.Net.Reflection
{
    public class ResolverDeriver
    {
        public static Resolver Derive<T>()
        {
            var res = new SingleTypeResolver(){SubResolvers =(GetSubResolvers(typeof(T),false).ToList())};
            return res;
        }

        private static Type[] _numericTypes = new[]
        {
            typeof(uint), typeof(ulong), typeof(ushort), typeof(byte), typeof(int), typeof(short), typeof(long),
            typeof(double), typeof(float)
        };
        private static Resolver CreateResolver(Type memberType,MemberInfo memberInfo)
        {
            var resolverAttribute = memberInfo.GetCustomAttribute<ResolverAttribute>();
            if (resolverAttribute != null)
            {
                var instance = Activator.CreateInstance(resolverAttribute.ResolverType) as Resolver;
                return instance;
            }
            if (memberType.IsArray)
            {
                var len = GetLength(memberInfo);
                var ret = new ArrayResolver(CreateResolver(memberType.GetElementType(),memberInfo),len.min,len.max);
                return ret;
            }
            else if (memberType == typeof(string))
            {
                var len = GetLength(memberType);
                var charset = CharSets.GetSet(GetCharset(memberInfo));
                var res = new StringResolver(charset, len.min, len.max);
                return res;
            }
            else if (_numericTypes.Contains(memberType))
            {
                var res = new BasicNumericResolver();
                var range = GetRange(memberInfo);
                res.Min = range.min;
                res.Max = range.max;
                if (memberInfo.GetCustomAttribute<NoClampAttribute>() != null)
                {
                    res.Clamped = false;
                }
                return res;
            }
            else if (memberType == typeof(char))
            {
                return new CharResolver(CharSets.GetSet(GetCharset(memberInfo)));
            }
            else if (memberType == typeof(bool))
            {
                return new BasicNumericResolver();
            }
            else if (memberType.IsClass)
            {
                var baseTypes = GetSubtypes(memberInfo);
                if (baseTypes.Count() == 0)
                {
                    if (memberType.IsAbstract)
                    {
                        throw new Exception(
                            $"Cannot create resolver for member {memberInfo.Name} in class {memberInfo.DeclaringType.FullName} because {memberType.Name} is abstract, please specify concrete subtypes using the Subtype Attribute");
                    }
                    var res = new SingleTypeResolver()
                    {
                        SubResolvers = GetSubResolvers(memberType, false).ToList()
                    };
                    return res;
                }
                var tree = BuildTree(memberType, baseTypes);
                var dict = new Dictionary<Type, Resolver>();
                Queue<TypeResolverTreeNode> nodes = new Queue<TypeResolverTreeNode>();
                nodes.Enqueue(tree);
                while (nodes.TryDequeue(out var curr))
                {
                    if(!curr.Type.IsAbstract)
                        dict[curr.Type] = curr.Holder as Resolver;
                    foreach (var child in curr.Children)
                    {
                        nodes.Enqueue(child);
                    }
                }

                var multiResolver = new MultiTypeResolver(dict);
                return multiResolver;
            }

            throw new Exception($"Could not create resolver for type {memberType.Name}");
        }

        private static IEnumerable<SubResolver> GetSubResolvers(Type t, bool declaredOnly)
        {
        
            foreach (var field in GetParametericFields(t,declaredOnly))
            {
                yield return new SubResolver()
                {
                    Accessor = new FieldAccessor(field.Name),
                    Resolver = CreateResolver(field.FieldType, field)
                };
            }

            foreach (var prop in GetParametericProps(t,declaredOnly))
            {
                yield return new SubResolver()
                {
                    Accessor = new PropertyAccessor(prop.Name),
                    Resolver = CreateResolver(prop.PropertyType, prop)
                };
            }
        }
        private static TypeResolverTreeNode BuildTree(Type rootType,IEnumerable<Type> types)
        {
            int minDepth = int.MaxValue;
            Dictionary<int, List<Type>> depthDict = new Dictionary<int, List<Type>>();
            if(!types.Contains(rootType)) types = types.Append(rootType);
            foreach (var type in types)
            {
                var depth = GetDepth(type);
                if (!depthDict.TryGetValue(depth, out var l))
                {
                    l = new();
                    depthDict[depth] = l;
                }
                l.Add(type);
                minDepth = Math.Min(minDepth, depth);
            }


            if (depthDict.Count == 0)
            {
                throw new Exception($"Invalid subtypes list for type {rootType.Name}");
            }

            if (depthDict[minDepth].Count > 1)
            {
                //throw new Exception($"Invalid subtypes list for type {rootType.Name} multiple roots found");
            }

            List<TypeResolverTreeNode> sortedNodes = new List<TypeResolverTreeNode>();
            Stack<Type> typeStack = new Stack<Type>();
        
        
            foreach (var type in types.OrderByDescending(GetDepth))
            {
                typeStack.Push(type);
            }
            while (typeStack.TryPop(out var res))
            {
                var depth = GetDepth(res);
                if (sortedNodes.Count == 0)
                {
                    sortedNodes.Add(new TypeResolverTreeNode(res,new SingleTypeResolver
                    {
                        SubResolvers = GetSubResolvers(res,false).ToList()
                    },depth));
                    sortedNodes.Sort((i,j)=>i.Depth-j.Depth);
                }
                else
                {
                    foreach (var sortedNode in sortedNodes)
                    {
                        if (sortedNode.Type.IsAssignableFrom(res))
                        {
                            var typeResolverTreeNode = new TypeResolverTreeNode(res,new SingleTypeExtensionResolver(sortedNode.Holder as Resolver, GetSubResolvers(res,true).ToList()),depth);
                            sortedNodes.Add(typeResolverTreeNode);
                            sortedNodes.Sort((i,j)=>i.Depth-j.Depth);
                            sortedNode.Children.Add(typeResolverTreeNode);
                            break;
                        }
                    }
                }
            }

            return sortedNodes.First();

        }
    
        private class TypeResolverTreeNode
        {
            public List<TypeResolverTreeNode> Children = new();
            public ISubResolverHolder Holder { get; set; }
            public Type Type { get; set; }
            public int Depth { get; set; }
            public TypeResolverTreeNode(Type type, ISubResolverHolder holder,int Depth)
            {
                this.Type = type;
                this.Holder = holder;
            }
        
        
        

        }
        private static int GetDepth(Type type)
        {
            if (type == typeof(object) || type == null)
            {
                return 0;
            }

            else
            {
                return GetDepth(type.BaseType) + 1;
            }
        }

        private static IEnumerable<PropertyInfo> GetParametericProps(Type type,bool declaredOnly=false)
        {
            foreach (var prop in type.GetProperties(BindingFlags.NonPublic|BindingFlags.Public| BindingFlags.Instance | BindingFlags.Instance | (declaredOnly? BindingFlags.DeclaredOnly : default)))
            {
                if (IsParameterizedMember(prop))
                {
                    yield return prop;
                }
            }
        }
        private static IEnumerable<FieldInfo> GetParametericFields(Type type,bool declaredOnly=false)
        {
            foreach (var prop in type.GetFields(BindingFlags.NonPublic|BindingFlags.Public| BindingFlags.Instance | BindingFlags.Instance | (declaredOnly? BindingFlags.DeclaredOnly : default)))
            {
                if (IsParameterizedMember(prop))
                {
                    yield return prop;
                }
            }
        }
    

        private static bool IsParameterizedMember(MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttributes()
                .Any(i => i is ResolverAttribute||i is ParameterizeAttribute || i is RangeAttribute || i is LengthAttribute || i is SubtypeAttribute) &&( memberInfo is FieldInfo || memberInfo is PropertyInfo);
        }

        private static (int min, int max) GetLength(MemberInfo memberInfo)
        {
            var customAttribute = memberInfo.GetCustomAttribute<LengthAttribute>();
            if (customAttribute != null)
            {
                return (customAttribute.MinLength,customAttribute.MaxLength);
            }
            else
            {
                return (0, 10);
            }
        }

        private static IEnumerable<Type> GetSubtypes(MemberInfo memberInfo)
        {
            foreach (var attribute in memberInfo.GetCustomAttributes())
            {
                if (attribute is SubtypeAttribute s)
                {
                    yield return s.Subtype;
                }
            }
        }
        private static (float min, float max) GetRange(MemberInfo memberInfo)
        {
            var customAttribute = memberInfo.GetCustomAttribute<RangeAttribute>();
            if (customAttribute != null)
            {
                return (customAttribute.MinValue,customAttribute.MaxValue);
            }
            else
            {
                return (0, 1);
            }
        }

        private static Charset GetCharset(MemberInfo memberInfo)
        {
            var charsetAttribute = memberInfo.GetCustomAttribute<CharsetAttribute>();
            if (charsetAttribute == null)
            {
                return Charset.AlphaNum;
            }

            else
            {
                return charsetAttribute.Set;
            }
        }
    }

    public class ParameterizeAttribute : System.Attribute
    {
    
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]

    public class CharsetAttribute : System.Attribute
    {
        public CharsetAttribute(Charset set)
        {
            this.Set = set;
        }

        public Charset Set { get; set; }
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false)]
    public class RangeAttribute : System.Attribute
    {
        public float MinValue;
        public float MaxValue;
        public RangeAttribute(float minValue, float maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public RangeAttribute(double minValue, double maxValue)
        {
            MinValue =(float) minValue;
            MaxValue=(float) maxValue;
        }

        public RangeAttribute(long minValue, long maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public RangeAttribute(ulong minValue, ulong maxValue)
        {
            MinValue = (float)minValue;
            MaxValue = (float)maxValue;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false)]
    public class LengthAttribute : System.Attribute
    {
        public int MinLength;
        public int MaxLength;

        public LengthAttribute(int minLength, int maxLength)
        {
            this.MinLength = minLength;
            this.MaxLength = maxLength;
        }

    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = true)]
    public class SubtypeAttribute : System.Attribute
    {
        public Type Subtype { get; set; }
        public SubtypeAttribute(Type subtype)
        {
            this.Subtype = subtype;
        }

    }

    public class ResolverAttribute : System.Attribute
    {
        public Type ResolverType;

        public ResolverAttribute(Type resolverType)
        {
            ResolverType = resolverType;
        }
    }

    public class NoClampAttribute :System.Attribute
    {
    
    }
}