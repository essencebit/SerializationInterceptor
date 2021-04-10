using System;

namespace SerializationInterceptor.Exceptions
{
    public sealed class TypeNotAttributeException : Exception
    {
        public TypeNotAttributeException(Type type)
            : base($"The type {type.FullName} is not an attribute")
        {
        }
    }
}
