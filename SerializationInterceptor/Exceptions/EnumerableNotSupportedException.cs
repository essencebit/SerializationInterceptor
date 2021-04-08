using System;

namespace SerializationInterceptor.Exceptions
{
    public class EnumerableNotSupportedException : Exception
    {
        public EnumerableNotSupportedException(string message)
            : base(message)
        {
        }
    }
}
