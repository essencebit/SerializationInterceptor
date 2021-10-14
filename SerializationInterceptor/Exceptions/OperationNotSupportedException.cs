using SerializationInterceptor.Enums;
using System;

namespace SerializationInterceptor.Exceptions
{
	public sealed class OperationNotSupportedException : Exception
	{
		internal OperationNotSupportedException(Operation operation)
			: base($"Operation {operation} not supported")
		{
		}
	}
}
