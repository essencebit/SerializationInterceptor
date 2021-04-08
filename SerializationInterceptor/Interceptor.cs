using SerializationInterceptor.Enums;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SerializationInterceptor
{
    //TODO: document thrown exceptions
    public class Interceptor
    {
        #region serialization
        #region sync
        public static void Serialize<T>(T obj, Stream stream, Action<object, Type, Stream> serialization)
        {
            ObjectToClone(obj, out object objClone, out Type objCloneType);
            serialization.Invoke(objClone, objCloneType, stream);
        }

        public static void Serialize(object obj, Type objType, Stream stream, Action<object, Type, Stream> serialization)
        {
            ObjectToClone(obj, objType, out object objClone, out Type objCloneType);
            serialization.Invoke(objClone, objCloneType, stream);
        }

        public static string Serialize<T>(T obj, Func<object, Type, string> serialization)
        {
            ObjectToClone(obj, out object objClone, out Type objCloneType);
            return serialization.Invoke(objClone, objCloneType);
        }

        public static string Serialize(object obj, Type objType, Func<object, Type, string> serialization)
        {
            ObjectToClone(obj, objType, out object objClone, out Type objCloneType);
            return serialization.Invoke(objClone, objCloneType);
        }
        #endregion

        #region async
        public static Task SerializeAsync<T>(T obj, Stream stream, Func<object, Type, Stream, Task> serialization)
        {
            ObjectToClone(obj, out object objClone, out Type objCloneType);
            return serialization.Invoke(objClone, objCloneType, stream);
        }

        public static Task SerializeAsync(object obj, Type objType, Stream stream, Func<object, Type, Stream, Task> serialization)
        {
            ObjectToClone(obj, objType, out object objClone, out Type objCloneType);
            return serialization.Invoke(objClone, objCloneType, stream);
        }

        public static Task<string> SerializeAsync<T>(T obj, Func<object, Type, Task<string>> serialization)
        {
            ObjectToClone(obj, out object objClone, out Type objCloneType);
            return serialization.Invoke(objClone, objCloneType);
        }

        public static Task<string> SerializeAsync(object obj, Type objType, Func<object, Type, Task<string>> serialization)
        {
            ObjectToClone(obj, objType, out object objClone, out Type objCloneType);
            return serialization.Invoke(objClone, objCloneType);
        }
        #endregion
        #endregion serialization

        #region deserialization
        #region sync
        public static T Deserialize<T>(string @string, Func<string, Type, object> deserialization)
        {
            var objCloneType = TypeCloneFactory.CloneType<T>(Operation.Deserialization);
            var objClone = deserialization.Invoke(@string, objCloneType);
            return CloneToObject<T>(objClone, objCloneType);
        }

        public static object Deserialize(string @string, Type objType, Func<string, Type, object> deserialization)
        {
            var objCloneType = TypeCloneFactory.CloneType(Operation.Deserialization, objType);
            var objClone = deserialization.Invoke(@string, objCloneType);
            return CloneToObject(objType, objClone, objCloneType);
        }

        public static T Deserialize<T>(Stream stream, Func<Stream, Type, object> deserialization)
        {
            var objCloneType = TypeCloneFactory.CloneType<T>(Operation.Deserialization);
            var objClone = deserialization.Invoke(stream, objCloneType);
            return CloneToObject<T>(objClone, objCloneType);
        }

        public static object Deserialize(Stream stream, Type objType, Func<Stream, Type, object> deserialization)
        {
            var objCloneType = TypeCloneFactory.CloneType(Operation.Deserialization, objType);
            var objClone = deserialization.Invoke(stream, objCloneType);
            return CloneToObject(objType, objClone, objCloneType);
        }
        #endregion

        #region async
        public static Task<T> DeserializeAsync<T>(string @string, Func<string, Type, Task<object>> deserialization)
        {
            var objCloneType = TypeCloneFactory.CloneType<T>(Operation.Deserialization);
            var objClone = deserialization.Invoke(@string, objCloneType).ConfigureAwait(false).GetAwaiter().GetResult();
            var obj = CloneToObject<T>(objClone, objCloneType);
            return Task.FromResult(obj);
        }

        public static Task<object> DeserializeAsync(string @string, Type objType, Func<string, Type, Task<object>> deserialization)
        {
            var objCloneType = TypeCloneFactory.CloneType(Operation.Deserialization, objType);
            var objClone = deserialization.Invoke(@string, objCloneType).ConfigureAwait(false).GetAwaiter().GetResult();
            var obj = CloneToObject(objType, objClone, objCloneType);
            return Task.FromResult(obj);
        }

        public static Task<T> DeserializeAsync<T>(Stream stream, Func<Stream, Type, Task<object>> deserialization)
        {
            var objCloneType = TypeCloneFactory.CloneType<T>(Operation.Deserialization);
            var objClone = deserialization.Invoke(stream, objCloneType).ConfigureAwait(false).GetAwaiter().GetResult();
            var obj = CloneToObject<T>(objClone, objCloneType);
            return Task.FromResult(obj);
        }

        public static Task<object> DeserializeAsync(Stream stream, Type objType, Func<Stream, Type, Task<object>> deserialization)
        {
            var objCloneType = TypeCloneFactory.CloneType(Operation.Deserialization, objType);
            var objClone = deserialization.Invoke(stream, objCloneType).ConfigureAwait(false).GetAwaiter().GetResult();
            var obj = CloneToObject(objType, objClone, objCloneType);
            return Task.FromResult(obj);
        }
        #endregion
        #endregion deserialization

        #region private
        private static void ObjectToClone<T>(T obj, out object objClone, out Type objCloneType)
        {
            var objType = typeof(T);
            ObjectToClone(obj, objType, out objClone, out objCloneType);
        }

        private static void ObjectToClone(object obj, Type objType, out object objClone, out Type objCloneType)
        {
            objCloneType = TypeCloneFactory.CloneType(Operation.Serialization, objType);
            objClone = PureAutoMapper.Map(obj, objType, objCloneType);
        }

        private static T CloneToObject<T>(object objClone, Type objCloneType)
        {
            var objType = typeof(T);
            return (T)CloneToObject(objType, objClone, objCloneType);
        }

        private static object CloneToObject(Type objType, object objClone, Type objCloneType)
            => PureAutoMapper.Map(objClone, objCloneType, objType);
        #endregion
    }
}
