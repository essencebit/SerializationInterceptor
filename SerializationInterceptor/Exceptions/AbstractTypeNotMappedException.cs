using SerializationInterceptor.Utilities;
using System;

namespace SerializationInterceptor.Exceptions
{
	public sealed class AbstractTypeNotMappedException : Exception
	{
		internal AbstractTypeNotMappedException(Type abstractType)
			: base($"Abstract type {abstractType.GetTypePrettyName()} not mapped")
		{
		}
	}
}
