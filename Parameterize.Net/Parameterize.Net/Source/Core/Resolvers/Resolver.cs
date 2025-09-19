using System;
using System.Collections.Generic;

namespace Parameterize.Net
{
    /// <summary>
    /// The property to index mapping of a data structur
    /// </summary>
    public abstract class Resolver : IParameterizeSerialize
    {
        public abstract int Length { get; }

        public T Resolve<T>(Span<float> value)
        {
            return (T)Resolve(typeof(T), value);
        }

        public abstract object Resolve(Type type, Span<float> value);

        public Range[] GetRange()
        {
            var ret = new Range[Length];
            GetRange(ret);
            return ret;
        }
        public abstract void GetRange(Span<Range> rangeSpan);

        public float[] Get(object representative)
        {
            var ret = new float[Length];
            Get(ret, representative);
            return ret;
        }
        public abstract void Get(Span<float> range, object representative);
    }

    public struct Range
    {
        public float Min, Max;

        public Range(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public float Clamp(float x)
        {

            if (Min > Max)
                return Math.Clamp(x, Max, Min);
            return Math.Clamp(x, Min, Max);
        }

        public float Lerp(float t)
        {
       

            return Clamp(Min + (t * (Max - Min)));
        }

        public Range Combine(Range other)
        {
            return new Range(Math.Min(Min, other.Min), Math.Max(Max, other.Max));
        }
    }

    internal interface ISubResolverHolder
    {
        public List<SubResolver> SubResolvers { get; }
    }

    public static class ParameterizeExtensions
    {
        public static float[] GetRandom(this Range[] range, Random r=null)
        {
            r ??= new Random();
            var ret = new float[range.Length];
            for (int i = 0; i < range.Length; i++)
            {
                ret[i] = range[i].Lerp((float)r.NextDouble());
            }

            return ret;
        }
    }
}