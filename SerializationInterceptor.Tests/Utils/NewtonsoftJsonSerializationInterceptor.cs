using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SerializationInterceptor.Tests.Utils
{
    public class NewtonsoftJsonSerializationInterceptor
    {
        public static string Serialize<T>(T obj)
        {
            return Interceptor.InterceptSerialization(obj, (o, t) =>
            {
                var serializer = new JsonSerializer();
                serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                using var stream = new MemoryStream();
                using var streamWriter = new StreamWriter(stream);
                using var jsonTextWriter = new JsonTextWriter(streamWriter);
                serializer.Serialize(jsonTextWriter, o, t);
                jsonTextWriter.Flush();
                return Encoding.Default.GetString(stream.ToArray());
            });
        }

        public static Task<string> SerializeAsync<T>(T obj)
        {
            return Interceptor.InterceptSerializationAsync(obj, (o, t) =>
            {
                var serializer = new JsonSerializer();
                serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                using var stream = new MemoryStream();
                using var streamWriter = new StreamWriter(stream);
                using var jsonTextWriter = new JsonTextWriter(streamWriter);
                serializer.Serialize(jsonTextWriter, o, t);
                jsonTextWriter.Flush();
                return Task.FromResult(Encoding.Default.GetString(stream.ToArray()));
            });
        }

        public static T Deserialize<T>(string @string, AbstractConcreteMap abstractConcreteMap = null)
        {
            return Interceptor.InterceptDeserialization<T>(
                @string,
                (s, t) =>
                {
                    var serializer = new JsonSerializer();
                    using var stream = new MemoryStream(Encoding.UTF8.GetBytes(s));
                    using var streamReader = new StreamReader(stream);
                    using var jsonTextReader = new JsonTextReader(streamReader);
                    return serializer.Deserialize(jsonTextReader, t);
                },
                abstractConcreteMap);
        }

        public static Task<T> DeserializeAsync<T>(string @string, AbstractConcreteMap abstractConcreteMap = null)
        {
            return Interceptor.InterceptDeserializationAsync<T>(
                @string,
                (s, t) =>
                {
                    var serializer = new JsonSerializer();
                    using var stream = new MemoryStream(Encoding.UTF8.GetBytes(s));
                    using var streamReader = new StreamReader(stream);
                    using var jsonTextReader = new JsonTextReader(streamReader);
                    return Task.FromResult(serializer.Deserialize(jsonTextReader, t));
                },
                abstractConcreteMap);
        }
    }
}
