using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Parameterize2.Net
{
    public class ParameterizeConverter : JsonConverter
    {
        private HashSet<string> _assemblies = new HashSet<string>();

        private const string PlaceHolder = "<TypeName>";

        private ConcurrentDictionary<Guid, Resolver> _resolversCache = new();
        private ConcurrentDictionary<Resolver, Guid> _reverseResolversCache = new();

        private bool SkipOne = false;
        
        public void RegisterAssembly(Type type)
        {
            _assemblies.Add(type.AssemblyQualifiedName.Replace(type.Name, PlaceHolder));
        }

        bool TryGetType(string name, out Type t)
        {
            t = null;
            foreach (var assembly in _assemblies)
            {
                t = Type.GetType(assembly.Replace(PlaceHolder, name));
                if (t != null)
                {
                    return true;
                }
            }

            return false;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            RegisterAssembly(value.GetType());

            if (value is Resolver r && _reverseResolversCache.TryGetValue(r, out var g))
            {
                JObject n = new JObject();
                n["Ref"] = g.ToString();
                n.WriteTo(writer);
                return;
            }
            SkipOne = true;
            JObject j = JObject.FromObject(value,serializer);
            j["Type"] = value.GetType().Name;
            if (value is Resolver res)
            {
                var newGuid = Guid.NewGuid();
                _resolversCache[newGuid] = res;
                _reverseResolversCache[res] = newGuid;
                j["Guid"] = newGuid.ToString();
            }

            j.WriteTo(writer);
            
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            RegisterAssembly(objectType);
            
            var obj = JObject.Load(reader);
            if (obj.ContainsKey("Ref") && _resolversCache.TryGetValue(Guid.Parse(obj["Ref"].ToString()),out var res))
            {
                SkipOne = false;
                return res;
            }
            if (TryGetType(obj["Type"].Value<string>(), out Type type))
            {
                SkipOne = true;
                return obj.ToObject(type, serializer);
            }
            else
            {
                
                return null;
            }
        }


        public override bool CanConvert(Type objectType)
        {
            if (SkipOne)
            {
                SkipOne = false;
                return false;
                
            }

            var isAssignableFrom = typeof(IParameterizeSerialize).IsAssignableFrom(objectType);
            return isAssignableFrom;
        }
    }
}