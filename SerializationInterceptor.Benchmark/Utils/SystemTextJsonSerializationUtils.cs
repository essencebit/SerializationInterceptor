using System.Text.Json;

namespace SerializationInterceptor.Benchmark.Utils
{
	internal class SystemTextJsonSerializationUtils
	{
		public static string InterceptSerialization<T>(T obj)
		{
			return Interceptor.InterceptSerialization(obj, (o, t) =>
			{
				return JsonSerializer.Serialize(o, t);
			});
		}

		public static T InterceptDeserialization<T>(string @string)
		{
			return Interceptor.InterceptDeserialization<T>(
				@string,
				(s, t) =>
				{
					return JsonSerializer.Deserialize(s, t);
				});
		}
	}
}
