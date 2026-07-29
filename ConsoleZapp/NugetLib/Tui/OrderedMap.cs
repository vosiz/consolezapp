using System.Collections.Generic;

namespace ConsoleZapp
{
    public class OrderedMap<TValue>
    {
        public TValue this[string key] // throws if the key is missing
        {
            get { return Map[key]; }
        }

        public int Count // entries currently stored
        {
            get { return Map.Count; }
        }

        public IEnumerable<string> Keys // in insertion order
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

        private readonly Dictionary<string, TValue> Map = new Dictionary<string, TValue>();
        private readonly List<string> Order = new List<string>();

        // Constructor
        public OrderedMap() { }

        // Adds or updates a key's value in place
        // - new keys append, existing keys keep position
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
