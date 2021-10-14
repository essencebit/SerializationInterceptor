using SerializationInterceptor.Attributes;
using System;

namespace SerializationInterceptor.Tests.Attributes
{
	public class DummyInterceptorAttribute : InterceptorAttribute
	{
		public DummyInterceptorAttribute()
			: base(null, typeof(DummyAttribute))
		{
		}

		protected override void Intercept(in AttributeParams attributeBuilderParams, object context)
		{
		}
	}

	public class DummyAttribute : Attribute
	{
		public DummyAttribute(Dummy[] array, Dummy @enum, string s)
		{
		}
	}

	public enum Dummy : short
	{
		V1 = -2,
		V2,
	}
}
