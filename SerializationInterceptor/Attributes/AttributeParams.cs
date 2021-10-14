using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace SerializationInterceptor.Attributes
{
	/// <summary>
	/// An instance of this class encapsulates all necessary params for instantiating an attribute
	/// </summary>
	public sealed class AttributeParams
	{
		internal AttributeParams(
			ConstructorInfo constructor,
			ReadOnlyCollection<ConstructorArg> constructorArgs,
			ReadOnlyCollection<NamedProp> namedProps,
			ReadOnlyCollection<NamedField> namedFields)
		{
			Constructor = constructor;
			ConstructorArgs = constructorArgs;
			NamedProps = namedProps;
			NamedFields = namedFields;
		}

		public ConstructorInfo Constructor { get; }
		public ReadOnlyCollection<ConstructorArg> ConstructorArgs { get; }
		public ReadOnlyCollection<NamedProp> NamedProps { get; }
		public ReadOnlyCollection<NamedField> NamedFields { get; }
	}

	public sealed class ConstructorArg
	{
		public ConstructorArg(object argValue)
		{
			ArgValue = argValue;
		}

		public object ArgValue { get; set; }
	}

	public sealed class NamedProp
	{
		public NamedProp(PropertyInfo prop, object propValue)
		{
			Prop = prop ?? throw new ArgumentNullException(nameof(prop));
			PropValue = propValue;
		}

		public PropertyInfo Prop { get; }
		public object PropValue { get; set; }
	}

	public sealed class NamedField
	{
		public NamedField(FieldInfo field, object fieldValue)
		{
			Field = field ?? throw new ArgumentNullException(nameof(field));
			FieldValue = fieldValue;
		}

		public FieldInfo Field { get; }
		public object FieldValue { get; set; }
	}
}
