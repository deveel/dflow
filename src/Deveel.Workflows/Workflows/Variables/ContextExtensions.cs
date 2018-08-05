using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deveel.Workflows.Variables
{
    public static class ContextExtensions
    {
        public static bool TryGetVariable(this IContext context, string name, out Variable variable)
        {
            var current = context;
            while (current != null)
            {
                if (current is IVariableContext)
                {
                    var variables = ((IVariableContext)current).Variables;
                    if (variables.TryGetVariableAsync(name, out Variable v, context.CancellationToken).Result)
                    {
                        variable = v;
                        return true;
                    }
                }

                current = current.Parent;
            }

            // TODO: throw a Null-Reference Exception?
            variable = null;
            return false;
        }

        public static bool TrySetVariable(this IContext context, string name, object value)
        {
            var current = context;
            while (current != null)
            {
                if (current is IVariableContext)
                {
                    var variables = ((IVariableContext)current).Variables;
                    if (variables.TryGetVariableAsync(name, out Variable v, context.CancellationToken).Result)
                    {
                        variables.SetVariableAsync(new Variable(name, value), context.CancellationToken).Wait();
                        return true;
                    }
                }

                current = current.Parent;
            }

            // TODO: throw a Null-Reference Exception?
            return false;
        }

        public static async Task<object> FindVariableAsync(this IContext context, string name)
        {
            var current = context;
            while(current != null)
            {
                if (current is IVariableContext)
                {
                    var variables = ((IVariableContext)current).Variables;
                    if (await variables.TryGetVariableAsync(name, out Variable variable, context.CancellationToken))
                        return variable.Value;
                }

                current = current.Parent;
            }

            // TODO: throw a Null-Reference Exception?
            return null;
        }


        public static Task SetVariableAsync(this IContext context, string name, object value)
        {
            var current = context;
            while (current != null)
            {
                if (current is IVariableContext)
                {
                    var variables = ((IVariableContext)current).Variables;
                    return variables.SetVariableAsync(new Variable(name, value), context.CancellationToken);
                }

                current = current.Parent;
            }

            return Task.FromException(new FlowException("It was not possible to find any valid variables context"));
        }

        public static Task SetProcessVariableAsync(this IContext context, string name, object value)
        {
            var current = context;
            while (current != null)
            {
                if (current is ProcessContext)
                {
                    return ((IVariableContext)current).Variables.SetVariableAsync(new Variable(name, value), context.CancellationToken);
                }

                current = current.Parent;
            }

            return Task.FromException(new FlowException("This should never happen, but it was not possible to find a process"));
        }

            public static Task<IList<Variable>> GetVariablesAsync(this IContext context)
        {
            var current = context;
            while (current != null)
            {
                if (current is IVariableContext)
                {
                    var variables = ((IVariableContext)current).Variables;
                    return variables.GetVariablesAsync(context.CancellationToken);
                }

                current = current.Parent;
            }

            return Task.FromException<IList<Variable>>(new FlowException("It was not possible to find any valid variables context"));
        }
    }
}
