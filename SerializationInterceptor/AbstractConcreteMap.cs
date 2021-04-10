using System;
using System.Collections;
using System.Collections.Generic;

namespace SerializationInterceptor
{
    public sealed class AbstractConcreteMap : IDictionary<Type, Type>
    {
        private readonly IDictionary<Type, Type> _map = new Dictionary<Type, Type>();

        public Type this[Type key] { get => _map[key]; set => _map[key] = value; }

        public ICollection<Type> Keys => _map.Keys;

        public ICollection<Type> Values => _map.Values;

        public int Count => _map.Count;

        public bool IsReadOnly => _map.IsReadOnly;

        public void Add(Type @abstract, Type concrete)
        {
            Validate(@abstract, concrete);
            _map.Add(@abstract, concrete);
        }

        public void Add(KeyValuePair<Type, Type> abstractConcretePair)
        {
            Validate(abstractConcretePair.Key, abstractConcretePair.Value);
            _map.Add(abstractConcretePair);
        }

        public void Clear() => _map.Clear();

        public bool Contains(KeyValuePair<Type, Type> item) => _map.Contains(item);

        public bool ContainsKey(Type key) => _map.ContainsKey(key);

        public void CopyTo(KeyValuePair<Type, Type>[] array, int arrayIndex) => _map.CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<Type, Type>> GetEnumerator() => _map.GetEnumerator();

        public bool Remove(Type key) => _map.Remove(key);

        public bool Remove(KeyValuePair<Type, Type> item) => _map.Remove(item);

        public bool TryGetValue(Type key, out Type value) => _map.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_map).GetEnumerator();

        private static void Validate(Type @abstract, Type concrete)
        {
            if (@abstract == null) throw new ArgumentNullException(nameof(@abstract));
            if (concrete == null) throw new ArgumentNullException(nameof(concrete));
            if (!@abstract.IsAbstract && !@abstract.IsInterface) throw new ArgumentException($"Type {@abstract.FullName} is not an abstract or interface type", nameof(@abstract));
            if (concrete.IsAbstract || concrete.IsInterface) throw new ArgumentException($"Type {concrete.FullName} is not a concrete type", nameof(concrete));
            if (!@abstract.IsAssignableFrom(concrete)) throw new ArgumentException($"Type {concrete.FullName} is not assignable to type {@abstract.FullName}", nameof(concrete));
        }
    }
}
