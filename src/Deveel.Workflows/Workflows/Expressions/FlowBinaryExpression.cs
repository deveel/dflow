using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Expressions
{
    public sealed class FlowBinaryExpression : FlowExpression
    {
        internal FlowBinaryExpression(FlowExpressionType nodeType, FlowExpression left, FlowExpression right)
        {
            NodeType = nodeType;
            Left = left;
            Right = right;
        }

        public override FlowExpressionType NodeType { get; }

        public FlowExpression Left { get; }

        public FlowExpression Right { get; }

        private async Task<FlowExpression[]> ReduceSides(IContext context, CancellationToken cancellationToken)
        {
            var info = new List<BinaryReduceInfo> {
                new BinaryReduceInfo {Expression = Left, Offset = 0},
                new BinaryReduceInfo {Expression = Right, Offset = 1}
            }.OrderByDescending(x => x.Precedence).ToArray();

            foreach (var evaluateInfo in info)
            {
                evaluateInfo.Expression = await evaluateInfo.Expression.ReduceAsync(context, cancellationToken);
            }

            return info.OrderBy(x => x.Offset)
                .Select(x => x.Expression)
                .ToArray();
        }

        public override async Task<FlowExpression> ReduceAsync(IContext context, CancellationToken cancellationToken)
        {
            var sides = await ReduceSides(context, cancellationToken);

            var left = sides[0];
            var right = sides[1];

            if (left.NodeType != FlowExpressionType.Constant)
                throw new InvalidOperationException("The reduced left side of a binary expression is not constant");
            if (right.NodeType != FlowExpressionType.Constant)
                throw new InvalidOperationException("The reduced right side of a binary expression is not constant.");

            var value1 = ((FlowConstantExpression)left).Value;
            var value2 = ((FlowConstantExpression)right).Value;

            var result = BinaryOperator.Evaluate(NodeType, value1, value2);

            return Constant(result);
        }

        #region BinaryReduceInfo

        class BinaryReduceInfo
        {
            public FlowExpression Expression { get; set; }

            public int Offset { get; set; }

            public int Precedence => Expression.Precedence;
        }

        #endregion
    }
}
