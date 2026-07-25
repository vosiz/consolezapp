using System.Collections.Generic;

namespace ConsoleZapp
{
    internal class OrderedMap<TValue>
    {
        private readonly Dictionary<string, TValue> Map = new Dictionary<string, TValue>();
        private readonly List<string> Order = new List<string>();

        // Constructor
        public OrderedMap() { }

        // Gets the value for the given key, throwing if it isn't present
        public TValue this[string key]
        {
            get { return Map[key]; }
        }

        // Number of entries currently stored
        public int Count
        {
            get { return Map.Count; }
        }

        // Keys in insertion order
        public IEnumerable<string> Keys
        {
            get { return Order; }
        }

        // Values in insertion order
        public IEnumerable<TValue> Values
        {
            get
            {
                foreach (var key in Order)
                    yield return Map[key];
            }
        }

        // Adds a new key or updates an existing one's value in place - new keys are appended to the end of iteration order, existing keys keep their original position
        public void Set(string key, TValue value)
        {
            if (!Map.ContainsKey(key))
                Order.Add(key);

            Map[key] = value;
        }

        // Retrieves a value by key without throwing if it's missing
        public bool TryGetValue(string key, out TValue value)
        {
            return Map.TryGetValue(key, out value);
        }
    }
}
