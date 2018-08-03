using System;
using System.Collections;
using System.Collections.Generic;

namespace Deveel.Workflows
{
    public sealed class ProcessSequence : IEnumerable<FlowNode>
    {
        private readonly LinkedList<FlowNode> objects;
        private readonly Process process;

        internal ProcessSequence(Process process)
        {
            this.process = process;
            objects = new LinkedList<FlowNode>();
        }

        private LinkedListNode<FlowNode> FindNode(string id)
        {
            var node = objects.First;
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

            objects.AddLast(node);
        }

        public void AddAfter(string objName, FlowNode node)
        {
            var listNode = FindNode(objName);

            if (listNode == null)
                throw new ArgumentException();

            objects.AddAfter(listNode, node);
        }

        public void AddBefore(string objName, FlowNode node)
        {
            var listNode = FindNode(objName);

            if (listNode == null)
                throw new ArgumentException();

            objects.AddBefore(listNode, node);
        }

        public void Remove(string id)
        {
            var node = FindNode(id);
            if (node == null)
                throw new ArgumentException($"None object with ID '{id}' in sequence");

            objects.Remove(node);
        }

        public void Clear()
        {
            objects.Clear();
        }

        public IEnumerator<FlowNode> GetEnumerator()
        {
            return objects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
