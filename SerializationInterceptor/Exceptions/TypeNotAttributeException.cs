using SerializationInterceptor.Utilities;
using System;

namespace SerializationInterceptor.Exceptions
{
	public sealed class TypeNotAttributeException : Exception
	{
		internal TypeNotAttributeException(Type type)
			: base($"Type {type.GetTypePrettyName()} is not an attribute")
		{
		}
	}
}
