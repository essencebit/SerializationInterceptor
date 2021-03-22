using System;
using System.Collections.Generic;
using System.Linq;

namespace SerializationInterceptor.Extensions
{
    public static class TypeExtensions
    {
        private static readonly IEnumerable<TypeCode> NumericTypes = new List<TypeCode>
        {
            TypeCode.SByte, TypeCode.Byte,
            TypeCode.Int16, TypeCode.UInt16,
            TypeCode.Int32, TypeCode.UInt32,
            TypeCode.Int64, TypeCode.UInt64,
            TypeCode.Single, TypeCode.Double,
            TypeCode.Decimal
        };

        public static bool IsNumeric(this Type type) => !type.IsEnum && NumericTypes.Contains(Type.GetTypeCode(type));
    }
}
