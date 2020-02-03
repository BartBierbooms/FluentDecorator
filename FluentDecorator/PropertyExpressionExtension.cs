using System;
using System.Linq.Expressions;

namespace FluentDecorator
{
    internal static class PropertyExpressionExtension
    {
        private static readonly string expressionCannotBeNullMessage = "The expression cannot be null.";
        private static readonly string invalidExpressionMessage = "Invalid expression.";

        private static string SubstractExpressionPlaceHolder(string memberName)
        {
            int pos = memberName.IndexOf(".");
            if (pos > 0 && pos < memberName.Length)
            {
                return memberName.Substring(pos + 1);
            }
            return memberName;
        }

        public static string GetMemberName(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentException(expressionCannotBeNullMessage);
            }

            if (expression is MemberExpression)
            {
                // Reference type property or field
                var memberExpression = (MemberExpression)expression;
                return SubstractExpressionPlaceHolder(memberExpression.ToString());
            }

            if (expression is MethodCallExpression)
            {
                // Reference type method
                var methodCallExpression = (MethodCallExpression)expression;
                return methodCallExpression.Method.Name;
            }

            if (expression is UnaryExpression)
            {
                // Property, field of method returning value type
                var unaryExpression = (UnaryExpression)expression;
                return GetMemberName(unaryExpression);
            }

            throw new ArgumentException(invalidExpressionMessage);
        }
    }
}
