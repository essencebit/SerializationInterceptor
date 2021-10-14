using SerializationInterceptor.Utilities;
using System;
using System.Collections.Generic;

namespace SerializationInterceptor.Exceptions
{
	public sealed class EnumerableNotSupportedException : Exception
	{
		internal EnumerableNotSupportedException(Type enumerableType)
			: base($"Enumerable of type {enumerableType.GetTypePrettyName()} not supported. Allowed only arrays of any number of dimensions supported by CLR and types from System.Collections.Generic that implement {typeof(ICollection<>).GetTypePrettyName()} interface")
		{
		}
	}
}
