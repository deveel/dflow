using System;
using System.Globalization;
using System.Linq;
using Antlr4.Runtime.Misc;

namespace Deveel.Workflows.Expressions
{
    class ExpressionParseVisitor : ExpressionsParserBaseVisitor<FlowExpression>
    {
        public override FlowExpression VisitConstantFalse(ExpressionsParser.ConstantFalseContext context)
        {
            return FlowExpression.Constant(false);
        }

        public override FlowExpression VisitConstantTrue(ExpressionsParser.ConstantTrueContext context)
        {
            return FlowExpression.Constant(true);
        }

        public override FlowExpression VisitConstantNull(ExpressionsParser.ConstantNullContext context)
        {
            return FlowExpression.Constant(null);
        }

        public override FlowExpression VisitConstantArray(ExpressionsParser.ConstantArrayContext context)
        {
            var expOrVector = context.array().expressionOrVector();
            if (expOrVector == null)
                return FlowExpression.Constant(new FlowExpression[0]);

            var exp = Visit(expOrVector.expression());
            var vector = context.array().expressionOrVector().vectorExpression();
            if (vector == null)
                return FlowExpression.Constant(new[] {exp});

            var exps = vector.expression().Select(Visit).ToArray();
            if (exps.Length == 0)
                return FlowExpression.Constant(new []{exp});

            var array = new FlowExpression[exps.Length + 1];
            array[0] = exp;
            Array.Copy(exps, 0, array, 1, exps.Length);

            return FlowExpression.Constant(array);
        }

        public override FlowExpression VisitConstantNumeric(ExpressionsParser.ConstantNumericContext context)
        {
            var value = context.numeric().GetText();
            var formatInfo = new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            };

            if (Int32.TryParse(value, NumberStyles.Integer, formatInfo, out int i))
                return FlowExpression.Constant(i);
            if (Int64.TryParse(value, NumberStyles.Integer, formatInfo, out long l))
                return FlowExpression.Constant(l);
            if (Double.TryParse(value, NumberStyles.Float, formatInfo, out double d))
                return FlowExpression.Constant(d);
            if (Decimal.TryParse(value, NumberStyles.Number, formatInfo, out decimal dec))
                return FlowExpression.Constant(dec);

            throw new ArgumentException();
        }

        public override FlowExpression VisitConstantString(ExpressionsParser.ConstantStringContext context)
        {
            var s = context.quoted_string().GetText();
            if (String.IsNullOrWhiteSpace(s))
                throw new InvalidOperationException();
            if (s.Length < 2)
                throw new InvalidOperationException();

            if (s[0] == '\'' ||
                s[1] == '\"')
                s = s.Substring(1);
            if (s[s.Length - 1] == '\'' ||
                s[s.Length - 1] == '\"')
                s = s.Substring(0, s.Length - 1);

            return FlowExpression.Constant(s);
        }

        private static string VariableName(ExpressionsParser.BindVariableContext context)
        {
            var text = context.GetText();
            if (String.IsNullOrEmpty(text))
                return text;

            if (text[0] == '$')
                text = text.Substring(1);

            return text;
        }

        public override FlowExpression VisitAssignExpression(ExpressionsParser.AssignExpressionContext context)
        {
            var varName = VariableName(context.bindVariable());
            var value = Visit(context.expression());
            return FlowExpression.Assign(varName, value);
        }

        public override FlowExpression VisitBindVariable(ExpressionsParser.BindVariableContext context)
        {
            var varRef = VariableName(context);
            return FlowExpression.Variable(varRef);
        }

        public override FlowExpression VisitUnaryplusExpression(ExpressionsParser.UnaryplusExpressionContext context)
        {
            return FlowExpression.Plus(Visit(context.unaryExpression()));
        }

        public override FlowExpression VisitUnaryminusExpression(ExpressionsParser.UnaryminusExpressionContext context)
        {
            return FlowExpression.Negate(Visit(context.unaryExpression()));
        }

        public override FlowExpression VisitNegatedExpression(ExpressionsParser.NegatedExpressionContext context)
        {
            if (context.negatedExpression() != null)
            {
                var eqExp = Visit(context.negatedExpression());
                return FlowExpression.Negate(eqExp);
            }

            return Visit(context.equalityExpression());
        }

        private FlowExpressionType GetBinaryOperator(string s)
        {
            if (s == "+")
                return FlowExpressionType.Add;
            if (s == "-")
                return FlowExpressionType.Subtract;
            if (s == "=")
                return FlowExpressionType.Equal;
            if (s == "<>" || s == "!=")
                return FlowExpressionType.NotEqual;
            if (s == "/")
                return FlowExpressionType.Divide;
            if (s == "*")
                return FlowExpressionType.Multiply;
            if (s == "%" ||
                String.Equals(s, "MOD", StringComparison.OrdinalIgnoreCase))
                return FlowExpressionType.Modulo;
            if (s == ">")
                return FlowExpressionType.GreaterThan;
            if (s == ">=")
                return FlowExpressionType.GreaterThanOrEqual;
            if (s == "<")
                return FlowExpressionType.LessThan;
            if (s == "<=")
                return FlowExpressionType.LessThanOrEqual;

            if (String.Equals(s, "IS", StringComparison.OrdinalIgnoreCase))
                return FlowExpressionType.Is;

            throw new NotSupportedException($"Expression type '{s}' not supported");
        }

