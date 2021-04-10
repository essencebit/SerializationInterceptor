using System;

namespace SerializationInterceptor.Exceptions
{
    public sealed class TypeNotConcreteException : Exception
    {
        public TypeNotConcreteException(Type type)
            : base($"Type {type.FullName} cannot be instanciated")
        {
        }
    }
}
