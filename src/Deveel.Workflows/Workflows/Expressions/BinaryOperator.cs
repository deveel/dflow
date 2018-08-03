using System;

namespace Deveel.Workflows.Expressions
{
    static class BinaryOperator
    {
        public static object Evaluate(FlowExpressionType binaryType, object left, object right)
        {
            switch (binaryType)
            {
                case FlowExpressionType.Add:
                    return Add(left, right);
                case FlowExpressionType.Subtract:
                    return Subtract(left, right);
                case FlowExpressionType.Multiply:
                    return Multiply(left, right);
                case FlowExpressionType.Divide:
                    return Divide(left, right);
                case FlowExpressionType.Modulo:
                    return Modulo(left, right);
                case FlowExpressionType.GreaterThan:
                    return IsGreaterThan(left, right);
                case FlowExpressionType.GreaterThanOrEqual:
                    return IsGreterThanOrEqual(left, right);
                case FlowExpressionType.LessThan:
                    return IsLessThan(left, right);
                case FlowExpressionType.LessThanOrEqual:
                    return IsLessThanOrEqual(left, right);
                case FlowExpressionType.Equal:
                    return IsEqual(left, right);
                case FlowExpressionType.NotEqual:
                    return IsNotEqual(left, right);
                case FlowExpressionType.Is:
                    return Is(left, right);
                case FlowExpressionType.And:
                    return And(left, right);
                case FlowExpressionType.Or:
                    return Or(left, right);
                default:
                    throw new InvalidOperationException($"The type {binaryType} is not a binary expression or is not supported.");
            }

        }

        private static bool Or(object left, object right)
        {
            if (!(left is bool) ||
                !(right is bool))
                throw new InvalidOperationException();

            return (bool) left || (bool) right;
        }

        private static bool And(object left, object right)
        {
            if (!(left is bool) ||
                !(right is bool))
                throw new InvalidOperationException();

            return (bool)left && (bool)right;
        }

        private static bool Is(object left, object right)
        {
            if (right == null)
                return left == null;
            if (right is string && 
                (string)right == String.Empty)
            {
                return left is string && String.IsNullOrWhiteSpace((string) left);
            }

            return false;
        }

        private static bool IsNotEqual(object left, object right)
        {
            return !Equals(left, right);
        }

        private static bool IsEqual(object left, object right)
        {
            return Equals(left, right);
        }

        private static bool IsLessThanOrEqual(object left, object right)
        {
            throw new NotImplementedException();
        }

        private static bool IsLessThan(object left, object right)
        {
            throw new NotImplementedException();
        }

        private static bool IsGreterThanOrEqual(object left, object right)
        {
            throw new NotImplementedException();
        }

        private static bool IsGreaterThan(object left, object right)
        {
            throw new NotImplementedException();
        }

        private static object Modulo(object left, object right)
        {
            throw new NotImplementedException();
        }

        private static object Divide(object left, object right)
        {
            throw new NotImplementedException();
        }

        private static object Multiply(object left, object right)
        {
            throw new NotImplementedException();
        }

        private static object Add(object left, object right)
        {
            throw new NotImplementedException();
        }

        private static object Subtract(object left, object right)
        {
            throw new NotImplementedException();
        }
    }
}
