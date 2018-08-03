using System;
using System.Collections;
using System.Collections.Generic;

namespace Deveel.Workflows
{
    public abstract class Gateway : FlowNode, IEnumerable<IGatewayFlow>
    {
        protected Gateway(string id)
            : base(id)
        {
        }

        public override FlowNodeType NodeType => FlowNodeType.Gateway;

        IEnumerator<IGatewayFlow> IEnumerable<IGatewayFlow>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected abstract IEnumerator<IGatewayFlow> GetEnumerator();

        internal abstract void ValidateAdd(IEnumerable<IGatewayFlow> flows, IGatewayFlow item);

        internal virtual bool Contains(IEnumerable<IGatewayFlow> flows, IGatewayFlow item)
        {
            return false;
        }
    }
}
