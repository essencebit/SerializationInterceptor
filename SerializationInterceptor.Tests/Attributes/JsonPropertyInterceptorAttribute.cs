using Newtonsoft.Json;
using SerializationInterceptor.Attributes;
using System.Linq;

namespace SerializationInterceptor.Tests.Attributes
{
	internal class JsonPropertyInterceptorAttribute : InterceptorAttribute
	{
		public JsonPropertyInterceptorAttribute(string interceptorId)
			: base(interceptorId, typeof(JsonPropertyAttribute))
		{
		}

		protected override void Intercept(in AttributeParams attributeBuilderParams, object context)
		{
			attributeBuilderParams.ConstructorArgs.First().ArgValue = InterceptorId;
		}
	}
}
