using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Infrastructure
{
    public sealed class InMemoryExecutionRegistry : IExecutionRegistry
    {
        private Dictionary<string, IExecutionContext> states;

        public InMemoryExecutionRegistry()
        {
            states = new Dictionary<string, IExecutionContext>();
        }

        public Task RegisterAsync(string nodeId, IExecutionContext state)
        {
            states.Add(nodeId, state);
            return Task.CompletedTask;
        }

        public Task<IExecutionContext> FindStateAsync(string nodeId)
        {
            while (!states.ContainsKey(nodeId))
            {
                Thread.Sleep(200);
            }

            return Task.FromResult(states[nodeId]);
        }
    }
}
