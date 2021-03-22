using SerializationInterceptor.Attributes;
using SerializationInterceptor.Extensions;
using SerializationInterceptor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SerializationInterceptor
{
    // TODO: make internal
    public static class SerializationTypeCloneFactory
    {
        public static Type CloneType<T>() => CloneType(typeof(T));

        public static Type CloneType(Type type)
        {
            var typeHasInterceptorsMap = new Dictionary<Type, bool>();
            FillTypeHasInterceptorsMap(type, typeHasInterceptorsMap);
            if (!TypeHasInterceptors(type, typeHasInterceptorsMap)) return type;
            var moduleBuilder = CreateModule(type);
            var typeCloneMap = new Dictionary<Type, Type>();
            return CloneType(moduleBuilder, type, typeHasInterceptorsMap, typeCloneMap);
        }

        private static void FillTypeHasInterceptorsMap(Type type, IDictionary<Type, bool> typeHasInterceptorsMap)
        {
            if (typeHasInterceptorsMap.ContainsKey(type)) return;
            bool hasInterceptors;
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                hasInterceptors = TypeHasInterceptors(type.GetGenericTypeDefinition(), typeHasInterceptorsMap)
                    || type.GetGenericArguments().Any(x => TypeHasInterceptors(x, typeHasInterceptorsMap));
            }
            else if (type.IsArray)
            {
                hasInterceptors = TypeHasInterceptors(type.GetElementType(), typeHasInterceptorsMap);
            }
            else
            {
                var props = type.GetProperties(Utils.PublicInstance);
                hasInterceptors = type.GetCustomAttributes().Any(x => x.GetType().IsSubclassOf(typeof(InterceptorAttribute)))
                    || props.Any(x =>
                        x.GetCustomAttributes().Any(x => x.GetType().IsSubclassOf(typeof(InterceptorAttribute)))
                        || TypeHasInterceptors(x.PropertyType, typeHasInterceptorsMap));
            }
            typeHasInterceptorsMap[type] = hasInterceptors;
        }

        private static ModuleBuilder CreateModule(Type type)
        {
            var assemblyName = BuildAssemblyName(type);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            return assemblyBuilder.DefineDynamicModule(BuildModuleName(assemblyName));
        }

        private static Type CloneType(ModuleBuilder moduleBuilder, Type type, IDictionary<Type, bool> typeHasInterceptorsMap, IDictionary<Type, Type> typeCloneMap)
        {
            if (TypeCloned(type, typeCloneMap)) return typeCloneMap[type];
            if (type.IsGenericTypeDefinition) return CloneGenericTypeDefinition(moduleBuilder, type, typeHasInterceptorsMap, typeCloneMap);
            if (type.IsGenericType) return CloneGenericType(moduleBuilder, type, typeHasInterceptorsMap, typeCloneMap);
            if (type.IsArray) return CloneArrayType(moduleBuilder, type, typeHasInterceptorsMap, typeCloneMap);
            return CloneSimpleType(moduleBuilder, type, typeHasInterceptorsMap, typeCloneMap);
        }

        private static Type CloneSimpleType(ModuleBuilder moduleBuilder, Type type, IDictionary<Type, bool> typeHasInterceptorsMap, IDictionary<Type, Type> typeCloneMap)
        {
            var typeBuilder = moduleBuilder.DefineType(BuildTypeName(type), TypeAttributes.Public, GetBaseType(type));
            typeCloneMap[type] = typeBuilder;
            SetTypeAttributes(typeBuilder, type);
            type.GetProperties(Utils.PublicInstance).ToList().ForEach(x => CloneProp(moduleBuilder, typeBuilder, x, typeHasInterceptorsMap, typeCloneMap));
            var typeClone = typeBuilder.CreateType();
            typeCloneMap[type] = typeClone;
            return typeClone;
        }

        private static Type CloneGenericType(ModuleBuilder moduleBuilder, Type type, IDictionary<Type, bool> typeHasInterceptorsMap, IDictionary<Type, Type> typeCloneMap)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            Type genericTypeClone;
            if (TypeHasInterceptors(genericTypeDefinition, typeHasInterceptorsMap))
            {
                var genericTypeDefinitionClone = CloneType(moduleBuilder, genericTypeDefinition, typeHasInterceptorsMap, typeCloneMap);
                var genericTypeArgs = CloneGenericArgs(moduleBuilder, type, typeHasInterceptorsMap, typeCloneMap);
                genericTypeClone = genericTypeDefinitionClone.MakeGenericType(genericTypeArgs.ToArray());
            }
            else
            {
                var genericTypeArgs = CloneGenericArgs(moduleBuilder, type, typeHasInterceptorsMap, typeCloneMap);
                genericTypeClone = genericTypeDefinition.MakeGenericType(genericTypeArgs.ToArray());
            }
            typeCloneMap[type] = genericTypeClone;
            return genericTypeClone;
        }

        private static IEnumerable<Type> CloneGenericArgs(ModuleBuilder moduleBuilder, Type type, IDictionary<Type, bool> typeHasInterceptorsMap, IDictionary<Type, Type> typeCloneMap)
        {
            return type.GetGenericArguments().Select(x => TypeHasInterceptors(x, typeHasInterceptorsMap) ? CloneType(moduleBuilder, x, typeHasInterceptorsMap, typeCloneMap) : x);
        }

        private static Type CloneGenericTypeDefinition(ModuleBuilder moduleBuilder, Type type, IDictionary<Type, bool> typeHasInterceptorsMap, IDictionary<Type, Type> typeCloneMap)
        {
            var typeBuilder = moduleBuilder.DefineType(BuildGenericTypeName(type), TypeAttributes.Public, GetBaseType(type));
            typeCloneMap[type] = typeBuilder;
            SetTypeAttributes(typeBuilder, type);
            typeBuilder.DefineGenericParameters(type.GetGenericArguments().Select(x => x.Name).ToArray());
            type.GetProperties(Utils.PublicInstance).ToList().ForEach(x => CloneProp(moduleBuilder, typeBuilder, x, typeHasInterceptorsMap, typeCloneMap));
            var genericTypeDefinitionClone = typeBuilder.CreateType();
            typeCloneMap[type] = genericTypeDefinitionClone;
            return genericTypeDefinitionClone;
        }

        private static Type CloneArrayType(ModuleBuilder moduleBuilder, Type type, IDictionary<Type, bool> typeHasInterceptorsMap, IDictionary<Type, Type> typeCloneMap)
        {
            var arrayTypeClone = CloneType(moduleBuilder, type.GetElementType(), typeHasInterceptorsMap, typeCloneMap).MakeArrayType();
            typeCloneMap[type] = arrayTypeClone;
            return arrayTypeClone;
        }

        private static void CloneProp(ModuleBuilder moduleBuilder, TypeBuilder typeBuilder, PropertyInfo prop, IDictionary<Type, bool> typeHasInterceptorsMap, IDictionary<Type, Type> typeCloneMap)
        {
            var propType = TypeHasInterceptors(prop.PropertyType, typeHasInterceptorsMap)
                ? CloneType(moduleBuilder, prop.PropertyType, typeHasInterceptorsMap, typeCloneMap)
                : prop.PropertyType;

            var fieldBuilder = typeBuilder.DefineField(BuildFieldName(prop), propType, FieldAttributes.Private);

            var propBuilder = typeBuilder.DefineProperty(prop.Name, PropertyAttributes.None, CallingConventions.Any, propType, null);

            var getterSetterAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            var getterMethodBuilder = typeBuilder.DefineMethod(BuildGetterMethodName(prop), getterSetterAttr, propType, null);
            var getILGen = getterMethodBuilder.GetILGenerator();
            getILGen.Emit(OpCodes.Ldarg_0);
            getILGen.Emit(OpCodes.Ldfld, fieldBuilder);
            getILGen.Emit(OpCodes.Ret);

            var setterMethodBuilder = typeBuilder.DefineMethod(BuildSetterMethodName(prop), getterSetterAttr, null, new Type[] { propType });
            var setILGen = setterMethodBuilder.GetILGenerator();
            setILGen.Emit(OpCodes.Ldarg_0);
            setILGen.Emit(OpCodes.Ldarg_1);
            setILGen.Emit(OpCodes.Stfld, fieldBuilder);
            setILGen.Emit(OpCodes.Ret);

            propBuilder.SetGetMethod(getterMethodBuilder);
            propBuilder.SetSetMethod(setterMethodBuilder);

            SetPropAttributes(propBuilder, prop);
        }

        private static void SetTypeAttributes(TypeBuilder typeBuilder, Type type)
        {
            var interceptors = FilterInterceptors(type.GetCustomAttributes());
            Utils.GetAttributeBuilderParams(type, x => !x.AttributeType.IsSubclassOf(typeof(InterceptorAttribute))).ToList().ForEach(x =>
            {
                x = TryInterceptAttributeBuilderParams(x, interceptors);
                typeBuilder.SetCustomAttribute(CreateAttributeBuilder(x));
            });
        }

        private static void SetPropAttributes(PropertyBuilder propBuilder, PropertyInfo prop)
        {
            var interceptors = FilterInterceptors(prop.GetCustomAttributes());
            Utils.GetAttributeBuilderParams(prop, x => !x.AttributeType.IsSubclassOf(typeof(InterceptorAttribute))).ToList().ForEach(x =>
            {
                x = TryInterceptAttributeBuilderParams(x, interceptors);
                propBuilder.SetCustomAttribute(CreateAttributeBuilder(x));
            });
        }

        private static IEnumerable<InterceptorAttribute> FilterInterceptors(IEnumerable<Attribute> attributes)
            => attributes
                .Where(x => x.GetType().IsSubclassOf(typeof(InterceptorAttribute)))
                .Select(x => (InterceptorAttribute)x)
                .ToList();

        private static AttributeBuilderParams TryInterceptAttributeBuilderParams(AttributeBuilderParams attributeBuilderParams, IEnumerable<InterceptorAttribute> interceptors)
        {
            var interceptor = interceptors.FirstOrDefault(i => i.TypeOfAttributeToIntercept == attributeBuilderParams.Constructor.DeclaringType);
            if (interceptor != null) attributeBuilderParams = interceptor.Intercept(attributeBuilderParams);
            return attributeBuilderParams;
        }

        private static CustomAttributeBuilder CreateAttributeBuilder(AttributeBuilderParams attributeBuilderParams)
            => new CustomAttributeBuilder(
                attributeBuilderParams.Constructor, attributeBuilderParams.ConstructorArgs.ToArray(),
                attributeBuilderParams.NamedProps.Select(p => p.NamedProp).ToArray(), attributeBuilderParams.NamedProps.Select(p => p.PropValue).ToArray(),
                attributeBuilderParams.NamedFields.Select(f => f.NamedField).ToArray(), attributeBuilderParams.NamedFields.Select(f => f.FieldValue).ToArray());

        private static bool TypeHasInterceptors(Type type, IDictionary<Type, bool> typeHasInterceptorsMap)
            => typeHasInterceptorsMap[type];

        private static bool TypeCloned(Type type, IDictionary<Type, Type> typeCloneMap)
            => typeCloneMap.ContainsKey(type);

        private static Type GetBaseType(Type type)
            => type.IsValueType ? (type.IsEnum ? typeof(Enum) : typeof(ValueType)) : typeof(object);

        private static AssemblyName BuildAssemblyName(Type type)
            => new AssemblyName($"{type.Name}CloneAssembly");

        private static string BuildModuleName(AssemblyName assemblyName)
            => $"{assemblyName.Name}Clone.dll";

        private static string BuildTypeName(Type type)
            => $"{type.Name}Clone";

        private static string BuildGenericTypeName(Type type)
            => $"{type.Name.Split('`')[0]}Clone`{type.GetGenericArguments().Length}";

        private static string BuildFieldName(PropertyInfo prop)
            => char.IsUpper(prop.Name[0]) ? $"_{prop.Name.LowerFirstLetter()}" : $"_{prop.Name}";

        private static string BuildGetterMethodName(PropertyInfo prop)
            => $"get_{prop.Name}";

        private static string BuildSetterMethodName(PropertyInfo prop)
            => $"set_{prop.Name}";
    }
}
