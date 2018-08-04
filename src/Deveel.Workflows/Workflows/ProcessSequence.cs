using System;
using System.Collections;
using System.Collections.Generic;

namespace Deveel.Workflows
{
    public sealed class ProcessSequence : IEnumerable<FlowNode>
    {
        private readonly LinkedList<FlowNode> nodes;

        internal ProcessSequence()
        {
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

        private bool Exists(string objName)
        {
            return FindNode(objName) != null;
        }

        public void Add(FlowNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (Exists(node.Id))
                throw new ArgumentException();

            nodes.AddLast(node);
        }

        public void AddAfter(string objName, FlowNode node)
        {
            var listNode = FindNode(objName);

            if (listNode == null)
                throw new ArgumentException();

            nodes.AddAfter(listNode, node);
        }

        public void AddBefore(string objName, FlowNode node)
        {
            var listNode = FindNode(objName);

            if (listNode == null)
                throw new ArgumentException();

            nodes.AddBefore(listNode, node);
        }

        public void Remove(string id)
        {
            var node = FindNode(id);
            if (node == null)
                throw new ArgumentException($"None object with ID '{id}' in sequence");

            nodes.Remove(node);
        }

        public void Clear()
        {
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
