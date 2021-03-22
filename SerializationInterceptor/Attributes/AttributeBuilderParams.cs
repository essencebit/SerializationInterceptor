using System.Collections.Generic;
using System.Reflection;

namespace SerializationInterceptor.Attributes
{
    public class AttributeBuilderParams
    {
        public ConstructorInfo Constructor { get; set; }
        public IEnumerable<object> ConstructorArgs { get; set; }
        public IEnumerable<AttributeBuilderNamedPropParam> NamedProps { get; set; }
        public IEnumerable<AttributeBuilderNamedFieldParam> NamedFields { get; set; }
    }

    public class AttributeBuilderNamedPropParam
    {
        public PropertyInfo NamedProp { get; set; }
        public object PropValue { get; set; }
    }

    public class AttributeBuilderNamedFieldParam
    {
        public FieldInfo NamedField { get; set; }
        public object FieldValue { get; set; }
    }
}
