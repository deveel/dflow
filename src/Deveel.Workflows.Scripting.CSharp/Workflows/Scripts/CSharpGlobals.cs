using System;
using System.Dynamic;

using Deveel.Workflows.Variables;

namespace Deveel.Workflows.Scripts
{
    public sealed class CSharpGlobals
    {
        internal CSharpGlobals(ScriptContext context)
        {
            this.context = context;
            vars = new Variables(context);
        }

        public ScriptContext context;

        public dynamic vars;

        class Variables : DynamicObject
        {
            private readonly ScriptContext context;

            internal Variables(ScriptContext context)
            {
                this.context = context;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                Variable variable;
                if (context.TryGetVariable(binder.Name, out variable))
                {
                    result = variable.Value;
                    return true;
                }

                return base.TryGetMember(binder, out result);
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                if (context.TrySetVariable(binder.Name, value))
                {
                    return true;
                }

                return base.TrySetMember(binder, value);
            }
        }
    }
}
