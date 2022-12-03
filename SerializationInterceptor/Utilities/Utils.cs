using SerializationInterceptor.Attributes;
using SerializationInterceptor.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace SerializationInterceptor.Utilities
{
	internal static class Utils
	{
		public const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;
		public const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;
		public const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;

		public static bool IsEmptyEnumerable(this object obj)
		{
			if (obj == null) return false;
			var type = obj.GetType();
			if (!type.IsGenericEnumerable()) return false;
			var itemType = type.GetInterfaces().First(x => x.GetGenericTypeDefinition() == typeof(IEnumerable<>)).GetGenericArguments();
			return type == InvokeGenericMethod(typeof(Enumerable), nameof(Enumerable.Empty), PublicStatic, _ => true, itemType, null).GetType();
		}

		public static object InvokeGenericMethod(Type declaringType, string methodName, BindingFlags bindingFlags, Func<MethodInfo, bool> filter, Type[] genericArgs, object obj, params object[] methodArgs)
			=> GetGenericMethodDefinition(declaringType, methodName, bindingFlags, filter)
				.MakeGenericMethod(genericArgs)
				.Invoke(obj, methodArgs);

		public static AttributeParams ToAttributeParams(this CustomAttributeData attributeData)
			=> new AttributeParams(
				attributeData.Constructor,
				attributeData.ConstructorArguments?.Select(x => new ConstructorArg(ProcessAttributeArg(x))).ToReadOnlyCollection(),
				attributeData.NamedArguments?.Where(x => !x.IsField).Select(x => new NamedProp((PropertyInfo)x.MemberInfo, ProcessAttributeArg(x.TypedValue))).ToReadOnlyCollection(),
				attributeData.NamedArguments?.Where(x => x.IsField).Select(x => new NamedField((FieldInfo)x.MemberInfo, ProcessAttributeArg(x.TypedValue))).ToReadOnlyCollection());

		public static CustomAttributeBuilder CreateAttributeBuilder(AttributeParams attributeParams)
			=> new CustomAttributeBuilder(
				attributeParams.Constructor, attributeParams.ConstructorArgs?.Select(x => x.ArgValue).ToArray(),
				attributeParams.NamedProps?.Select(x => x.Prop).ToArray(), attributeParams.NamedProps?.Select(x => x.PropValue).ToArray(),
				attributeParams.NamedFields?.Select(x => x.Field).ToArray(), attributeParams.NamedFields?.Select(x => x.FieldValue).ToArray());

		public static CustomAttributeBuilder CreateAttributeBuilder(CustomAttributeData attributeData)
			=> new CustomAttributeBuilder(
				attributeData.Constructor, attributeData.ConstructorArguments?.Select(x => x.Value).ToArray(),
				attributeData.NamedArguments?.Where(x => !x.IsField).Select(x => (PropertyInfo)x.MemberInfo).ToArray(), attributeData.NamedArguments?.Where(x => !x.IsField).Select(x => (object)x.TypedValue).ToArray(),
				attributeData.NamedArguments?.Where(x => x.IsField).Select(x => (FieldInfo)x.MemberInfo).ToArray(), attributeData.NamedArguments?.Where(x => x.IsField).Select(x => (object)x.TypedValue).ToArray());

		public static string GetTypePrettyName(this Type type)
			=> type.IsGenericType
				? type.GetGenericTypePrettyName()
				: type.IsArray
					? type.GetArrayTypePrettyName()
					: type.Name;

		private static object ProcessAttributeArg(CustomAttributeTypedArgument arg)
			=> arg.Value is ReadOnlyCollection<CustomAttributeTypedArgument> collection
				? InvokeGenericMethod(typeof(Utils), nameof(ToArray), PublicStatic, _ => true, new[] { arg.ArgumentType.GetElementType() }, null, new[] { collection.Select(x => ProcessAttributeArg(x)).ToArray() })
				: arg.ArgumentType.IsEnum
					? InvokeGenericMethod(typeof(Utils), nameof(Cast), PublicStatic, _ => true, new[] { arg.ArgumentType }, null, arg.Value)
					: arg.Value;

		private static MethodInfo GetGenericMethodDefinition(Type declaringType, string methodName, BindingFlags bindingFlags, Func<MethodInfo, bool> filter)
			=> declaringType
				.GetMethods(bindingFlags)
				.Where(x => x.Name == methodName && x.IsGenericMethodDefinition)
				.Where(filter)
				.First();

		public static T Cast<T>(object obj) => (T)obj;

		public static T[] ToArray<T>(object[] elements)
		{
			if (elements == null) return null;
			var length = elements.Length;
			var array = new T[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = (T)elements[i];
			}
			return array;
		}

		private static string GetGenericTypePrettyName(this Type type)
			=> $"{type.Name.Split('`')[0]}<{string.Join(", ", type.GetGenericArguments().Select(x => GetTypePrettyName(x)))}>";

		private static string GetArrayTypePrettyName(this Type type)
			=> type.GetArrayTypePrettyName(currentArrayTypeSpecifier: new StringBuilder(5));

		private static string GetArrayTypePrettyName(this Type type, StringBuilder currentArrayTypeSpecifier)
		{
			currentArrayTypeSpecifier.Append($"[{new string(',', type.GetArrayRank() - 1)}]");
			var elementType = type.GetElementType();
			return elementType.IsArray
				? GetArrayTypePrettyName(elementType, currentArrayTypeSpecifier)
				: elementType.GetTypePrettyName() + currentArrayTypeSpecifier.ToString();
		}
	}
}
