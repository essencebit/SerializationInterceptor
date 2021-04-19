using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace SerializationInterceptor.Benchmark.Utils
{
    internal class NewtonsoftJsonSerializationUtils
    {
        public static string InterceptSerialization<T>(T obj)
        {
            return Interceptor.InterceptSerialization(obj, (o, t) =>
            {
                return Serialize(o, t);
            });
        }

        public static string Serialize(object o, Type t)
        {
            var serializer = new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            var jsonTextWriter = new JsonTextWriter(streamWriter);
            serializer.Serialize(jsonTextWriter, o, t);
            jsonTextWriter.Flush();
            return Encoding.Default.GetString(stream.ToArray());
        }

        public static T InterceptDeserialization<T>(string @string)
        {
            return Interceptor.InterceptDeserialization<T>(
                @string,
                (s, t) =>
                {
                    return Deserialize(s, t);
                });
        }

        public static object Deserialize(string s, Type t)
        {
            var serializer = new JsonSerializer();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(s));
            var streamReader = new StreamReader(stream);
            var jsonTextReader = new JsonTextReader(streamReader);
            return serializer.Deserialize(jsonTextReader, t);
        }
    }
}
