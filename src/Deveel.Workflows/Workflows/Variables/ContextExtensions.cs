using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows.Variables
{
    public static class ContextExtensions
    {
        public static async Task<object> FindVariableAsync(this IContext context, string name)
        {
            var registry = context.GetService<IVariableRegistry>();
            if (registry == null)
                return null;

            var variableObj = await registry.FindVariableAsync(name);
            if (variableObj == null)
                return null;

            return variableObj.Value;
        }


        public static async Task SetVariableAsync(this IContext context, string name, object value)
        {
            var registry = context.GetService<IVariableRegistry>();
            if (registry == null)
                throw new InvalidOperationException();

            await registry.SetVariableAsync(new Variable(name, value));
        }

        public static Task<IList<Variable>> GetVariablesAsync(this IContext context)
        {
            var registry = context.GetService<IVariableRegistry>();
            if (registry == null)
                return Task.FromResult<IList<Variable>>(new List<Variable>());

            return registry.GetVariablesAsync();
        }
    }
}
