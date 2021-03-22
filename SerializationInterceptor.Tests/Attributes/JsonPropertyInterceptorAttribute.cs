using Newtonsoft.Json;
using SerializationInterceptor.Attributes;

namespace SerializationInterceptor.Tests.Attributes
{
    public class JsonPropertyInterceptorAttribute : InterceptorAttribute
    {
        public JsonPropertyInterceptorAttribute(string interceptorKey)
            : base(interceptorKey, typeof(JsonPropertyAttribute))
        {
        }

        protected override AttributeBuilderParams Intercept(AttributeBuilderParams originalAttributeBuilderParams)
        {
            originalAttributeBuilderParams.ConstructorArgs = new[] { InterceptorId };
            return originalAttributeBuilderParams;
        }
    }
}
