using System;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace Deveel.Workflows.Expressions
{
    public abstract class FlowExpression
    {
        internal FlowExpression()
        {
            Precedence = GetPrecedence();
        }

        public abstract FlowExpressionType NodeType { get; }

        internal int Precedence { get; }

        private int GetPrecedence()
        {
            switch (NodeType)
            {
                // Group
                case FlowExpressionType.Group:
                    return 151;

                // References
                case FlowExpressionType.Function:
                case FlowExpressionType.Assign:
                case FlowExpressionType.Variable:
                    return 150;

                // Unary
                case FlowExpressionType.Plus:
                case FlowExpressionType.Negate:
                    return 140;

                // Multiplicative
                case FlowExpressionType.Multiply:
                case FlowExpressionType.Divide:
                case FlowExpressionType.Modulo:
                    return 130;

                // Additive
                case FlowExpressionType.Add:
                case FlowExpressionType.Subtract:
                    return 120;

                // Relational
                case FlowExpressionType.GreaterThan:
                case FlowExpressionType.GreaterThanOrEqual:
                case FlowExpressionType.LessThan:
                case FlowExpressionType.LessThanOrEqual:
                case FlowExpressionType.Is:
                    return 110;

                // Equality
                case FlowExpressionType.Equal:
                case FlowExpressionType.NotEqual:
                    return 100;

                // Logical
                case FlowExpressionType.And:
                    return 90;
                case FlowExpressionType.Or:
                    return 89;

                // Constant
                case FlowExpressionType.Constant:
                    return 70;
            }

            return -1;
        }

        internal abstract Task<FlowExpression> ReduceAsync(IExecutionContext context);

        public virtual FlowExpression Accept(FlowExpressionVisitor visitor)
        {
            return visitor.Visit(this);
        }

        public static FlowExpression Parse(string s)
        {
            var inputStream = new AntlrInputStream(s);
            var lexer = new ExpressionsLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ExpressionsParser(tokenStream);

            var exp = parser.expression();

            return ParseExpression(exp);
        }

        private static FlowExpression ParseExpression(ExpressionsParser.ExpressionContext exp)
        {
            var visitor = new ExpressionParseVisitor();
            return visitor.Visit(exp);
        }

        #region Factories

        public static FlowConstantExpression Constant(object value)
        {
            return new FlowConstantExpression(value);
        }

        public static FlowVariableExpression Variable(string variableName)
        {
            return new FlowVariableExpression(variableName);
        }

        public static FlowBinaryExpression And(FlowExpression left, FlowExpression right)
        {
            return Binary(FlowExpressionType.And, left, right);
        }

        public static FlowBinaryExpression Or(FlowExpression left, FlowExpression right)
            => Binary(FlowExpressionType.Or, left, right);

        public static FlowBinaryExpression Binary(FlowExpressionType expressionType, FlowExpression left,
            FlowExpression right)
        {
            switch (expressionType)
            {
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
                case FlowExpressionType.Is:
                case FlowExpressionType.And:
                case FlowExpressionType.Or:
                    return new FlowBinaryExpression(expressionType, left, right);
                default:
                    throw new ArgumentException($"The expression type {expressionType} is not binary");
            }
        }

        public static FlowBinaryExpression Add(FlowExpression left, FlowExpression right)
            => Binary(FlowExpressionType.Add, left, right);

        public static FlowUnaryExpression Unary(FlowExpressionType expressionType, FlowExpression unary)
        {
            switch (expressionType)
            {
                case FlowExpressionType.Plus:
                case FlowExpressionType.Negate:
                    return new FlowUnaryExpression(expressionType, unary);
                default:
                    throw new ArgumentException($"The expresion type '{expressionType}' is not unary");
            }
            
        }

        public static FlowUnaryExpression Plus(FlowExpression unary)
        {
            return Unary(FlowExpressionType.Plus, unary);
        }

        public static FlowUnaryExpression Negate(FlowExpression unary)
        {
            return Unary(FlowExpressionType.Negate, unary);
        }

        public static FlowGroupExpression Group(FlowExpression expression)
        {
            return new FlowGroupExpression(expression);
        }

        public static FlowFunctionExpression Function(string functionName, FlowExpression[] args)
        {
            return new FlowFunctionExpression(functionName, args);
        }

        public static FlowExpression Assign(string variable, FlowExpression value)
            => new FlowAssignExpression(variable, value);

        #endregion
    }
}
