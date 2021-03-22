using SerializationInterceptor;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Test
{
    public class SystemTextJsonSerializationInterceptor
    {
        #region Serialization
        #region Sync
        public static string Serialize<T>(T obj)
        {
            return Interceptor.Serialize(obj, (o, t) =>
            {
                return JsonSerializer.Serialize(o, t);
            });
        }
        #endregion Sync

        #region Async
        public static Task<string> SerializeAsync<T>(T obj)
        {
            return Interceptor.SerializeAsync(obj, async (o, t) =>
            {
                using var stream = new MemoryStream();
                await JsonSerializer.SerializeAsync(stream, o, t);
                return Encoding.UTF8.GetString(stream.ToArray());
            });
        }
        #endregion Async
        #endregion Serialization

        #region Deserialization
        #region Sync
        public static T Deserialize<T>(string @string)
        {
            return Interceptor.Deserialize<T>(@string, (s, t) =>
            {
                var span = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(s));
                return JsonSerializer.Deserialize(span, t);
            });
        }
        #endregion Sync

        #region Async
        public static Task<T> DeserializeAsync<T>(string @string)
        {
            return Interceptor.DeserializeAsync<T>(@string, async (s, t) =>
            {
                using var stream = new MemoryStream();
                using var writer = new StreamWriter(stream);
                writer.Write(s);
                writer.Flush();
                stream.Position = 0;
                var obj = await JsonSerializer.DeserializeAsync(stream, t);
                return obj;
            });
        }
        #endregion Async
        #endregion Deserialization
    }
}
