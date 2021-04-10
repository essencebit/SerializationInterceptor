using System;

namespace SerializationInterceptor.Exceptions
{
    public sealed class EnumerableNotSupportedException : Exception
    {
        public EnumerableNotSupportedException(string message)
            : base(message)
        {
        }
    }
}
