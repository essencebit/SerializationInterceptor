using SerializationInterceptor.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace SerializationInterceptor.Utilities
{
    internal static class Utils
    {
        public static BindingFlags PublicInstance { get; } = BindingFlags.Instance | BindingFlags.Public;
        public static BindingFlags PrivateStatic { get; } = BindingFlags.NonPublic | BindingFlags.Static;

        public static MethodInfo GetGenericMethodDefinition(
            Type declaringType, string methodName, BindingFlags bindingFlags, Func<MethodInfo, bool> filter)
                => declaringType.GetMethods(bindingFlags)
                    .Where(x => x.Name == methodName && x.IsGenericMethodDefinition)
                    .Where(filter)
                    .First();

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
        {
            if (param.Value is ReadOnlyCollection<CustomAttributeTypedArgument> collection)
                return InvokeGenericMethod(nameof(ToArray), param.ArgumentType.GetElementType(), collection.Select(x => ProcessAttributeParam(x)).ToArray());

            if (param.ArgumentType.IsEnum)
                return InvokeGenericMethod(nameof(ToType), param.ArgumentType, param.Value);

            return param.Value;
        }

        private static object InvokeGenericMethod(string methodName, Type type, object param)
            => GetGenericMethodDefinition(typeof(Utils), methodName, PrivateStatic, x => true)
                .MakeGenericMethod(type)
                .Invoke(null, new[] { param });

        private static T ToType<T>(object obj) => (T)obj;

        private static T[] ToArray<T>(params object[] elements)
        {
            var array = new T[elements.Length];
            for (int i = 0; i < elements.Length; i++)
            {
                array[i] = ToType<T>(elements[i]);
            }

            return array;
        }
    }
}
