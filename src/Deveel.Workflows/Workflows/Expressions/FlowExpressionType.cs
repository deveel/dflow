using System;

namespace Deveel.Workflows.Expressions
{
    public enum FlowExpressionType
    {
        Constant,

        // Unariies
        Plus,
        Negate,

        // Additive Binaries
        Add,
        Subtract,

        // Multiplicative Binaries
        Multiply,
        Divide,
        Modulo,

        // Comparison Binaries
        Equal,
        NotEqual,
        Is,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,

        // Logical Binaries
        And,
        Or,

        Function,
        Variable,
        Assign,

        Group
    }
}
