using Newtonsoft.Json;
using SerializationInterceptor.Attributes;

namespace SerializationInterceptor.Tests.Attributes
{
    internal class JsonPropertyInterceptorAttribute : InterceptorAttribute
    {
        public JsonPropertyInterceptorAttribute(string interceptorId)
            : base(interceptorId, typeof(JsonPropertyAttribute))
        {
        }

        protected override AttributeBuilderParams Intercept(AttributeBuilderParams originalAttributeBuilderParams, object context)
        {
            originalAttributeBuilderParams.ConstructorArgs = new[] { InterceptorId };
            return originalAttributeBuilderParams;
        }
    }
}
