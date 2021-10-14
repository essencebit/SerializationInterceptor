using SerializationInterceptor.Exceptions;
using System;

namespace SerializationInterceptor.Attributes
{
	/// <summary>
	/// Inherit from this abstract class in order to create an interceptor attribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = true)]
	public abstract class InterceptorAttribute : Attribute
	{
		internal protected string InterceptorId { get; }
		internal Type InterceptedAttributeType { get; }

		/// <param name="interceptorId">The id by which the interceptor attribute can be identified</param>
		/// <param name="interceptedAttributeType">The type of the intercepted attribute</param>
		/// /// <exception cref="ArgumentNullException">Thrown if null passed for <paramref name="interceptedAttributeType"/></exception>
		/// <exception cref="TypeNotAttributeException">Thrown if type passed for <paramref name="interceptedAttributeType"/> is not an attribute</exception>
		public InterceptorAttribute(string interceptorId, Type interceptedAttributeType)
		{
			if (interceptedAttributeType == null) throw new ArgumentNullException(nameof(interceptedAttributeType));
			if (!interceptedAttributeType.IsSubclassOf(typeof(Attribute))) throw new TypeNotAttributeException(interceptedAttributeType);
			InterceptorId = interceptorId;
			InterceptedAttributeType = interceptedAttributeType;
		}

		/// <summary>
		/// Implement this method for being able to alter the values of params used for instantiating the intercepted attribute.
		/// Hint: use InterceptorId property inside this method to identify the currently running interceptor attribute instance.
		/// </summary>
		/// <param name="attributeParams">Original params used for instantiating the intercepted attribute</param>
		/// <param name="context">Serialization/deserialization context</param>
		internal protected abstract void Intercept(in AttributeParams attributeParams, object context);
	}
}
