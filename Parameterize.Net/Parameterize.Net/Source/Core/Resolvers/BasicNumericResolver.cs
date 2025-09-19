using System;

namespace Parameterize.Net
{
    /// <summary>
    /// Basic numberic data type resolver (float, int, bool, char, etc...)
    /// </summary>
    public class BasicNumericResolver : Resolver
    {
        public override int Length { get; } = 1;
        public float Min=float.NegativeInfinity;
        public float Max= float.PositiveInfinity;

        public bool Clamped = true;
        internal BasicNumericResolver()
        {
        
        }

        public static BasicNumericResolver Create<T>(T min, T max)
        {
            return new BasicNumericResolver()
            {
                Min = (float)Convert.ChangeType(min, typeof(float)),
                Max = (float)Convert.ChangeType(max, typeof(float))
            };
        }

        public static BasicNumericResolver Create<T>()
        {
            return new();
        }
        public override object Resolve(Type type, Span<float> value)
        {
            var val = value[0];
            if(Clamped) val = Math.Clamp(value[0],this.Min,this.Max);
            if (type == typeof(float))
            {
                return (float)val;
            }
            else if (type == typeof(int))
            {
                val = Math.Clamp(val, int.MinValue, int.MaxValue);
                return (int)val;
            }
            else if (type == typeof(uint))
            {
                val = Math.Clamp(val, uint.MinValue, uint.MaxValue);
                return (uint)val;
            }
            else if (type == typeof(short))
            {
                
                val = Math.Clamp(val, short.MinValue, short.MaxValue);
                return (short)val;
            }
            else if (type == typeof(ushort))
            {
                val = Math.Clamp(val, ushort.MinValue, ushort.MaxValue);
             
                return (ushort)val;
            }
            else if (type == typeof(long))
            {
                val = Math.Clamp(val, long.MinValue, long.MaxValue);
                return (long)val;
            }
            else if (type == typeof(ulong))
            {
                val = Math.Clamp(val, ulong.MinValue, ulong.MaxValue);
                return (ulong)val;
            }
            else if (type == typeof(bool))
            {
                val = value[0];
                return val >= 0;
            }
            else if (type == typeof(double))
            {
                
                return (double)val;
            }
            else if (type == typeof(byte))
            {
                val = Math.Clamp(val, byte.MinValue, byte.MaxValue);
                return (byte)val;
            }

            throw new Exception($"Type {type.Name} is not handled by resolver of type {this.GetType().Name}");
        }

        public override void GetRange(Span<Range> rangeSpan)
        {
            rangeSpan[0] = new Range(float.IsNegativeInfinity(this.Min) ? float.MinValue : Min,
                float.IsPositiveInfinity(Max) ? float.MaxValue : Max);
        }

        public override void Get(Span<float> range, object representative)
        {
            range[0] = (float)Convert.ChangeType(representative, typeof(float));
        }
    }
}