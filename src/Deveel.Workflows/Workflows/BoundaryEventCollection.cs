using System;
using System.Collections;
using System.Collections.Generic;

namespace Deveel.Workflows
{
    public sealed class BoundaryEventCollection : ICollection<BoundaryEvent>
    {
        private readonly List<BoundaryEvent> events;
        private readonly Activity activity;

        internal BoundaryEventCollection(Activity activity)
        {
            this.activity = activity;
            events = new List<BoundaryEvent>();
        }

        public void Add(BoundaryEvent item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            item.AttachTo(activity);

            events.Add(item);
        }

        public void Clear()
        {
            foreach (var @event in events)
            {
                @event.DetachFrom(activity);
            }

            events.Clear();
        }

        public bool Contains(BoundaryEvent item)
        {
            return events.Contains(item);
        }

        void ICollection<BoundaryEvent>.CopyTo(BoundaryEvent[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public bool Remove(BoundaryEvent item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (events.Remove(item))
            {
                item.DetachFrom(activity);
                return true;
            }

            return false;
        }

        public int Count => events.Count;

        bool ICollection<BoundaryEvent>.IsReadOnly => false;

        public IEnumerator<BoundaryEvent> GetEnumerator()
        {
            return events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
