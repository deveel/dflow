using System;

namespace Deveel.Workflows.Expressions
{
    public class FlowExpressionVisitor
    {
        public virtual FlowExpression Visit(FlowExpression expression)
        {
            if (expression == null)
                return null;

            switch (expression.NodeType)
            {
                case FlowExpressionType.Plus:
                case FlowExpressionType.Negate:
                    return VisitUnary((FlowUnaryExpression) expression);
                case FlowExpressionType.Add:
                case FlowExpressionType.Subtract:
                case FlowExpressionType.Multiply:
                case FlowExpressionType.Divide:
                case FlowExpressionType.Modulo:
                case FlowExpressionType.Equal:
                case FlowExpressionType.NotEqual:
                case FlowExpressionType.GreaterThan:
                case FlowExpressionType.GreaterThanOrEqual:
                case FlowExpressionType.LessThan:
                case FlowExpressionType.LessThanOrEqual:
                case FlowExpressionType.And:
                case FlowExpressionType.Or:
                case FlowExpressionType.Is:
                    return VisitBinary((FlowBinaryExpression) expression);
                case FlowExpressionType.Variable:
                    return VisitVariable((FlowVariableExpression) expression);
                case FlowExpressionType.Constant:
                    return VisitConstant((FlowConstantExpression) expression);
                case FlowExpressionType.Function:
                    return VisitFunction((FlowFunctionExpression) expression);
                case FlowExpressionType.Group:
                    return VisitGroup((FlowGroupExpression) expression);
                default:
                    throw new ArgumentException();
            }
        }

        public virtual FlowExpression VisitUnary(FlowUnaryExpression expression)
        {
            var operand = expression.Operand;
            if (operand != null)
                operand = Visit(operand);

            return FlowExpression.Unary(expression.NodeType, operand);
        }

        public virtual FlowExpression VisitBinary(FlowBinaryExpression expression)
        {
            var left = expression.Left;
            if (left != null)
                left = Visit(left);

            var right = expression.Right;
            if (right != null)
                right = Visit(right);

            return FlowExpression.Binary(expression.NodeType, left, right);
        }

        public virtual FlowExpression VisitVariable(FlowVariableExpression expression)
        {
            return FlowExpression.Variable(expression.VariableName);
        }

        public virtual FlowExpression VisitConstant(FlowConstantExpression expression)
        {
            return FlowExpression.Constant(expression.Value);
        }

        public virtual FlowExpression VisitFunction(FlowFunctionExpression expression)
        {
            var args = expression.Arguments;
            if (args != null)
            {
                var list = new FlowExpression[args.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    list[i] = Visit(args[i]);
                }

                args = list;
            }

            return FlowExpression.Function(expression.FucntionName, args);
        }

        public virtual FlowExpression VisitGroup(FlowGroupExpression expression)
        {
            var exp = expression.Expression;
            if (exp != null)
                exp = Visit(exp);

            return FlowExpression.Group(exp);
        }
    }
}

