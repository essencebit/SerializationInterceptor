using Newtonsoft.Json;
using System;
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
                var serializer = new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                using var stream = new MemoryStream();
                using var streamWriter = new StreamWriter(stream);
                using var jsonTextWriter = new JsonTextWriter(streamWriter);
                serializer.Serialize(jsonTextWriter, o, t);
                jsonTextWriter.Flush();
                return Encoding.Default.GetString(stream.ToArray());
            });
        }

        public static string Serialize(object obj, Type objType)
        {
            return Interceptor.InterceptSerialization(obj, objType, (o, t) =>
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

        public static void Serialize<T>(T obj, Stream stream)
        {
            Interceptor.InterceptSerialization(obj, stream, (o, t, s) =>
            {
                var serializer = new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                using var streamWriter = new StreamWriter(s);
                using var jsonTextWriter = new JsonTextWriter(streamWriter);
                serializer.Serialize(jsonTextWriter, o, t);
                jsonTextWriter.Flush();
            });
        }

        public static void Serialize(object obj, Type objType, Stream stream)
        {
            Interceptor.InterceptSerialization(obj, objType, stream, (o, t, s) =>
            {
                var serializer = new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                using var streamWriter = new StreamWriter(s);
                using var jsonTextWriter = new JsonTextWriter(streamWriter);
                serializer.Serialize(jsonTextWriter, o, t);
                jsonTextWriter.Flush();
            });
        }

        public static Task<string> SerializeAsync<T>(T obj)
        {
            return Interceptor.InterceptSerializationAsync(obj, async (o, t) =>
            {
                var serializer = new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                using var stream = new MemoryStream();
                using var streamWriter = new StreamWriter(stream);
                using var jsonTextWriter = new JsonTextWriter(streamWriter);
                serializer.Serialize(jsonTextWriter, o, t);
                jsonTextWriter.Flush();
                return await Task.FromResult(Encoding.Default.GetString(stream.ToArray()));
            });
        }

        public static async Task<string> SerializeAsync(object obj, Type objType)
        {
            return await Interceptor.InterceptSerializationAsync(obj, objType, async (o, t) =>
            {
                var serializer = new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                using var stream = new MemoryStream();
                using var streamWriter = new StreamWriter(stream);
                using var jsonTextWriter = new JsonTextWriter(streamWriter);
                serializer.Serialize(jsonTextWriter, o, t);
                jsonTextWriter.Flush();
                return await Task.FromResult(Encoding.Default.GetString(stream.ToArray()));
            });
        }

        public static async Task SerializeAsync<T>(T obj, Stream stream)
        {
            await Interceptor.InterceptSerializationAsync(obj, stream, async (o, t, s) =>
            {
                var serializer = new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                using var streamWriter = new StreamWriter(s);
                using var jsonTextWriter = new JsonTextWriter(streamWriter);
                serializer.Serialize(jsonTextWriter, o, t);
                jsonTextWriter.Flush();
                await Task.Yield();
            });
        }

        public static async Task SerializeAsync(object obj, Type objType, Stream stream)
        {
            await Interceptor.InterceptSerializationAsync(obj, objType, stream, async (o, t, s) =>
            {
                var serializer = new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                using var streamWriter = new StreamWriter(s);
                using var jsonTextWriter = new JsonTextWriter(streamWriter);
                serializer.Serialize(jsonTextWriter, o, t);
                jsonTextWriter.Flush();
                await Task.Yield();
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

        public static object Deserialize(string @string, Type objType, AbstractConcreteMap abstractConcreteMap = null)
        {
            return Interceptor.InterceptDeserialization(
                @string,
                objType,
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

        public static T Deserialize<T>(Stream stream, AbstractConcreteMap abstractConcreteMap = null)
        {
            return Interceptor.InterceptDeserialization<T>(
                stream,
                (s, t) =>
                {
                    var serializer = new JsonSerializer();
                    using var streamReader = new StreamReader(s);
                    using var jsonTextReader = new JsonTextReader(streamReader);
                    return serializer.Deserialize(jsonTextReader, t);
                },
                abstractConcreteMap);
        }

        public static object Deserialize(Stream @string, Type objType, AbstractConcreteMap abstractConcreteMap = null)
        {
            return Interceptor.InterceptDeserialization(
                @string,
                objType,
                (s, t) =>
                {
                    var serializer = new JsonSerializer();
                    using var streamReader = new StreamReader(s);
                    using var jsonTextReader = new JsonTextReader(streamReader);
                    return serializer.Deserialize(jsonTextReader, t);
                },
                abstractConcreteMap);
        }

        public static Task<T> DeserializeAsync<T>(string @string, AbstractConcreteMap abstractConcreteMap = null)
        {
            return Interceptor.InterceptDeserializationAsync<T>(
                @string,
                async (s, t) =>
                {
                    var serializer = new JsonSerializer();
                    using var stream = new MemoryStream(Encoding.UTF8.GetBytes(s));
                    using var streamReader = new StreamReader(stream);
                    using var jsonTextReader = new JsonTextReader(streamReader);
                    return await Task.FromResult(serializer.Deserialize(jsonTextReader, t));
                },
                abstractConcreteMap);
        }

        public static async Task<object> DeserializeAsync(string @string, Type objType, AbstractConcreteMap abstractConcreteMap = null)
        {
            return await Interceptor.InterceptDeserializationAsync(
                @string,
                objType,
                async (s, t) =>
                {
                    var serializer = new JsonSerializer();
                    using var stream = new MemoryStream(Encoding.UTF8.GetBytes(s));
                    using var streamReader = new StreamReader(stream);
                    using var jsonTextReader = new JsonTextReader(streamReader);
                    return await Task.FromResult(serializer.Deserialize(jsonTextReader, t));
                },
                abstractConcreteMap);
        }

        public static async Task<T> DeserializeAsync<T>(Stream stream, AbstractConcreteMap abstractConcreteMap = null)
        {
            return await Interceptor.InterceptDeserializationAsync<T>(
                stream,
                async (s, t) =>
                {
                    var serializer = new JsonSerializer();
                    using var streamReader = new StreamReader(s);
                    using var jsonTextReader = new JsonTextReader(streamReader);
                    return await Task.FromResult(serializer.Deserialize(jsonTextReader, t));
                },
                abstractConcreteMap);
        }

        public static async Task<object> DeserializeAsync(Stream @string, Type objType, AbstractConcreteMap abstractConcreteMap = null)
        {
            return await Interceptor.InterceptDeserializationAsync(
                @string,
                objType,
                async (s, t) =>
                {
                    var serializer = new JsonSerializer();
                    using var streamReader = new StreamReader(s);
                    using var jsonTextReader = new JsonTextReader(streamReader);
                    return await Task.FromResult(serializer.Deserialize(jsonTextReader, t));
                },
                abstractConcreteMap);
        }
    }
}
