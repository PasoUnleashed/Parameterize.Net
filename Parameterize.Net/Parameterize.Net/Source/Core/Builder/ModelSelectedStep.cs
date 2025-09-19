using System;
using System.Linq.Expressions;

namespace Parameterize2.Net.Builder
{
    public class ModelSelectedStep<T>
    {
        internal Resolver Resolver { get; set; }

  
        /*public ClassPropertyControlStep<T, TPropType> Property<TPropType>(Expression<Func<T, TPropType>> expression)
            where TPropType : class
        {
            return new ClassPropertyControlStep<T, TPropType>();
        }*/
    

        public ModelSelectedStep<T> SetResolver<TProp>(Expression<Func<T, TProp>> expression,Resolver resolver)
        {
            if (Resolver is ISubResolverHolder h)
            {
                h.SubResolvers.Add(new SubResolver
                {
                    Accessor = ExpressionUtil.CreateAccessor(expression),
                    Resolver = resolver
                });
            }
            return this;
        }
        public NumericPropertyControlStep<T, int> Property(Expression<Func<T, int>> expression)
        {
            return new NumericPropertyControlStep<T, int>(ExpressionUtil.CreateAccessor(expression),this.Resolver);
        }

        public NumericPropertyControlStep<T, uint> Property(Expression<Func<T, uint>> expression)
        {
            return new NumericPropertyControlStep<T, uint>(ExpressionUtil.CreateAccessor(expression),this.Resolver);
        }

        public NumericPropertyControlStep<T, short> Property(Expression<Func<T, short>> expression)
        {
            return new NumericPropertyControlStep<T, short>(ExpressionUtil.CreateAccessor(expression),this.Resolver);
        }

        public NumericPropertyControlStep<T, ushort> Property(Expression<Func<T, ushort>> expression)
        {
            return new NumericPropertyControlStep<T, ushort>(ExpressionUtil.CreateAccessor(expression),this.Resolver);
        }


        public NumericPropertyControlStep<T, ulong> Property(Expression<Func<T, ulong>> expression)
        {
            return new NumericPropertyControlStep<T, ulong>(ExpressionUtil.CreateAccessor(expression),Resolver);
        }

        public NumericPropertyControlStep<T, long> Property(Expression<Func<T, long>> expression)
        {
            return new NumericPropertyControlStep<T, long>(ExpressionUtil.CreateAccessor(expression),Resolver);
        }

        public NumericPropertyControlStep<T, double> Property(Expression<Func<T, double>> expression)
        {
            return new NumericPropertyControlStep<T, double>(ExpressionUtil.CreateAccessor(expression),Resolver);
        }
        public NumericPropertyControlStep<T, float> Property(Expression<Func<T, float>> expression)
        {
            return new NumericPropertyControlStep<T, float>(ExpressionUtil.CreateAccessor(expression),Resolver);
        }
        public NumericPropertyControlStep<T, byte> Property(Expression<Func<T, byte>> expression)
        {
            return new NumericPropertyControlStep<T, byte>(ExpressionUtil.CreateAccessor(expression),Resolver);
        }

        public ModelSelectedStep<T> Property(Expression<Func<T, bool>> expression)
        {
            (Resolver as ISubResolverHolder)?.SubResolvers.Add(new SubResolver()
            {
                Accessor = ExpressionUtil.CreateAccessor(expression),
                Resolver = new BasicNumericResolver()
            });
            return this;
        }

        public StringPropertyControlStep<T> Property(Expression<Func<T, string>> expression,int minLength,int maxLength,Span<char> charset)
        {
            return new StringPropertyControlStep<T>(ExpressionUtil.CreateAccessor(expression),Resolver,minLength,maxLength,charset);
        }

        public ArrayPropertyControlStep<T, TElement> Property<TElement>(Expression<Func<T, TElement[]>> expression,Resolver elementResolver, int minLength,int maxLength)
        {
            return new ArrayPropertyControlStep<T, TElement>(ExpressionUtil.CreateAccessor(expression),Resolver,elementResolver,minLength,maxLength);
        }
 
        public NumericArrayPropertyControlStep<T, int> Property(Expression<Func<T, int[]>> expression, int minLength,int maxLength)
        {
            return new NumericArrayPropertyControlStep<T, int>(ExpressionUtil.CreateAccessor(expression),Resolver,minLength,maxLength);
        }
        public NumericArrayPropertyControlStep<T, uint> Property(Expression<Func<T, uint[]>> expression, int minLength,int maxLength)
        {
            return new NumericArrayPropertyControlStep<T, uint>(ExpressionUtil.CreateAccessor(expression),Resolver,minLength,maxLength);
        }
        public NumericArrayPropertyControlStep<T, short> Property(Expression<Func<T, short[]>> expression, int minLength,int maxLength)
        {
            return new NumericArrayPropertyControlStep<T, short>(ExpressionUtil.CreateAccessor(expression),Resolver,minLength,maxLength);
        }
        public NumericArrayPropertyControlStep<T, ushort> Property(Expression<Func<T, ushort[]>> expression, int minLength,int maxLength)
        {
            return new NumericArrayPropertyControlStep<T, ushort>(ExpressionUtil.CreateAccessor(expression),Resolver,minLength,maxLength);
        }
        public NumericArrayPropertyControlStep<T, ulong> Property(Expression<Func<T, ulong[]>> expression, int minLength,int maxLength)
        {
            return new NumericArrayPropertyControlStep<T, ulong>(ExpressionUtil.CreateAccessor(expression),Resolver,minLength,maxLength);
        }
        public NumericArrayPropertyControlStep<T, long> Property(Expression<Func<T, long[]>> expression, int minLength,int maxLength)
        {
            return new NumericArrayPropertyControlStep<T, long>(ExpressionUtil.CreateAccessor(expression),Resolver,minLength,maxLength);
        }
        public NumericArrayPropertyControlStep<T, byte> Property(Expression<Func<T, byte[]>> expression, int minLength,int maxLength)
        {
            return new NumericArrayPropertyControlStep<T, byte>(ExpressionUtil.CreateAccessor(expression),Resolver,minLength,maxLength);
        }
        public NumericArrayPropertyControlStep<T, double> Property(Expression<Func<T, double[]>> expression, int minLength,int maxLength)
        {
            return new NumericArrayPropertyControlStep<T, double>(ExpressionUtil.CreateAccessor(expression),Resolver,minLength,maxLength);
        }
        public NumericArrayPropertyControlStep<T, float> Property(Expression<Func<T, float[]>> expression, int minLength,int maxLength)
        {
            return new NumericArrayPropertyControlStep<T, float>(ExpressionUtil.CreateAccessor(expression),Resolver,minLength,maxLength);
        }
    
        public Resolver Get()
        {
            return this.Resolver;
        }
    }
}