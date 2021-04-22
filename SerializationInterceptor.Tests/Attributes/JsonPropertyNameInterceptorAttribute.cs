using SerializationInterceptor.Attributes;
using System.Text.Json.Serialization;

namespace SerializationInterceptor.Tests.Attributes
{
    internal class JsonPropertyNameInterceptorAttribute : InterceptorAttribute
    {
        public JsonPropertyNameInterceptorAttribute(string interceptorId)
            : base(interceptorId, typeof(JsonPropertyNameAttribute))
        {
        }

        protected override AttributeBuilderParams Intercept(AttributeBuilderParams originalAttributeBuilderParams, object context)
        {
            originalAttributeBuilderParams.ConstructorArgs = new[] { InterceptorId };
            return originalAttributeBuilderParams;
        }
    }
}
