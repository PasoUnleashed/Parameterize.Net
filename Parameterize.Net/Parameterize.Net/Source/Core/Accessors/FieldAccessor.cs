using System;
using System.Reflection;

namespace Parameterize2.Net
{
    internal class FieldAccessor : Accessor
    {
        public string FieldName { get; private set; }

        public FieldAccessor(string fieldName)
        {
            FieldName = fieldName;
        }

        public override Type GetAccessedType(object target)
        {
            var fieldInfo = target.GetType().GetField(this.FieldName,
                BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo == null)
            {
                throw new Exception($"Field not found ({FieldName}) on target of type {target.GetType().Name}");
            }

            return fieldInfo.FieldType;
        }

        public override object Get(object target)
        {
            var fieldInfo = target.GetType().GetField(this.FieldName,
                BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo == null)
            {
                throw new Exception($"Field not found ({FieldName}) on target of type {target.GetType().Name}");
            }

            return fieldInfo
                .GetValue(target);
        }

        public override void Set(object target, object value)
        {
            var fieldInfo = target.GetType().GetField(this.FieldName,
                BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo == null)
            {
                throw new Exception($"Field not found ({FieldName}) on target of type {target.GetType().Name}");
            }

            fieldInfo.SetValue(target, value);
        }

        
    }
}