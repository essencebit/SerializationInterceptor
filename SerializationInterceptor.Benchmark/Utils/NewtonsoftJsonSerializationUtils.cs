using Newtonsoft.Json;

namespace SerializationInterceptor.Benchmark.Utils
{
	internal class NewtonsoftJsonSerializationUtils
	{
		public static string InterceptSerialization<T>(T obj)
		{
			return Interceptor.InterceptSerialization(obj, (o, t) =>
			{
				return JsonConvert.SerializeObject(o, t, null);
			});
		}

		public static T InterceptDeserialization<T>(string @string)
		{
			return Interceptor.InterceptDeserialization<T>(
				@string,
				(s, t) =>
				{
					return JsonConvert.DeserializeObject(s, t);
				});
		}
	}
}
