using SerializationInterceptor.Exceptions;
using System;

namespace SerializationInterceptor.Attributes
{
    /// <summary>
    /// Inherit from this abstract class in order to create an attribute interceptor
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = true)]
    public abstract class InterceptorAttribute : Attribute
    {
        internal protected string InterceptorId { get; }
        internal Type TypeOfAttributeToIntercept { get; }

        /// <summary>
        /// Creates an attribute interceptor
        /// </summary>
        /// <param name="interceptorId">The id by which the interceptor can be identified</param>
        /// <param name="typeOfAttributeToIntercept">Type of the attribute to intercept</param>
        /// <exception cref="TypeNotAttributeException">Thrown if input type is not an attribute</exception>
        public InterceptorAttribute(string interceptorId, Type typeOfAttributeToIntercept)
        {
            if (!typeOfAttributeToIntercept.IsSubclassOf(typeof(Attribute))) throw new TypeNotAttributeException(typeOfAttributeToIntercept);
            InterceptorId = interceptorId;
            TypeOfAttributeToIntercept = typeOfAttributeToIntercept;
        }

        /// <summary>
        /// Implement this method for being able to alter the params for building intercepted attribute anew.
        /// Return same params if alteration not needed. Hint: use InterceptorId property inside this method to identify the currently running interceptor.
        /// </summary>
        /// <param name="originalAttributeBuilderParams">Original params used for building the intercepted attribute</param>
        /// <param name="context">Serialization/deserialization context</param>
        /// <returns>Altered params used for building the intercepted attribute anew</returns>
        internal protected abstract AttributeBuilderParams Intercept(AttributeBuilderParams originalAttributeBuilderParams, object context);
    }
}
