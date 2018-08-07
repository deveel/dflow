using System;
using System.Collections;
using System.Collections.Generic;

namespace Deveel.Workflows
{
    public sealed class ProcessSequence : IEnumerable<FlowNode>
    {
        private readonly LinkedList<FlowNode> nodes;
        private readonly ISequenceHandler handler;

        internal ProcessSequence(ISequenceHandler handler) {
            this.handler = handler;
            nodes = new LinkedList<FlowNode>();
        }

        private LinkedListNode<FlowNode> FindNode(string id)
        {
            var node = nodes.First;
            while(node != null)
            {
                if (node.Value != null && node.Value.Id == id)
                    return node;

                node = node.Next;
            }

            return null;
        }

        public bool Contains(string nodeId)
        {
            return FindNode(nodeId) != null;
        }

        private bool Exists(FlowNode node) {
            var current = handler;

            while (current != null) {
                if (current.NodeExists(node))
                    return true;

                current = current.Parent;
            }

            return false;
        }

        private void AssertNotExists(FlowNode node) {
            if (Exists(node))
                throw new FlowException("A node with the same ID already exists in the process context");
        }

        private void SetParentHandler(FlowNode node) {
            if (node is ISequenceHandler)
                ((ISequenceHandler) node).Parent = handler;
        }

        public void Add(FlowNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            AssertNotExists(node);

            SetParentHandler(node);

            handler.OnNodeAttached(node);
            nodes.AddLast(node);
        }

        public void AddAfter(string nodeId, FlowNode node)
        {
            var listNode = FindNode(nodeId);

            if (listNode == null)
                throw new FlowException($"Reference node '{nodeId}' was not found in the sequence");

            AssertNotExists(node);
            SetParentHandler(node);

            handler.OnNodeAttached(node);
            nodes.AddAfter(listNode, node);
        }

        public void AddBefore(string nodeId, FlowNode node)
        {
            var listNode = FindNode(nodeId);

            if (listNode == null)
                throw new FlowException($"Reference node '{nodeId}' was not found in the sequence");

            AssertNotExists(node);
            SetParentHandler(node);

            handler.OnNodeAttached(node);
            nodes.AddBefore(listNode, node);
        }

        public void Remove(string nodeId)
        {
            var listNode = FindNode(nodeId);

            if (listNode == null)
                throw new FlowException($"Reference node '{nodeId}' was not found in the sequence");

            handler.OnNodeDetached(listNode.Value);
            nodes.Remove(listNode);
        }

        public void Clear()
        {
            foreach (var node in nodes) {
                handler.OnNodeDetached(node);
            }

            nodes.Clear();
        }

        public IEnumerator<FlowNode> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
