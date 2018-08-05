using System;

namespace Deveel.Workflows.Variables
{
    public interface IVariableContext
    {
        IVariableRegistry Variables { get; }
    }
}
