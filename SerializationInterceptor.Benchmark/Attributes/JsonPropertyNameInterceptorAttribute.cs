using SerializationInterceptor.Attributes;
using System.Text.Json.Serialization;

namespace SerializationInterceptor.Benchmark.Attributes
{
    internal class JsonPropertyNameInterceptorAttribute : InterceptorAttribute
    {
        public JsonPropertyNameInterceptorAttribute(string interceptorId)
            : base(interceptorId, typeof(JsonPropertyNameAttribute))
        {
        }

        protected override AttributeBuilderParams Intercept(AttributeBuilderParams originalAttributeBuilderParams)
        {
            originalAttributeBuilderParams.ConstructorArgs = new[] { InterceptorId };
            return originalAttributeBuilderParams;
        }
    }
}
