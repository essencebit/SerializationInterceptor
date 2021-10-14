using SerializationInterceptor.Attributes;
using System.Linq;
using System.Text.Json.Serialization;

namespace SerializationInterceptor.Tests.Attributes
{
	internal class JsonPropertyNameInterceptorAttribute : InterceptorAttribute
	{
		public JsonPropertyNameInterceptorAttribute(string interceptorId)
			: base(interceptorId, typeof(JsonPropertyNameAttribute))
		{
		}

		protected override void Intercept(in AttributeParams attributeBuilderParams, object context)
		{
			attributeBuilderParams.ConstructorArgs.First().ArgValue = InterceptorId;
		}
	}
}
