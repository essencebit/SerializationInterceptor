using System.Collections.Generic;
using System.Reflection;

namespace SerializationInterceptor.Attributes
{
    /// <summary>
    /// An instance of this class encapsulates all necessary params for building an attribute
    /// </summary>
    public sealed class AttributeBuilderParams
    {
        public ConstructorInfo Constructor { get; set; }
        public IEnumerable<object> ConstructorArgs { get; set; }
        public IEnumerable<AttributeBuilderNamedPropParam> NamedProps { get; set; }
        public IEnumerable<AttributeBuilderNamedFieldParam> NamedFields { get; set; }
    }

    public sealed class AttributeBuilderNamedPropParam
    {
        public PropertyInfo NamedProp { get; set; }
        public object PropValue { get; set; }
    }

    public sealed class AttributeBuilderNamedFieldParam
    {
        public FieldInfo NamedField { get; set; }
        public object FieldValue { get; set; }
    }
}
