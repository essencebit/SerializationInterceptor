using System.Text.Json;
using System.Text.Json.Serialization;

namespace SerializationInterceptor.Benchmark.Utils
{
    internal class SystemTextJsonSerializationUtils
    {
        public static string InterceptSerialization<T>(T obj)
        {
            return Interceptor.InterceptSerialization(obj, (o, t) =>
            {
                return JsonSerializer.Serialize(o, t, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
            });
        }

        public static T InterceptDeserialization<T>(string @string)
        {
            return Interceptor.InterceptDeserialization<T>(
                @string,
                (s, t) =>
                {
                    return JsonSerializer.Deserialize(s, t, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
                });
        }
    }
}
