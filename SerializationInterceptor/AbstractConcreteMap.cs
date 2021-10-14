using SerializationInterceptor.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SerializationInterceptor
{
	/// <summary>
	/// Represents a mapping between the abstract types and their concrete implementations. This mapping will be used during deserialization.
	/// By abstract type is meant any type that cannot be instantiated(enumerables not counted).
	/// By concrete type is meant any type that can be instantiated.
	/// </summary>
	public sealed class AbstractConcreteMap : IDictionary<Type, Type>
	{
		private readonly IDictionary<Type, Type> _map = new Dictionary<Type, Type>();

		public Type this[Type key] { get => _map[key]; set => _map[key] = value; }

		public ICollection<Type> Keys => _map.Keys;

		public ICollection<Type> Values => _map.Values;

		public int Count => _map.Count;

		public bool IsReadOnly => _map.IsReadOnly;

		/// <param name="abstract">An abstract type</param>
		/// <param name="concrete">A concrete type</param>
		/// <exception cref="ArgumentNullException">Thrown if a null is passed for any of the arguments</exception>
		/// <exception cref="ArgumentException">Thrown in any of the cases:
		/// -The key does not contain an abstract type
		/// -The value does not contain a concrete type
		/// -The concrete type is not assignable to the abstract type</exception>
		public void Add(Type @abstract, Type concrete)
		{
			Validate(@abstract, concrete);
			_map.Add(@abstract, concrete);
		}

		/// <param name="abstractConcretePair">A key-value pair containing an abstract type in the key and a concrete type in the value</param>
		/// <exception cref="ArgumentNullException">Thrown if a null is passed for the key or the value</exception>
		/// <exception cref="ArgumentException">Thrown in any of the cases:
		/// -The key does not contain an abstract type
		/// -The value does not contain a concrete type
		/// -The concrete type is not assignable to the abstract type</exception>
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
			if (!@abstract.IsAbstract && !@abstract.IsInterface) throw new ArgumentException($"Type {@abstract.GetTypePrettyName()} is not abstract", nameof(@abstract));
			if (concrete.IsAbstract || concrete.IsInterface) throw new ArgumentException($"Type {concrete.GetTypePrettyName()} is not concrete", nameof(concrete));
			if (!@abstract.IsAssignableFrom(concrete)) throw new ArgumentException($"Type {concrete.GetTypePrettyName()} is not assignable to type {@abstract.GetTypePrettyName()}", nameof(concrete));
		}
	}
}
