using System;
using System.Collections.Generic;
using Deveel.Workflows.Actors;

namespace Deveel.Workflows.Scripts
{
    public sealed class ScriptGlobals
    {
        internal ScriptGlobals(ExecutionContext context, IDictionary<string, object> variables)
        {
            Context = context;
            Variables = new Dictionary<string, object>(variables);
        }

        public ExecutionContext Context { get; }

        public IActor Actor => Context.Actor;

        internal IDictionary<string, object> Variables { get; }

        internal bool VariablesSet { get; private set; }

        public T GetVariable<T>(string variableName)
        {
            if (Variables == null ||
                !Variables.TryGetValue(variableName, out object value))
                return default(T);

            return (T) Convert.ChangeType(value, typeof(T));
        }

        public void SetVariable(string variableName, object value)
        {
            Variables[variableName] = value;
            VariablesSet = true;
        }
    }
}
