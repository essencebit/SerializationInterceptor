using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SerializationInterceptor.Extensions
{
	internal static class TypeExtensions
	{
		private static readonly IEnumerable<TypeCode> _numericTypes = new List<TypeCode>
		{
			TypeCode.SByte, TypeCode.Byte,
			TypeCode.Int16, TypeCode.UInt16,
			TypeCode.Int32, TypeCode.UInt32,
			TypeCode.Int64, TypeCode.UInt64,
			TypeCode.Single, TypeCode.Double,
			TypeCode.Decimal
		};

		public static bool IsNumeric(this Type type) => !type.IsEnum && _numericTypes.Contains(Type.GetTypeCode(type));
		public static bool IsEnumerable(this Type type) => type == typeof(IEnumerable) || type.GetInterfaces().Any(x => x == typeof(IEnumerable));
		public static bool IsGenericEnumerable(this Type type) => (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) || type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
		public static bool IsGenericCollection(this Type type) => (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>)) || type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));
		public static bool IsGenericDictionary(this Type type) => (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>)) || type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>));
	}
}
