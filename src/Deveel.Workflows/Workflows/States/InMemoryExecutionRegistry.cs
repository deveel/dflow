using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.States
{
    public sealed class InMemoryExecutionRegistry : IExecutionRegistry
    {
        private Dictionary<StateId, ExecutionState> states;

        public InMemoryExecutionRegistry()
        {
            states = new Dictionary<StateId, ExecutionState>();
        }

        public Task RegisterAsync(ExecutionState state)
        {
            states.Add(new StateId(state.ProcessId, state.NodeId), state);
            return Task.CompletedTask;
        }

        public Task<ExecutionState> FindStateAsync(string processId, string nodeId)
        {
            var stateId = new StateId(processId, nodeId);

            while (!states.ContainsKey(stateId))
            {
                Thread.Sleep(200);
            }

            return Task.FromResult(states[stateId]);
        }

        #region StateId

        struct StateId : IEquatable<StateId>
        {
            public StateId(string processId, string nodeId)
            {
                ProcessId = processId;
                NodeId = nodeId;
            }

            public string ProcessId { get; }

            public string NodeId { get; }

            public bool Equals(StateId other)
            {
                return string.Equals(ProcessId, other.ProcessId) && string.Equals(NodeId, other.NodeId);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is StateId && Equals((StateId) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((ProcessId != null ? ProcessId.GetHashCode() : 0) * 397) ^ (NodeId != null ? NodeId.GetHashCode() : 0);
                }
            }
        }

        #endregion
    }
}
