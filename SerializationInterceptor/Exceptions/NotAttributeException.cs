using System;

namespace SerializationInterceptor.Exceptions
{
    public sealed class NotAttributeException : Exception
    {
        public NotAttributeException(Type type)
            : base($"The type {type.FullName} is not an attribute")
        {
        }
    }
}
