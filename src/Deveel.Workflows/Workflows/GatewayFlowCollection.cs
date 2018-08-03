using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Workflows
{
    public sealed class GatewayFlowCollection<TFlow> : ICollection<TFlow> where TFlow : IGatewayFlow
    {
        private readonly Gateway gateway;
        private readonly List<TFlow> flows;

        internal GatewayFlowCollection(Gateway gateway)
        {
            this.gateway = gateway;
            flows = new List<TFlow>();
        }

        public IEnumerator<TFlow> GetEnumerator()
        {
            return flows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TFlow flow)
        {
            gateway.ValidateAdd((IEnumerable<IGatewayFlow>) flows, flow);
            flows.Add(flow);
        }

        public void Clear()
        {
            flows.Clear();
        }

        public bool Contains(TFlow flow)
        {
            return gateway.Contains((IEnumerable<IGatewayFlow>) flows, flow);
        }

        void ICollection<TFlow>.CopyTo(TFlow[] array, int arrayIndex)
        {
            flows.CopyTo(array, arrayIndex);
        }

        public bool Remove(TFlow item)
        {
            return flows.Remove(item);
        }

        public int Count => flows.Count;

        bool ICollection<TFlow>.IsReadOnly => false;
    }
}