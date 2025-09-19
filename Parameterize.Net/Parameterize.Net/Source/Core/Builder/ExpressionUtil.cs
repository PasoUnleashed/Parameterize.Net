using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Parameterize2.Net.Builder
{
    internal class ExpressionUtil
    {
        internal static Accessor CreateAccessor<Tin, Tout>(Expression<Func<Tin, Tout>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                if (memberExpression.Expression is ParameterExpression)
                {
                    if (memberExpression.Member is FieldInfo)
                    {
                        return new FieldAccessor(memberExpression.Member.Name);
                    }
                    else if (memberExpression.Member is PropertyInfo)
                    {
                        return new PropertyAccessor(memberExpression.Member.Name);
                    }
                }
            }


            throw new ArgumentException("Expression must be a single property access");
        }
    }
}