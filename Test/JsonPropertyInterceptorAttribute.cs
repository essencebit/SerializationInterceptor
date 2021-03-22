using SerializationInterceptor.Attributes;
using System;
using System.Linq;

namespace Test
{
    public class JsonPropertyInterceptorAttribute : InterceptorAttribute
    {
        public JsonPropertyInterceptorAttribute(string interceptorKey, Type typeOfAttributeToIntercept)
            : base(interceptorKey, typeOfAttributeToIntercept)
        {
        }

        protected override AttributeBuilderParams Intercept(AttributeBuilderParams originalAttributeBuilderParams)
        {
            switch (InterceptorId)
            {
                case "key":
                    var copy = originalAttributeBuilderParams.ConstructorArgs.ToList();
                    copy[0] = "altered";
                    originalAttributeBuilderParams.ConstructorArgs = copy;
                    break;
                default:
                    break;
            }

            return originalAttributeBuilderParams;
        }
    }
}
