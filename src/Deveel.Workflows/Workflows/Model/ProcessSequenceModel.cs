using System;
using System.Collections;
using System.Collections.Generic;

namespace Deveel.Workflows.Model
{
    public sealed class ProcessSequenceModel : IEnumerable<FlowNodeModel>
    {
        private LinkedList<FlowNodeModel> objects;

        public ProcessSequenceModel() {
            objects = new LinkedList<FlowNodeModel>();
        }

        public FlowNodeModel StartActivity => objects.First?.Value;

        public FlowNodeModel EndActivity => objects.Last?.Value;

        public int Count => objects.Count;

        private bool Exists(string activityName)
        {
            var node = objects.First;
            while (node != null)
            {
                if (node.Value.Id == activityName)
                    return true;

                node = node.Next;
            }

            return false;
        }

        public void Add(FlowNodeModel obj)
        {
            if (Exists(obj.Id))
                throw new ArgumentException($"A flow object with a name '{0}' is already in the sequence");

            objects.AddLast(obj);
        }

        public void AddAfter(string name, FlowNodeModel obj)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var node = objects.First;
            while (node != null && node.Value.Id != name)
                node = node.Next;

            if (node == null)
                throw new ArgumentException($"No flow object named '{name}' found in the sequence");

            if (Exists(obj.Id))
                throw new ArgumentException($"A flow object named '{obj.Id}' is already in the sequence");

            objects.AddAfter(node, obj);
        }

        public void AddBefore(string name, FlowNodeModel obj)
        {
            var node = objects.First;
            while (node != null && node.Value.Id != name)
                node = node.Next;

            if (node == null)
                throw new ArgumentException($"No object named '{name}' found in the sequence");

            if (Exists(obj.Id))
                throw new ArgumentException($"An object named '{obj.Id}' is already in the sequence");

            objects.AddBefore(node, obj);
        }

        public void Remove(string name)
        {
            var node = objects.First;
            while (node != null && node.Value.Id != name)
                node = node.Next;

            if (node == null)
                throw new ArgumentException($"No object named '{name}' found in the sequence");

            objects.Remove(node);
        }

        public IEnumerator<FlowNodeModel> GetEnumerator()
        {
            return objects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
