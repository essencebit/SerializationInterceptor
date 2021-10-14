using SerializationInterceptor.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SerializationInterceptor.Utilities
{
    internal static class Utils
    {
        public static BindingFlags PublicInstance { get; } = BindingFlags.Public | BindingFlags.Instance;
        public static BindingFlags PublicStatic { get; } = BindingFlags.Public | BindingFlags.Static;
        public static BindingFlags PrivateStatic { get; } = BindingFlags.NonPublic | BindingFlags.Static;

        public static bool IsEnumerable(this Type type) => type == typeof(IEnumerable) || type.GetInterfaces().Any(x => x == typeof(IEnumerable));
        public static bool IsGenericEnumerable(this Type type) => (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) || type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        public static bool IsGenericCollection(this Type type) => (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>)) || type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));
        public static bool IsGenericDictionary(this Type type) => (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>)) || type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>));

        public static bool IsEmptyEnumerable(this object obj)
        {
            if (obj == null) return false;
            var type = obj.GetType();
            if (!type.IsGenericEnumerable()) return false;
            var itemType = type.GetInterfaces().First(x => x.GetGenericTypeDefinition() == typeof(IEnumerable<>)).GetGenericArguments();
            return type == InvokeGenericMethod(typeof(Enumerable), nameof(Enumerable.Empty), PublicStatic, itemType).GetType();
        }

        public static string GetTypePrettyName(this Type type)
            => type.IsGenericType
                ? type.GetGenericTypePrettyName()
                : type.IsArray
                    ? type.GetArrayTypePrettyName()
                    : type.Name;

        public static MethodInfo GetGenericMethodDefinition(
            Type declaringType, string methodName, BindingFlags bindingFlags, Func<MethodInfo, bool> filter)
                => declaringType.GetMethods(bindingFlags)
                    .Where(x => x.Name == methodName && x.IsGenericMethodDefinition)
                    .Where(filter)
                    .First();

        public static object InvokeGenericMethod(Type declaringType, string methodName, BindingFlags bindingFlags, Type[] genericArgs, params object[] methodParams)
            => GetGenericMethodDefinition(declaringType, methodName, bindingFlags, x => true)
                .MakeGenericMethod(genericArgs)
                .Invoke(null, methodParams);

        public static IEnumerable<AttributeBuilderParams> GetAttributeBuilderParams(Type type, Func<CustomAttributeData, bool> attributeFilter)
            => GetAttributeBuilderParams(CustomAttributeData.GetCustomAttributes(type).Where(attributeFilter));

        public static IEnumerable<AttributeBuilderParams> GetAttributeBuilderParams(MemberInfo member, Func<CustomAttributeData, bool> attributeFilter)
            => GetAttributeBuilderParams(CustomAttributeData.GetCustomAttributes(member).Where(attributeFilter));

        private static IEnumerable<AttributeBuilderParams> GetAttributeBuilderParams(IEnumerable<CustomAttributeData> attributesData)
            => attributesData
                .Select(x => new AttributeBuilderParams
                {
                    Constructor = x.Constructor,
                    ConstructorArgs = x.ConstructorArguments.Select(x => ProcessAttributeParam(x)).ToArray(),
                    NamedProps = x.NamedArguments.Where(x => !x.IsField).Select(x => new AttributeBuilderNamedPropParam { NamedProp = (PropertyInfo)x.MemberInfo, PropValue = ProcessAttributeParam(x.TypedValue) }),
                    NamedFields = x.NamedArguments.Where(x => x.IsField).Select(x => new AttributeBuilderNamedFieldParam { NamedField = (FieldInfo)x.MemberInfo, FieldValue = ProcessAttributeParam(x.TypedValue) }),
                });

        private static object ProcessAttributeParam(CustomAttributeTypedArgument param)
            => param.Value is ReadOnlyCollection<CustomAttributeTypedArgument> collection
                ? InvokeGenericMethod(typeof(Utils), nameof(ToArray), PrivateStatic, new[] { param.ArgumentType.GetElementType() }, new[] { collection.Select(x => ProcessAttributeParam(x)).ToArray() })
                : param.ArgumentType.IsEnum
                    ? InvokeGenericMethod(typeof(Utils), nameof(Cast), PrivateStatic, new[] { param.ArgumentType }, param.Value)
                    : param.Value;

        private static T Cast<T>(object obj) => (T)obj;

        private static T[] ToArray<T>(params object[] elements)
        {
            var array = new T[elements.Length];
            for (int i = 0; i < elements.Length; i++)
            {
                array[i] = Cast<T>(elements[i]);
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
            return type.GetElementType().IsArray
                ? GetArrayTypePrettyName(type.GetElementType(), currentArrayTypeSpecifier)
                : type.GetElementType().GetTypePrettyName() + currentArrayTypeSpecifier.ToString();
        }
    }
}
