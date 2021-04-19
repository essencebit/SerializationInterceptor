using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SerializationInterceptor.Tests.Utils
{
    public class SystemTextJsonSerializationInterceptor
    {
        public static string Serialize<T>(T obj)
        {
            return Interceptor.InterceptSerialization(obj, (o, t) =>
            {
                return JsonSerializer.Serialize(o, t, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
            });
        }

        public static string Serialize(object obj, Type objType)
        {
            return Interceptor.InterceptSerialization(obj, objType, (o, t) =>
            {
                return JsonSerializer.Serialize(o, t, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
            });
        }

        public static Task<string> SerializeAsync<T>(T obj)
        {
            return Interceptor.InterceptSerializationAsync(obj, async (o, t) =>
            {
                using var stream = new MemoryStream();
                await JsonSerializer.SerializeAsync(stream, o, t, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
                return Encoding.UTF8.GetString(stream.ToArray());
            });
        }

        public static Task<string> SerializeAsync(object obj, Type objType)
        {
            return Interceptor.InterceptSerializationAsync(obj, objType, async (o, t) =>
            {
                using var stream = new MemoryStream();
                await JsonSerializer.SerializeAsync(stream, o, t, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
                return Encoding.UTF8.GetString(stream.ToArray());
            });
        }

        public static T Deserialize<T>(string @string, AbstractConcreteMap abstractConcreteMap = null)
        {
            return Interceptor.InterceptDeserialization<T>(
                @string,
                (s, t) =>
                {
                    return JsonSerializer.Deserialize(s, t, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
                },
                abstractConcreteMap);
        }

        public static object Deserialize(string @string, Type objType, AbstractConcreteMap abstractConcreteMap = null)
        {
            return Interceptor.InterceptDeserialization(
                @string,
                objType,
                (s, t) =>
                {
                    return JsonSerializer.Deserialize(s, t, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
                },
                abstractConcreteMap);
        }

        public static Task<T> DeserializeAsync<T>(string @string, AbstractConcreteMap abstractConcreteMap = null)
        {
            return Interceptor.InterceptDeserializationAsync<T>(
                @string,
                async (s, t) =>
                {
                    using var stream = new MemoryStream();
                    using var writer = new StreamWriter(stream);
                    writer.Write(s);
                    writer.Flush();
                    stream.Position = 0;
                    var obj = await JsonSerializer.DeserializeAsync(stream, t, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
                    return obj;
                },
                abstractConcreteMap);
        }

        public static Task<object> DeserializeAsync(string @string, Type objType, AbstractConcreteMap abstractConcreteMap = null)
        {
            return Interceptor.InterceptDeserializationAsync(
                @string,
                objType,
                async (s, t) =>
                {
                    using var stream = new MemoryStream();
                    using var writer = new StreamWriter(stream);
                    writer.Write(s);
                    writer.Flush();
                    stream.Position = 0;
                    var obj = await JsonSerializer.DeserializeAsync(stream, t, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
                    return obj;
                },
                abstractConcreteMap);
        }
    }
}
