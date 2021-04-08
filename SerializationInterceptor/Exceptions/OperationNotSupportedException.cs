using System;

namespace SerializationInterceptor.Exceptions
{
    public sealed class OperationNotSupportedException : Exception
    {
        public OperationNotSupportedException(string message)
            : base(message)
        {
        }
    }
}
