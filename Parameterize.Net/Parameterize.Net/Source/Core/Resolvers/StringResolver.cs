using System;

namespace Parameterize.Net
{
    public class StringResolver : ArrayResolver
    {
        

        public StringResolver(char[] charSet, int minLength, int maxLength) : base(new CharResolver(charSet),minLength, maxLength)
        {
            
        }
        public override object Resolve(Type type, Span<float> value)
        {
            return new string((char[])base.Resolve(typeof(char[]), value));
        }
    }
}