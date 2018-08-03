using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deveel.Workflows.Variables
{
    public sealed class InMemoryVariableRegistry : IVariableRegistry
    {
        private Dictionary<string, Variable> variables;

        public InMemoryVariableRegistry()
        {
            variables = new Dictionary<string, Variable>();
        }

        public Task SetVariableAsync(Variable variable)
        {
            variables[variable.Name] = variable;
            return Task.CompletedTask;
        }

        public Task<Variable> FindVariableAsync(string name)
        {
            if (!variables.TryGetValue(name, out var variable))
                return Task.FromResult<Variable>(null);

            return Task.FromResult(variable);
        }
    }
}
