using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NSBD.SharepointAutoMapper
{
    public static class MapperCamlQuery
    {
        private static XElement ParseNodeType(ExpressionType type)
        {
            XElement node;

            switch (type)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.And:
                    node = new XElement("And");
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    node = new XElement("Or");
                    break;
                case ExpressionType.Equal:
                    node = new XElement("Eq");
                    break;
                case ExpressionType.GreaterThan:
                    node = new XElement("Gt");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    node = new XElement("Geq");
                    break;
                case ExpressionType.LessThan:
                    node = new XElement("Lt");
                    break;
                case ExpressionType.LessThanOrEqual:
                    node = new XElement("Leq");
                    break;
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", type));
            }

            return node;
        }

        private static XElement VisitMemberAccess(MemberExpression member)
        {

            var expr = member.Expression;
            if (expr.NodeType == ExpressionType.Constant)
            {
                LambdaExpression lambda = Expression.Lambda(member);
                Delegate fn = lambda.Compile();
                return VisitConstant(Expression.Constant(fn.DynamicInvoke(null), member.Type));

            }
            else
            {
                return new XElement("FieldRef", new XAttribute("Name", member.Member.Name));
            }
        }

        private static XElement VisitConstant(ConstantExpression constant)
        {
            return new XElement("Value", ParseValueType(constant.Type), constant.Value);
        }

        private static XAttribute ParseValueType(Type type)
        {
            string name = "Text";

            switch (type.Name)
            {
                case "DateTime":
                    name = "DateTime";
                    break;
                case "String":
                    name = "Text";
                    break;
                default:
                    throw new Exception(string.Format("Unhandled value type parser for: '{0}'", type.Name));
            }

            return new XAttribute("Type", name);
        }

        private static XElement VisitMethodCall(MethodCallExpression methodcall)
        {
            XElement node;
            XElement left = Visit(methodcall.Object);
            XElement right = Visit(methodcall.Arguments[0]);

            switch (methodcall.Method.Name)
            {
                case "Contains":
                    node = new XElement("Contains");
                    break;
                case "StartsWith":
                    node = new XElement("BeginsWith");
                    break;
                default:
                    throw new Exception(string.Format("Unhandled method call: '{0}'", methodcall.Method.Name));
            }

            if (left != null && right != null)
            {
                node.Add(left, right);
            }

            return node;

        }
        private static XElement VisitBinary(BinaryExpression binary)
        {
            XElement node = ParseNodeType(binary.NodeType);

            XElement left = Visit(binary.Left);
            XElement right = Visit(binary.Right);

            if (left != null && right != null)
            {
                node.Add(left, right);
            }

            return node;
        }

        private static XElement Visit(Expression expression)
        {
            if (expression == null)
            {
                return null;
            }

            switch (expression.NodeType)
            {
                case ExpressionType.Call:
                    return VisitMethodCall(expression as MethodCallExpression);
                case ExpressionType.MemberAccess:
                    return VisitMemberAccess(expression as MemberExpression);
                case ExpressionType.Constant:
                    return VisitConstant(expression as ConstantExpression);
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    return VisitBinary(expression as BinaryExpression);
                default:
                    return null;
            }
        }

        public static string BuildCamlQuery<T>(this IEntitySharepointMapper value ,Expression<Func<T, bool>> expression) where T : IEntitySharepointMapper
        {

            return Translate(expression.Body);


        }

        public static string Translate( Expression expression)
        {
            XElement query = new XElement("Where");
            query.Add(Visit(expression));
            return query.ToString(SaveOptions.DisableFormatting);
        }
    }
}
