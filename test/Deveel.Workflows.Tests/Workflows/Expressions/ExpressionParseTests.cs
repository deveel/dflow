using System;
using Xunit;

namespace Deveel.Workflows.Expressions
{
    public static class ExpressionParseTests
    {
        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("null", null)]
        [InlineData("''", "")]
        [InlineData("'test'", "test")]
        public static void ParseSimpleConstant(string s, object expected)
        {
            var exp = FlowExpression.Parse(s);

            Assert.IsType<FlowConstantExpression>(exp);

            var value = ((FlowConstantExpression) exp).Value;
            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData("c()", "c", 0)]
        [InlineData("a(65, 'test')", "a", 2)]
        public static void ParseFunction(string s, string functionName, int argc)
        {
            var exp = FlowExpression.Parse(s);

            Assert.IsType<FlowFunctionExpression>(exp);

            Assert.Equal(functionName, ((FlowFunctionExpression)exp).FucntionName);
            Assert.Equal(argc, ((FlowFunctionExpression)exp).Arguments.Length);
        }

        [Theory]
        [InlineData("[a, b, 33]", 3)]
        [InlineData("['l']", 1)]
        [InlineData("[]", 0)]
        public static void ParseConstantArray(string s, int length)
        {
            var exp = FlowExpression.Parse(s);

            Assert.IsType<FlowConstantExpression>(exp);
            Assert.IsType<FlowExpression[]>(((FlowConstantExpression) exp).Value);

            var array = (FlowExpression[]) ((FlowConstantExpression) exp).Value;
            Assert.Equal(length, array.Length);
        }

        [Theory]
        [InlineData("($a)", FlowExpressionType.Variable)]
        [InlineData("(2 + 4)", FlowExpressionType.Add)]
        public static void ParseGroup(string s, FlowExpressionType expType)
        {
            var exp = FlowExpression.Parse(s);

            Assert.IsType<FlowGroupExpression>(exp);
            Assert.Equal(expType, ((FlowGroupExpression)exp).Expression.NodeType);
        }

        [Theory]
        [InlineData("$a == $b", FlowExpressionType.Equal, FlowExpressionType.Variable, FlowExpressionType.Variable)]
        [InlineData("$a > 3", FlowExpressionType.GreaterThan, FlowExpressionType.Variable, FlowExpressionType.Constant)]
        [InlineData("1 >= 89.02", FlowExpressionType.GreaterThanOrEqual, FlowExpressionType.Constant, FlowExpressionType.Constant)]
        [InlineData("$a != 'test'", FlowExpressionType.NotEqual, FlowExpressionType.Variable, FlowExpressionType.Constant)]
        [InlineData("true && false", FlowExpressionType.And, FlowExpressionType.Constant, FlowExpressionType.Constant)]
        [InlineData("$a || false", FlowExpressionType.Or, FlowExpressionType.Variable, FlowExpressionType.Constant)]
        [InlineData("a is null", FlowExpressionType.Is, FlowExpressionType.Variable, FlowExpressionType.Constant)]
        [InlineData("a <= 33", FlowExpressionType.LessThanOrEqual, FlowExpressionType.Variable, FlowExpressionType.Constant)]
        public static void ParseBinary(string s, FlowExpressionType type, FlowExpressionType leftType,
            FlowExpressionType rightType)
        {
            var exp = FlowExpression.Parse(s);

            Assert.Equal(type, exp.NodeType);
            Assert.IsType<FlowBinaryExpression>(exp);
            Assert.Equal(leftType, ((FlowBinaryExpression)exp).Left.NodeType);
            Assert.Equal(rightType, ((FlowBinaryExpression)exp).Right.NodeType);
        }

        [Theory]
        [InlineData("!true", FlowExpressionType.Negate, FlowExpressionType.Constant)]
        [InlineData("+$a", FlowExpressionType.Plus, FlowExpressionType.Variable)]
        [InlineData("!(a is null)", FlowExpressionType.Negate, FlowExpressionType.Group)]
        public static void ParseUnary(string s, FlowExpressionType type, FlowExpressionType operandType)
        {
            var exp = FlowExpression.Parse(s);

            Assert.Equal(type, exp.NodeType);
            Assert.IsType<FlowUnaryExpression>(exp);
            Assert.Equal(operandType, ((FlowUnaryExpression)exp).Operand.NodeType);
        }

        [Theory]
        [InlineData("a = 22", "a", FlowExpressionType.Constant)]
        public static void ParseAssign(string s, string varName, FlowExpressionType valueType)
        {
            var exp = FlowExpression.Parse(s);

            Assert.IsType<FlowAssignExpression>(exp);

            Assert.Equal(varName, ((FlowAssignExpression)exp).VariableName);
            Assert.Equal(valueType, ((FlowAssignExpression)exp).Value.NodeType);
        }
    }
}

