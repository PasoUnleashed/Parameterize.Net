using System;
using System.Reflection;

namespace Parameterize.Net
{
    internal class PropertyAccessor : Accessor
    {
        public string PropertyName { get; private set; }

        public PropertyAccessor(string name)
        {
            this.PropertyName = name;
        }
        public override Type GetAccessedType(object target)
        {
            var propertyInfo = target.GetType().GetProperty(this.PropertyName,
                BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (propertyInfo == null)
            {
                throw new Exception($"Property not found ({PropertyName}) on target of type {target.GetType().Name}");
            }

            return propertyInfo.PropertyType;
        }

        public override object Get(object target)
        {
            var propertyInfo = target.GetType().GetProperty(this.PropertyName,
                BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (propertyInfo == null)
            {
                throw new Exception($"Property not found ({PropertyName}) on target of type {target.GetType().Name}");
            }

            return propertyInfo
                .GetValue(target);
        }

        public override void Set(object target, object value)
        {
            var propertyInfo = target.GetType().GetProperty(this.PropertyName,
                BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (propertyInfo == null)
            {
                throw new Exception($"Property not found ({PropertyName}) on target of type {target.GetType().Name}");
            }

            propertyInfo.SetValue(target, value);
        }
    }
}