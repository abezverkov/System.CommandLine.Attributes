using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace System.CommandLine.Attributes
{
    internal static class ExpressionHelper
    {
        internal static string GetMemberName<T>(this Expression<T> expression)
        {
            switch (expression.Body)
            {
                case MemberExpression m:
                    return m.Member.Name;
                case UnaryExpression u when u.Operand is MemberExpression m:
                    return m.Member.Name;
                case ConstantExpression c:
                    return c.Value.ToString();
                default:
                    throw new NotImplementedException(expression.GetType().ToString());
            }
        }
    }
}
