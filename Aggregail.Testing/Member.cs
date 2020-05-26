using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Aggregail.Testing
{
    internal static class Member
    {
        public static MemberInfo Of<T>(Expression<Func<T, object>> expression)
        {
            var memberExpression = (MemberExpression) expression.Body;
            return memberExpression.Member;
        }
    }
}