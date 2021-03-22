using SerializationInterceptor.Attributes;
using SerializationInterceptor.Extensions;
using SerializationInterceptor.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SerializationInterceptor
{
    /// <summary>
    /// This class is used strictly for testing purpose. It helps to visualize the type definition, and thus makes debugging easier.
    /// Don't bother wasting time on it.
    /// </summary>
    public static class TypeDefinitionSerializer
    {
        public static string Serialize<T>() => Serialize(typeof(T));

        public static string Serialize(Type type)
        {
            var encounteredTypes = new Dictionary<Type, string>();
            ProcessType(type, encounteredTypes);
            var serializedTypes = new StringBuilder();
            foreach (var serializedType in encounteredTypes.Values)
            {
                serializedTypes.AppendLine(serializedType);
            }
            return serializedTypes.ToString();
        }

        private static void ProcessType(Type type, IDictionary<Type, string> encounteredTypes)
        {
            if (encounteredTypes.ContainsKey(type)) return;
            encounteredTypes[type] = null;
            var typeDefinition = GetTypeDefinition(type, encounteredTypes);
            encounteredTypes[type] = typeDefinition;
            if (type.IsGenericType) ProcessGenericArgs(type, encounteredTypes);
            if (type.IsArray) ProcessElementType(type, encounteredTypes);
        }

        private static void ProcessGenericArgs(Type type, IDictionary<Type, string> encounteredTypes)
        {
            type.GetGenericArguments().ToList().ForEach(x => ProcessType(x, encounteredTypes));
        }

        private static void ProcessElementType(Type type, IDictionary<Type, string> encounteredTypes)
        {
            ProcessType(type.GetElementType(), encounteredTypes);
        }

        private static string GetTypeDefinition(Type type, IDictionary<Type, string> encounteredTypes)
        {
            var typeDefinition = new StringBuilder();
            ProcessTypeAttributes(type, typeDefinition);
            typeDefinition.AppendLine($"{GetTypeType(type)} {GetTypeName(type)}");
            typeDefinition.AppendLine("{");
            if (type.IsEnum) ProcessEnum(type, typeDefinition);
            else ProcessProps(type, encounteredTypes, typeDefinition);
            typeDefinition.AppendLine("}");
            return typeDefinition.ToString();
        }

        private static void ProcessProps(Type type, IDictionary<Type, string> encounteredTypes, StringBuilder typeDefinition)
        {
            foreach (var prop in type.GetProperties())
            {
                ProcessType(prop.PropertyType, encounteredTypes);
                ProcessPropAttributes(prop, typeDefinition);
                typeDefinition.AppendLine($"\t{GetTypeName(prop.PropertyType)} {prop.Name}");
            }
        }

        private static void ProcessEnum(Type type, StringBuilder typeDefinition)
        {
            Utils.GetGenericMethodDefinition(typeof(TypeDefinitionSerializer), nameof(ProcessEnumGeneric), Utils.PrivateStatic, _ => true)
                .MakeGenericMethod(type).Invoke(null, new[] { typeDefinition });
        }

        private static void ProcessEnumGeneric<T>(StringBuilder typeDefinition) where T : Enum
        {
            Enum.GetValues(typeof(T)).OfType<T>().ToList()
                .ForEach(x => typeDefinition.AppendLine($"\t{Enum.GetName(typeof(T), x)} = {Convert.ChangeType(x, Enum.GetUnderlyingType(typeof(T)))}"));
        }

        private static void ProcessTypeAttributes(Type type, StringBuilder typeDefinition)
        {
            Utils.GetAttributeBuilderParams(type, x => true).ToList().ForEach(x => { typeDefinition.AppendLine(BuildAttributeAsString(x)); });
        }

        private static void ProcessPropAttributes(PropertyInfo prop, StringBuilder typeDefinition)
        {
            Utils.GetAttributeBuilderParams(prop, x => true).ToList().ForEach(x => { typeDefinition.AppendLine($"\t{BuildAttributeAsString(x)}"); });
        }

        private static string BuildAttributeAsString(AttributeBuilderParams attributeBuilderParams)
        {
            var attributeName = attributeBuilderParams.Constructor.DeclaringType.Name.TrimAttributeSuffix();
            var constructorArgs = string.Join(", ", attributeBuilderParams.ConstructorArgs.Select((x, i) => AttributeParamToString(x)));
            var namedProps = string.Join(", ", attributeBuilderParams.NamedProps.Select((p, i) => $"{p.NamedProp.Name} = {AttributeParamToString(p.PropValue)}"));
            var namedFields = string.Join(", ", attributeBuilderParams.NamedFields.Select((f, i) => $"{f.NamedField.Name} = {AttributeParamToString(f.FieldValue)}"));
            var attributeParams = new[] { constructorArgs, namedProps, namedFields }.Where(x => !string.IsNullOrEmpty(x));
            return $"[{attributeName}{(attributeParams.Any() ? $"({string.Join(",", attributeParams)})" : "")}]";
        }

        private static string AttributeParamToString(object attributeParam)
        {
            if (attributeParam == null) return "null";
            if (attributeParam is Enum e) return $"{e.GetType().Name}.{Enum.GetName(e.GetType(), e)}";
            if (attributeParam is Type t) return $"typeof({t.Name})";
            if (attributeParam is string s) return s.DoubleQuote();
            if (attributeParam is char c) return c.Quote();
            if (attributeParam is bool b) return b.ToString().LowerFirstLetter();
            if (attributeParam.GetType().IsNumeric()) return attributeParam.ToString();
            if (attributeParam.GetType().IsArray)
            {
                var elements = new List<object>();
                foreach (var item in (IEnumerable)attributeParam) { elements.Add(item); }
                return $"new {attributeParam.GetType().GetElementType().Name}[]{{{string.Join(", ", elements.Select(x => AttributeParamToString(x)))}}}";
            }
            return attributeParam.ToString();
        }

        private static string GetTypeName(Type type)
        {
            if (type.IsGenericType) return $"{type.Name.Split('`')[0]}<{string.Join(", ", type.GetGenericArguments().Select(x => GetTypeName(x)))}>";
            if (type.IsArray) return $"{GetTypeName(type.GetElementType())}[]";
            return type.Name;
        }

        private static string GetTypeType(Type type)
            => type.IsValueType ? (type.IsEnum ? "enum" : "struct") : "class";

        private static string TrimAttributeSuffix(this string attributeTypeName)
            => attributeTypeName[0..^9];
    }
}
