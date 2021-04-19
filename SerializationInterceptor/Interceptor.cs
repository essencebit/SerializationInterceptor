using SerializationInterceptor.Enums;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SerializationInterceptor
{
    /// <summary>
    /// Methods of this class can be used to intercept attribute params during serialization/deserialization process.
    /// IMPORTANT: Use this tool when no other option left. Avoid using it, as it drastically hits the performance due to heavy use of reflection behind the scene.
    /// LIMITATIONS:
    /// - Fields not supported. Only public non-static props are serialized/deserialized;
    /// - Root type and types of the props must be public(not declared in any non-public types either) and have a default parameterless constructor;
    /// - Only params of type and property attributes can be intercepted;
    /// - Not all enumerables supported. Allowed only arrays of any number of dimensions supported by CLR and types from System.Collections.Generic that implement generic ICollection interface;
    /// - Inheritance supported partially. If you have a prop of type X and assign to that prop a value of type Y and Y is a subclass of X, then only props of type X will be serialized/deserialized, those of type Y which are not in X will be ignored;
    /// </summary>
    public static class Interceptor
    {
        #region serialization
        #region sync
        /// <summary>
        /// Intercepts serialization
        /// </summary>
        /// <typeparam name="T">Type of the object to serialize</typeparam>
        /// <param name="obj">Object to serialize</param>
        /// <param name="stream">The stream the object will be serialized into</param>
        /// <param name="serialization">Serialization action</param>
        /// <exception cref="EnumerableNotSupportedException">Thrown when object contains an enumerable not supported by interceptor</exception>
        /// <exception cref="TypeNotAttributeException">Thrown when any of the evaluated attribute interceptors have as input param a type that is not an attribute</exception>
        public static void InterceptSerialization<T>(T obj, Stream stream, Action<object, Type, Stream> serialization)
        {
            ObjectToClone(obj, out object objClone, out Type objCloneType);
            serialization.Invoke(objClone, objCloneType, stream);
        }

        /// <summary>
        /// Intercepts serialization
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <param name="objType">Type of the object to serialize</param>
        /// <param name="stream">The stream the object will be serialized into</param>
        /// <param name="serialization">Serialization action</param>
        /// <exception cref="EnumerableNotSupportedException">Thrown when object contains an enumerable not supported by interceptor</exception>
        /// <exception cref="TypeNotAttributeException">Thrown when any of the evaluated attribute interceptors have as input param a type that is not an attribute</exception>
        public static void InterceptSerialization(object obj, Type objType, Stream stream, Action<object, Type, Stream> serialization)
        {
            ObjectToClone(obj, objType, out object objClone, out Type objCloneType);
            serialization.Invoke(objClone, objCloneType, stream);
        }

        /// <summary>
        /// Intercepts serialization
        /// </summary>
        /// <typeparam name="T">Type of the object to serialize</typeparam>
        /// <param name="obj">Object to serialize</param>
        /// <param name="serialization">Serialization function</param>
        /// <returns>Serialized object</returns>
        /// <exception cref="EnumerableNotSupportedException">Thrown when object contains an enumerable not supported by interceptor</exception>
        /// <exception cref="TypeNotAttributeException">Thrown when any of the evaluated attribute interceptors have as input param a type that is not an attribute</exception>
        public static string InterceptSerialization<T>(T obj, Func<object, Type, string> serialization)
        {
            ObjectToClone(obj, out object objClone, out Type objCloneType);
            return serialization.Invoke(objClone, objCloneType);
        }

        /// <summary>
        /// Intercepts serialization
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <param name="objType">Type of the object to serialize</param>
        /// <param name="serialization">Serialization function</param>
        /// <returns>Serialized object</returns>
        /// <exception cref="EnumerableNotSupportedException">Thrown when object contains an enumerable not supported by interceptor</exception>
        /// <exception cref="TypeNotAttributeException">Thrown when any of the evaluated attribute interceptors have as input param a type that is not an attribute</exception>
        public static string InterceptSerialization(object obj, Type objType, Func<object, Type, string> serialization)
        {
            ObjectToClone(obj, objType, out object objClone, out Type objCloneType);
            return serialization.Invoke(objClone, objCloneType);
        }
        #endregion

        #region async
        /// <summary>
        /// Intercepts serialization asyncronously
        /// </summary>
        /// <typeparam name="T">Type of the object to serialize</typeparam>
        /// <param name="obj">Object to serialize</param>
        /// <param name="stream">The stream the object will be serialized into</param>
        /// <param name="serialization">Serialization function</param>
        /// <returns></returns>
        /// <exception cref="EnumerableNotSupportedException">Thrown when object contains an enumerable not supported by interceptor</exception>
        /// <exception cref="TypeNotAttributeException">Thrown when any of the evaluated attribute interceptors have as input param a type that is not an attribute</exception>
        public static Task InterceptSerializationAsync<T>(T obj, Stream stream, Func<object, Type, Stream, Task> serialization)
        {
            ObjectToClone(obj, out object objClone, out Type objCloneType);
            return serialization.Invoke(objClone, objCloneType, stream);
        }

        /// <summary>
        /// Intercepts serialization asyncronously
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <param name="objType">Type of the object to serialize</param>
        /// <param name="stream">The stream the object will be serialized into</param>
        /// <param name="serialization">Serialization function</param>
        /// <returns></returns>
        /// <exception cref="EnumerableNotSupportedException">Thrown when object contains an enumerable not supported by interceptor</exception>
        /// <exception cref="TypeNotAttributeException">Thrown when any of the evaluated attribute interceptors have as input param a type that is not an attribute</exception>
        public static Task InterceptSerializationAsync(object obj, Type objType, Stream stream, Func<object, Type, Stream, Task> serialization)
        {
            ObjectToClone(obj, objType, out object objClone, out Type objCloneType);
            return serialization.Invoke(objClone, objCloneType, stream);
        }

        /// <summary>
        /// Intercepts serialization asyncronously
        /// </summary>
        /// <typeparam name="T">Type of the object to serialize</typeparam>
        /// <param name="obj">Object to serialize</param>
        /// <param name="serialization">Serialization function</param>
        /// <returns>Serialized object</returns>
        /// <exception cref="EnumerableNotSupportedException">Thrown when object contains an enumerable not supported by interceptor</exception>
        /// <exception cref="TypeNotAttributeException">Thrown when any of the evaluated attribute interceptors have as input param a type that is not an attribute</exception>
        public static Task<string> InterceptSerializationAsync<T>(T obj, Func<object, Type, Task<string>> serialization)
        {
            ObjectToClone(obj, out object objClone, out Type objCloneType);
            return serialization.Invoke(objClone, objCloneType);
        }

        /// <summary>
        /// Intercepts serialization asyncronously
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <param name="objType">Type of the object to serialize</param>
        /// <param name="serialization">Serialization function</param>
        /// <returns>Serialized object</returns>
        /// <exception cref="EnumerableNotSupportedException">Thrown when object contains an enumerable not supported by interceptor</exception>
        /// <exception cref="TypeNotAttributeException">Thrown when any of the evaluated attribute interceptors have as input param a type that is not an attribute</exception>
        public static Task<string> InterceptSerializationAsync(object obj, Type objType, Func<object, Type, Task<string>> serialization)
        {
            ObjectToClone(obj, objType, out object objClone, out Type objCloneType);
            return serialization.Invoke(objClone, objCloneType);
        }
        #endregion
        #endregion serialization

        #region deserialization
        #region sync
        /// <summary>
        /// Intercepts deserialization
        /// </summary>
        /// <typeparam name="T">Type of the object to deserialize</typeparam>
        /// <param name="dataSource">Data source</param>
        /// <param name="deserialization">Deserialization function</param>
        /// <param name="abstractConcreteMap">An object containing the mapping of abstract types and interfaces to their concrete implementations</param>
        /// <returns>Deserialized object</returns>
        /// <exception cref="EnumerableNotSupportedException">Thrown when object contains an enumerable not supported by interceptor</exception>
        /// <exception cref="TypeNotAttributeException">Thrown when any of the evaluated attribute interceptors have as input param a type that is not an attribute</exception>
        /// <exception cref="TypeNotConcreteException">Thrown when type of the object to be deserialized contains an abstract type or an interface(enumerables not counted) which are not included in abstract-concrete map</exception>
        public static T InterceptDeserialization<T>(string dataSource, Func<string, Type, object> deserialization, AbstractConcreteMap abstractConcreteMap = null)
        {
            var objCloneType = TypeCloneFactory.CloneType<T>(Operation.Deserialization);
            var objClone = deserialization.Invoke(dataSource, objCloneType);
            return CloneToObject<T>(objClone, abstractConcreteMap);
        }

        /// <summary>
        /// Intercepts deserialization
        /// </summary>
        /// <param name="dataSource">Data source</param>
        /// <param name="objType">Type of the object to deserialize</param>
        /// <param name="deserialization">Deserialization function</param>
        /// <param name="abstractConcreteMap">An object containing the mapping of abstract types and interfaces to their concrete implementations</param>
        /// <returns>Deserialized object</returns>
        /// <exception cref="EnumerableNotSupportedException">Thrown when object contains an enumerable not supported by interceptor</exception>
        /// <exception cref="TypeNotAttributeException">Thrown when any of the evaluated attribute interceptors have as input param a type that is not an attribute</exception>
        /// <exception cref="TypeNotConcreteException">Thrown when type of the object to be deserialized contains an abstract type or an interface(enumerables not counted) which are not included in abstract-concrete map</exception>
        public static object InterceptDeserialization(string dataSource, Type objType, Func<string, Type, object> deserialization, AbstractConcreteMap abstractConcreteMap = null)
        {
            var objCloneType = TypeCloneFactory.CloneType(Operation.Deserialization, objType);
            var objClone = deserialization.Invoke(dataSource, objCloneType);
            return CloneToObject(objClone, objType, abstractConcreteMap);
        }

        /// <summary>
        /// Intercepts deserialization
        /// </summary>
        /// <typeparam name="T">Type of the object to deserialize</typeparam>
        /// <param name="dataSource">Data source</param>
        /// <param name="deserialization">Deserialization function</param>
        /// <param name="abstractConcreteMap">An object containing the mapping of abstract types and interfaces to their concrete implementations</param>
        /// <returns>Deserialized object</returns>
        /// <exception cref="EnumerableNotSupportedException">Thrown when object contains an enumerable not supported by interceptor</exception>
        /// <exception cref="TypeNotAttributeException">Thrown when any of the evaluated attribute interceptors have as input param a type that is not an attribute</exception>
        /// <exception cref="TypeNotConcreteException">Thrown when type of the object to be deserialized contains an abstract type or an interface(enumerables not counted) which are not included in abstract-concrete map</exception>
        public static T InterceptDeserialization<T>(Stream dataSource, Func<Stream, Type, object> deserialization, AbstractConcreteMap abstractConcreteMap = null)
        {
            var objCloneType = TypeCloneFactory.CloneType<T>(Operation.Deserialization);
            var objClone = deserialization.Invoke(dataSource, objCloneType);
            return CloneToObject<T>(objClone, abstractConcreteMap);
        }

        /// <summary>
        /// Intercepts deserialization
        /// </summary>
        /// <param name="dataSource">Data source</param>
        /// <param name="objType">Type of the object to deserialize</param>
        /// <param name="deserialization">Deserialization function</param>
        /// <param name="abstractConcreteMap">An object containing the mapping of abstract types and interfaces to their concrete implementations</param>
        /// <returns>Deserialized object</returns>
        /// <exception cref="EnumerableNotSupportedException">Thrown when object contains an enumerable not supported by interceptor</exception>
        /// <exception cref="TypeNotAttributeException">Thrown when any of the evaluated attribute interceptors have as input param a type that is not an attribute</exception>
        /// <exception cref="TypeNotConcreteException">Thrown when type of the object to be deserialized contains an abstract type or an interface(enumerables not counted) which are not included in abstract-concrete map</exception>
        public static object InterceptDeserialization(Stream dataSource, Type objType, Func<Stream, Type, object> deserialization, AbstractConcreteMap abstractConcreteMap = null)
        {
            var objCloneType = TypeCloneFactory.CloneType(Operation.Deserialization, objType);
            var objClone = deserialization.Invoke(dataSource, objCloneType);
            return CloneToObject(objClone, objType, abstractConcreteMap);
        }
        #endregion

        #region async
        /// <summary>
        /// Intercepts deserialization asyncronously
        /// </summary>
        /// <typeparam name="T">Type of the object to deserialize</typeparam>
        /// <param name="dataSource">Data source</param>
        /// <param name="deserialization">Deserialization function</param>
        /// <param name="abstractConcreteMap">An object containing the mapping of abstract types and interfaces to their concrete implementations</param>
        /// <returns>Deserialized object</returns>
        /// <exception cref="EnumerableNotSupportedException">Thrown when object contains an enumerable not supported by interceptor</exception>
        /// <exception cref="TypeNotAttributeException">Thrown when any of the evaluated attribute interceptors have as input param a type that is not an attribute</exception>
        /// <exception cref="TypeNotConcreteException">Thrown when type of the object to be deserialized contains an abstract type or an interface(enumerables not counted) which are not included in abstract-concrete map</exception>
        public static Task<T> InterceptDeserializationAsync<T>(string dataSource, Func<string, Type, Task<object>> deserialization, AbstractConcreteMap abstractConcreteMap = null)
        {
            var objCloneType = TypeCloneFactory.CloneType<T>(Operation.Deserialization);
            var objClone = deserialization.Invoke(dataSource, objCloneType).ConfigureAwait(false).GetAwaiter().GetResult();
            var obj = CloneToObject<T>(objClone, abstractConcreteMap);
            return Task.FromResult(obj);
        }

        /// <summary>
        /// Intercepts deserialization asyncronously
        /// </summary>
        /// <param name="dataSource">Data source</param>
        /// <param name="objType">Type of the object to deserialize</param>
        /// <param name="deserialization">Deserialization function</param>
        /// <param name="abstractConcreteMap">An object containing the mapping of abstract types and interfaces to their concrete implementations</param>
        /// <returns>Deserialized object</returns>
        /// <exception cref="EnumerableNotSupportedException">Thrown when object contains an enumerable not supported by interceptor</exception>
        /// <exception cref="TypeNotAttributeException">Thrown when any of the evaluated attribute interceptors have as input param a type that is not an attribute</exception>
        /// <exception cref="TypeNotConcreteException">Thrown when type of the object to be deserialized contains an abstract type or an interface(enumerables not counted) which are not included in abstract-concrete map</exception>
        public static Task<object> InterceptDeserializationAsync(string dataSource, Type objType, Func<string, Type, Task<object>> deserialization, AbstractConcreteMap abstractConcreteMap = null)
        {
            var objCloneType = TypeCloneFactory.CloneType(Operation.Deserialization, objType);
            var objClone = deserialization.Invoke(dataSource, objCloneType).ConfigureAwait(false).GetAwaiter().GetResult();
            var obj = CloneToObject(objClone, objType, abstractConcreteMap);
            return Task.FromResult(obj);
        }

        /// <summary>
        /// Intercepts deserialization asyncronously
        /// </summary>
        /// <typeparam name="T">Type of the object to deserialize</typeparam>
        /// <param name="dataSource">Data source</param>
        /// <param name="deserialization">Deserialization function</param>
        /// <param name="abstractConcreteMap">An object containing the mapping of abstract types and interfaces to their concrete implementations</param>
        /// <returns>Deserialized object</returns>
        /// <exception cref="EnumerableNotSupportedException">Thrown when object contains an enumerable not supported by interceptor</exception>
        /// <exception cref="TypeNotAttributeException">Thrown when any of the evaluated attribute interceptors have as input param a type that is not an attribute</exception>
        /// <exception cref="TypeNotConcreteException">Thrown when type of the object to be deserialized contains an abstract type or an interface(enumerables not counted) which are not included in abstract-concrete map</exception>
        public static Task<T> InterceptDeserializationAsync<T>(Stream dataSource, Func<Stream, Type, Task<object>> deserialization, AbstractConcreteMap abstractConcreteMap = null)
        {
            var objCloneType = TypeCloneFactory.CloneType<T>(Operation.Deserialization);
            var objClone = deserialization.Invoke(dataSource, objCloneType).ConfigureAwait(false).GetAwaiter().GetResult();
            var obj = CloneToObject<T>(objClone, abstractConcreteMap);
            return Task.FromResult(obj);
        }

        /// <summary>
        /// Intercepts deserialization asyncronously
        /// </summary>
        /// <param name="dataSource">Data source</param>
        /// <param name="objType">Type of the object to deserialize</param>
        /// <param name="deserialization">Deserialization function</param>
        /// <param name="abstractConcreteMap">An object containing the mapping of abstract types and interfaces to their concrete implementations</param>
        /// <returns>Deserialized object</returns>
        /// <exception cref="EnumerableNotSupportedException">Thrown when object contains an enumerable not supported by interceptor</exception>
        /// <exception cref="TypeNotAttributeException">Thrown when any of the evaluated attribute interceptors have as input param a type that is not an attribute</exception>
        /// <exception cref="TypeNotConcreteException">Thrown when type of the object to be deserialized contains an abstract type or an interface(enumerables not counted) which are not included in abstract-concrete map</exception>
        public static Task<object> InterceptDeserializationAsync(Stream dataSource, Type objType, Func<Stream, Type, Task<object>> deserialization, AbstractConcreteMap abstractConcreteMap = null)
        {
            var objCloneType = TypeCloneFactory.CloneType(Operation.Deserialization, objType);
            var objClone = deserialization.Invoke(dataSource, objCloneType).ConfigureAwait(false).GetAwaiter().GetResult();
            var obj = CloneToObject(objClone, objType, abstractConcreteMap);
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
            objClone = PureAutoMapper.Map(obj, objCloneType);
        }

        private static T CloneToObject<T>(object objClone, AbstractConcreteMap abstractConcreteMap = null)
        {
            var objType = typeof(T);
            return (T)CloneToObject(objClone, objType, abstractConcreteMap);
        }

        private static object CloneToObject(object objClone, Type objType, AbstractConcreteMap abstractConcreteMap = null)
            => PureAutoMapper.Map(objClone, objType, abstractConcreteMap);
        #endregion
    }
}
