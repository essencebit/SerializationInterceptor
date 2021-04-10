using Newtonsoft.Json;
using SerializationInterceptor;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class NewtonsoftJsonSerializationInterceptor
    {
        #region Serialization
        #region Sync
        public static string Serialize<T>(T obj)
        {
            return Interceptor.Serialize(obj, (o, t) =>
            {
                var serializer = new JsonSerializer();
                using var stream = new MemoryStream();
                using var streamWriter = new StreamWriter(stream);
                using var jsonTextWriter = new JsonTextWriter(streamWriter);
                serializer.Serialize(jsonTextWriter, o, t);
                jsonTextWriter.Flush();
                return Encoding.Default.GetString(stream.ToArray());
            });
        }
        #endregion Sync

        #region Async
        public static Task<string> SerializeAsync<T>(T obj)
        {
            return Interceptor.SerializeAsync(obj, (o, t) =>
            {
                var serializer = new JsonSerializer();
                using var stream = new MemoryStream();
                using var streamWriter = new StreamWriter(stream);
                using var jsonTextWriter = new JsonTextWriter(streamWriter);
                serializer.Serialize(jsonTextWriter, o, t);
                jsonTextWriter.Flush();
                return Task.FromResult(Encoding.Default.GetString(stream.ToArray()));
            });
        }
        #endregion Async
        #endregion Serialization

        #region Deserialization
        #region Sync
        public static T Deserialize<T>(string @string, AbstractConcreteMap abstractConcreteMap = null)
        {
            return Interceptor.Deserialize<T>(@string, (s, t) =>
            {
                var serializer = new JsonSerializer();
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(s));
                using var streamReader = new StreamReader(stream);
                using var jsonTextReader = new JsonTextReader(streamReader);
                return serializer.Deserialize(jsonTextReader, t);
            }, abstractConcreteMap);
        }
        #endregion Sync

        #region Async
        public static Task<T> DeserializeAsync<T>(string @string, AbstractConcreteMap abstractConcreteMap = null)
        {
            return Interceptor.DeserializeAsync<T>(@string, (s, t) =>
            {
                var serializer = new JsonSerializer();
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(s));
                using var streamReader = new StreamReader(stream);
                using var jsonTextReader = new JsonTextReader(streamReader);
                return Task.FromResult(serializer.Deserialize(jsonTextReader, t));
            }, abstractConcreteMap);
        }
        #endregion Async
        #endregion Deserialization
    }
}