        public override FlowExpression VisitRelationalExpression(ExpressionsParser.RelationalExpressionContext context)
        {
            var exps = context.compoundExpression().Select(Visit).ToArray();

            FlowExpression last = null;
            for (int i = 0; i < exps.Length; i++)
            {
                if (last == null)
                {
                    last = exps[i];
                }
                else
                {
                    var opContext = context.relationalOperator(i - 1);
                    FlowExpressionType expType;
                    if (opContext.greaterThanOrEquals() != null)
                    {
                        expType = FlowExpressionType.GreaterThanOrEqual;
                    }
                    else if (opContext.lessThanOrEquals() != null)
                    {
                        expType = FlowExpressionType.LessThanOrEqual;
                    }
                    else if (opContext.equal() != null)
                    {
                        expType = FlowExpressionType.Equal;
                    }
                    else if (opContext.notEqual() != null)
                    {
                        expType = FlowExpressionType.NotEqual;
                    }
                    else if (opContext.op != null)
                    {
                        expType = GetBinaryOperator(opContext.op.Text);
                    }
                    else
                    {
                        throw new ParseCanceledException("Invalid relational operator");
                    }

                    last = FlowExpression.Binary(expType, last, exps[i]);
                }
            }

            return last;
        }

        public override FlowExpression VisitEqualityExpression(ExpressionsParser.EqualityExpressionContext context)
        {
            if (context.assignExpression() != null)
                return VisitAssignExpression(context.assignExpression());

            var left = Visit(context.relationalExpression());

            string op = null;

            if (context.IS().Length > 0)
            {
                    op = "IS";
            }

            if (String.IsNullOrEmpty(op))
                return left;

            object value;
            if (context.EMPTY().Length > 0)
            {
                value = String.Empty;
            }
            else if (context.NAN().Length > 0)
            {
                value = double.NaN;
            }
            else if (context.NULL().Length > 0)
            {
                value = null;
            }
            else 
            {
                throw new NotSupportedException();
            }

            var right = FlowExpression.Constant(value);
            var expType = GetBinaryOperator(op);
            return FlowExpression.Binary(expType, left, right);
        }

        public override FlowExpression VisitAdditiveExpression(ExpressionsParser.AdditiveExpressionContext context)
        {
            var exps = context.multiplyExpression().Select(Visit).ToArray();
            if (exps.Length == 1)
                return exps[0];

            FlowExpression last = null;
            for (int i = 0; i < exps.Length; i++)
            {
                if (last == null)
                {
                    last = exps[i];
                }
                else
                {
                    var opContext = context.additiveOperator(i - 1);
                    var expType = GetBinaryOperator(opContext.GetText());
                    last = FlowExpression.Binary(expType, last, exps[i]);
                }
            }

            return last;
        }

        public override FlowExpression VisitExpression(ExpressionsParser.ExpressionContext context)
        {
            var exps = context.logicalAndExpression().Select(Visit).ToArray();
            if (exps.Length == 1)
                return exps[0];

            FlowExpression last = null;
            foreach (var exp in exps)
            {
                if (last == null)
                {
                    last = exp;
                }
                else
                {
                    last = FlowExpression.Or(last, exp);
                }
            }

            return last;
        }

        public override FlowExpression VisitLogicalAndExpression(ExpressionsParser.LogicalAndExpressionContext context)
        {
            var exps = context.negatedExpression().Select(Visit).ToArray();
            if (exps.Length == 1)
                return exps[0];

            FlowExpression last = null;
            foreach (var exp in exps)
            {
                if (last == null)
                {
                    last = exp;
                }
                else
                {
                    last = FlowExpression.And(last, exp);
                }
            }

            return last;
        }

        public override FlowExpression VisitMultiplyExpression(ExpressionsParser.MultiplyExpressionContext context)
        {
            var exps = context.unaryExpression().Select(Visit).ToArray();
            if (exps.Length == 1)
                return exps[0];

            FlowExpression last = null;
            for (int i = 0; i < exps.Length; i++)
            {
                if (last == null)
                {
                    last = exps[i];
                }
                else
                {
                    var opContext = context.multiplyOperator(i - 1);
                    var expType = GetBinaryOperator(opContext.GetText());
                    last = FlowExpression.Binary(expType, last, exps[i]);
                }
            }

            return last;
        }

        public override FlowExpression VisitGroup(ExpressionsParser.GroupContext context)
        {
            var exp = Visit(context.expression());
            return FlowExpression.Group(exp);
        }

        public override FlowExpression VisitStandardFunction(ExpressionsParser.StandardFunctionContext context)
        {
            var functionName = context.objectName().GetText();
            var args = new FlowExpression[0];

            if (context.functionArgument() != null)
            {
                args = context.functionArgument().argument().Select(Visit).ToArray();
            }

            return FlowExpression.Function(functionName, args);
        }

        public override FlowExpression VisitConcatenation(ExpressionsParser.ConcatenationContext context)
        {
            var exps = context.additiveExpression().Select(Visit).ToArray();
            if (exps.Length == 1)
                return exps[0];

            FlowExpression exp = null;
            foreach (var e in exps)
            {
                if (exp == null)
                {
                    exp = e;
                }
                else
                {
                    exp = FlowExpression.Add(exp, e);
                }
            }

            return exp;
        }
    }
}
