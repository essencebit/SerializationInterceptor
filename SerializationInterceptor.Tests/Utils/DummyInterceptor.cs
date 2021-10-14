using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace SerializationInterceptor.Tests.Utils
{
	public class DummyInterceptor
	{
		public static string Serialize<T>(T obj)
		{
			return Interceptor.InterceptSerialization(obj, (o, t) =>
			{
				var serializer = new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
				using var stream = new MemoryStream();
				using var streamWriter = new StreamWriter(stream);
				using var jsonTextWriter = new JsonTextWriter(streamWriter);
				serializer.Serialize(jsonTextWriter, o, t);
				jsonTextWriter.Flush();
				return Encoding.Default.GetString(stream.ToArray());
			});
		}
	}
}
