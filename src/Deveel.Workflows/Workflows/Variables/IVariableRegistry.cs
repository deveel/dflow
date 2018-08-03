using System.Threading.Tasks;
using Deveel.Workflows.Infrastructure;

namespace Deveel.Workflows.Variables
{
    public interface IVariableRegistry
    {
        Task SetVariableAsync(Variable variable);

        Task<Variable> FindVariableAsync(string name);
    }
}
