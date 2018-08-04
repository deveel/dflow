using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deveel.Workflows.Variables
{
    public interface IVariableRegistry
    {
        Task SetVariableAsync(Variable variable);

        Task<Variable> FindVariableAsync(string name);

        Task<IList<Variable>> GetVariablesAsync();
    }
}
