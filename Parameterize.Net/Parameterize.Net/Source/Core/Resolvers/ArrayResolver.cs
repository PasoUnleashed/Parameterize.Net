using System;

namespace Parameterize.Net
{
    public class ArrayResolver : Resolver
    {
        public Resolver ElementResolver;

        public override int Length => ((ElementResolver.Length + 1) * (MaxLength - MinLength))+(ElementResolver.Length* MinLength);
        public int MinLength;
        public int MaxLength;

        public ArrayResolver(Resolver elementResolver,int minLength, int maxLength)
        {
            
            ElementResolver = elementResolver;
            MinLength = minLength;
            MaxLength = maxLength;
            this.ToggleResolver = new BasicNumericResolver();
            ToggleResolver.Min = -1;
            ToggleResolver.Max = 1;
        }

        public BasicNumericResolver ToggleResolver { get; set; }

        public override object Resolve(Type type, Span<float> value)
        {
            Span<float> minSpan = value.Slice(0, ElementResolver.Length * MinLength);
            Span<float> toggleSpan = value.Slice(ElementResolver.Length * MinLength,
                (ElementResolver.Length + 1) * (MaxLength - MinLength));
            int toggleableLength = 0;
            for (int i = 0; i < toggleSpan.Length; i += ElementResolver.Length + 1)
            {
                toggleableLength += ToggleResolver.Resolve<bool>(toggleSpan.Slice(i, 1)) ? 1 : 0;
            }

            var elementType = type.GetElementType();
            var res = Array.CreateInstance(elementType,MinLength+toggleableLength);
            int index = 0;
            for (int i = 0; i < minSpan.Length; i+=ElementResolver.Length)
            {
                res.SetValue(ElementResolver.Resolve(elementType,minSpan.Slice(i,ElementResolver.Length)),index);
                index += 1;
            }

            for (int i = 0; i < toggleSpan.Length; i += ElementResolver.Length + 1)
            {
                if (!ToggleResolver.Resolve<bool>(toggleSpan.Slice(i, 1)))
                {
                    continue;
                }
                res.SetValue(ElementResolver.Resolve(elementType,toggleSpan.Slice(i+1,ElementResolver.Length)),index);
                index += 1;
            }

            return res;
        }

        public override void GetRange(Span<Range> rangeSpan)
        {
            for (int i = 0; i < MinLength; i+=ElementResolver.Length)
            {
                ElementResolver.GetRange(rangeSpan.Slice(i,ElementResolver.Length));
            }

            for (int i = ElementResolver.Length * MinLength; i < this.Length; i += ElementResolver.Length + 1)
            {
                rangeSpan[i] = new Range(-1, 1);
                ElementResolver.GetRange(rangeSpan.Slice(i+1,ElementResolver.Length));
            }
        }

        public override void Get(Span<float> span,object array)
        {
            int index = 0;
            for (int i = 0; i < MinLength; i+=ElementResolver.Length)
            {
                ElementResolver.Get(span.Slice(i,ElementResolver.Length),(array as Array).GetValue(index));
                index += 1;
            }

            for (int i = ElementResolver.Length * MinLength; i < this.Length; i += ElementResolver.Length + 1)
            {
            
                ToggleResolver.Get(span.Slice(i,1),array);
                if (ToggleResolver.Resolve<bool>(span.Slice(i, 1)))
                {
                    ElementResolver.Get(span.Slice(i + 1, ElementResolver.Length), index);
                    index += 1;
                }
            }
        }
    }
}