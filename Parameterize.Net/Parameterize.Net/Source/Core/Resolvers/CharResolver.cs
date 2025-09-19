using System;

namespace Parameterize2.Net
{
    public class CharResolver : Resolver
    {
        public CharResolver(char[] charSet)
        {
            
            this.CharSet = charSet;
        }

        public char[] CharSet { get; set; }

        public override int Length { get; } = 1;
        public override object Resolve(Type type, Span<float> value)
        {
            int index =(int) Math.Clamp(value[0], 0, CharSet.Length);
            return CharSet[index];
        }

        public override void GetRange(Span<Range> rangeSpan)
        {
            rangeSpan[0] = new Range(0,CharSet.Length+0.999999f);
        }

        public override void Get(Span<float> range, object representative)
        {
            range[0] = Array.IndexOf(CharSet, (char)representative);
        }
    }
}