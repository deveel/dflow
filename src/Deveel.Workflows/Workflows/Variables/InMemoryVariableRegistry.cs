using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Variables
{
    public sealed class InMemoryVariableRegistry : IVariableRegistry
    {
        private Dictionary<string, Variable> variables;
        private Dictionary<string, IList<IVariableHandler>> handlers;

        public InMemoryVariableRegistry()
        {
            variables = new Dictionary<string, Variable>();
            handlers = new Dictionary<string, IList<IVariableHandler>>();
        }

        public Task SetVariableAsync(Variable variable, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            variables[variable.Name] = variable;

            return Task.Run(async () => await FireHandlersAsync(variable, cancellationToken));
        }

        private Task FireHandlersAsync(Variable variable, CancellationToken cancellationToken)
        {
            IList<IVariableHandler> varHandlers;
            if (handlers.TryGetValue(variable.Name, out varHandlers))
            {
                var tasks = varHandlers.Select(x => x.NotifySetAsync(variable, cancellationToken));
                return Task.WhenAll(tasks);
            }

            return Task.CompletedTask;
        }

        public Task<bool> TryGetVariableAsync(string name, out Variable variable, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!variables.TryGetValue(name, out variable))
                return Task.FromResult<bool>(false);

            return Task.FromResult(true);
        }

        public Task<IList<Variable>> GetVariablesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((IList<Variable>) variables.Values.ToList());
        }

        public Task HandleVariableAsync(string variableName, IVariableHandler handler, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            IList<IVariableHandler> varHandlers;
            if (!handlers.TryGetValue(variableName, out varHandlers))
                handlers[variableName] = varHandlers = new List<IVariableHandler>();

            varHandlers.Add(handler);
            return Task.CompletedTask;
        }
    }
}
