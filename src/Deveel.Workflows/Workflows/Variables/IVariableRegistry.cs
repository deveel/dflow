using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Variables
{
    public interface IVariableRegistry
    {
        Task SetVariableAsync(Variable variable, CancellationToken cancellationToken);

        Task<bool> TryGetVariableAsync(string name, out Variable variable, CancellationToken cancellationToken);

        Task<IList<Variable>> GetVariablesAsync(CancellationToken cancellationToken);

        Task HandleVariableAsync(string variableName, IVariableHandler handler, CancellationToken cancellationToken);
    }
}
