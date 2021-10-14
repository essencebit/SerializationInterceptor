using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SerializationInterceptor.Extensions
{
	internal static class EnumerableExtensions
	{
		public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> enumerable)
			=> enumerable == null
				? null
				: new ReadOnlyCollection<T>(enumerable.ToList());
	}
}
