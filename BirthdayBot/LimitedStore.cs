using System.Collections.Generic;

namespace BirthdayBot
{
    public class LimitedStore<T> // poor implementation for easier serialization
    {
        public int Capacity { get; set; }
        public Queue<T> InternalStore { get; set; } = new Queue<T>();

        public LimitedStore()
        {
        }

        public LimitedStore(int capacity, IEnumerable<T> items)
        {
            Capacity = capacity;
            foreach (var item in items)
            {
                InternalStore.Enqueue(item);
            }
        }
        public LimitedStore(int capacity)
        {
            Capacity = capacity;
        }

        public bool Contains(T item)
        {
            return InternalStore.Contains(item);
        }


        public void Push(T item)
        {
            if (InternalStore.Count >= Capacity)
            {
                while (InternalStore.Count >= Capacity)
                {
                    InternalStore.Dequeue();
                }
            }
            
            InternalStore.Enqueue(item);
        }

        public T Peek()
        {
            return InternalStore.Peek();
        }
    }
}