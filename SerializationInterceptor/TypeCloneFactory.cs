using SerializationInterceptor.Attributes;
using SerializationInterceptor.Enums;
using SerializationInterceptor.Exceptions;
using SerializationInterceptor.Extensions;
using SerializationInterceptor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SerializationInterceptor
{
	internal static class TypeCloneFactory
	{
		public static Type CloneType<T>(Operation operation, object context) => CloneType(operation, typeof(T), context);

		public static Type CloneType(Operation operation, Type type, object context)
		{
			var typesThatMustBeCloned = GetTypesThatMustBeCloned(operation, type);
			if (!TypeMustBeCloned(type, typesThatMustBeCloned)) return type;
			var moduleBuilder = CreateModule(type);
			var typesMap = new Dictionary<Type, Type>();
			return CloneType(operation, moduleBuilder, type, typesThatMustBeCloned, typesMap, context);
		}

		private static ISet<Type> GetTypesThatMustBeCloned(Operation operation, Type type)
		{
			var initialPath = Enumerable.Empty<Type>();
			var terminalPaths = new List<IList<Type>>();
			var typeHasInterceptorsMap = new Dictionary<Type, bool>();
			TraverseType(operation, initialPath, type, terminalPaths, typeHasInterceptorsMap);
			return GetTypesThatMustBeCloned(typeHasInterceptorsMap, terminalPaths);
		}

		private static void TraverseType(Operation operation, IEnumerable<Type> currentPath, Type type, IList<IList<Type>> terminalPaths, IDictionary<Type, bool> typeHasInterceptorsMap)
		{
			if (type.IsGenericType && !type.IsGenericTypeDefinition)
			{
				TraverseType(operation, currentPath, type.GetGenericTypeDefinition(), terminalPaths, typeHasInterceptorsMap);
				foreach (var genericArg in type.GetGenericArguments())
				{
					TraverseType(operation, currentPath, genericArg, terminalPaths, typeHasInterceptorsMap);
				}
			}
			else if (type.IsArray)
			{
				TraverseType(operation, currentPath, type.GetElementType(), terminalPaths, typeHasInterceptorsMap);
			}
			else
			{
				if (typeHasInterceptorsMap.ContainsKey(type))
				{
					terminalPaths.Add(currentPath.Concat(new List<Type> { type }).ToList());
					return;
				}
				if (type.IsEnumerable()) return;
				var props = GetProps(operation, type);
				typeHasInterceptorsMap[type] = TypeHasInterceptors(type) || PropsHaveInterceptors(props);
				if (!props.Any())
				{
					terminalPaths.Add(currentPath.Concat(new List<Type> { type }).ToList());
					return;
				}
				currentPath = currentPath.Concat(new Type[] { type });
				foreach (var prop in props)
				{
					TraverseType(operation, currentPath, prop.PropertyType, terminalPaths, typeHasInterceptorsMap);
				}
			}
		}

		private static ISet<Type> GetTypesThatMustBeCloned(IDictionary<Type, bool> typeMustBeClonedMap, IEnumerable<IList<Type>> terminalPaths)
		{
			var typesThatMustBeCloned = new HashSet<Type>();
			while (typeMustBeClonedMap.Any(x => x.Value))
			{
				var type = typeMustBeClonedMap.First(x => x.Value).Key;
				typesThatMustBeCloned.Add(type);
				typeMustBeClonedMap.Remove(type);
				foreach (var terminalPath in terminalPaths)
				{
					if (!terminalPath.Contains(type)) continue;
					var typePosition = terminalPath.IndexOf(type);
					for (int i = 0; i < typePosition; i++)
					{
						if (typeMustBeClonedMap.ContainsKey(terminalPath[i])) typeMustBeClonedMap[terminalPath[i]] = true;
					}
				}
			}
			return typesThatMustBeCloned;
		}

		private static ModuleBuilder CreateModule(Type type)
		{
			var assemblyName = BuildAssemblyName(type);
			var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
			return assemblyBuilder.DefineDynamicModule(BuildModuleName(assemblyName));
		}

		private static Type CloneType(Operation operation, ModuleBuilder moduleBuilder, Type type, ISet<Type> typesThatMustBeCloned, IDictionary<Type, Type> typesMap, object context)
		{
			if (TypeCloned(type, typesMap)) return typesMap[type];
			if (type.IsGenericType && !type.IsGenericTypeDefinition) return CloneGenericType(operation, moduleBuilder, type, typesThatMustBeCloned, typesMap, context);
			if (type.IsGenericTypeDefinition) return CloneGenericTypeDefinition(operation, moduleBuilder, type, typesThatMustBeCloned, typesMap, context);
			if (type.IsArray) return CloneArrayType(operation, moduleBuilder, type, typesThatMustBeCloned, typesMap, context);
			return CloneSimpleType(operation, moduleBuilder, type, typesThatMustBeCloned, typesMap, context);
		}

		private static Type CloneSimpleType(Operation operation, ModuleBuilder moduleBuilder, Type type, ISet<Type> typesThatMustBeCloned, IDictionary<Type, Type> typesMap, object context)
		{
			var typeBuilder = moduleBuilder.DefineType(BuildTypeName(type), TypeAttributes.Public, GetBaseType(type));
			typesMap[type] = typeBuilder;
			SetTypeAttributes(typeBuilder, type, context);
			foreach (var prop in GetProps(operation, type))
			{
				CloneProp(operation, moduleBuilder, typeBuilder, prop, typesThatMustBeCloned, typesMap, context);
			}
			var typeClone = typeBuilder.CreateType();
			typesMap[type] = typeClone;
			return typeClone;
		}

		private static Type CloneGenericType(Operation operation, ModuleBuilder moduleBuilder, Type type, ISet<Type> typesThatMustBeCloned, IDictionary<Type, Type> typesMap, object context)
		{
			var genericTypeDefinition = type.GetGenericTypeDefinition();
			Type genericTypeClone;
			if (TypeMustBeCloned(genericTypeDefinition, typesThatMustBeCloned))
			{
				var genericTypeDefinitionClone = CloneType(operation, moduleBuilder, genericTypeDefinition, typesThatMustBeCloned, typesMap, context);
				var genericTypeArgs = CloneGenericArgs(operation, moduleBuilder, type, typesThatMustBeCloned, typesMap, context);
				genericTypeClone = genericTypeDefinitionClone.MakeGenericType(genericTypeArgs.ToArray());
			}
			else
			{
				var genericTypeArgs = CloneGenericArgs(operation, moduleBuilder, type, typesThatMustBeCloned, typesMap, context);
				genericTypeClone = genericTypeDefinition.MakeGenericType(genericTypeArgs.ToArray());
			}
			typesMap[type] = genericTypeClone;
			return genericTypeClone;
		}

		private static IEnumerable<Type> CloneGenericArgs(Operation operation, ModuleBuilder moduleBuilder, Type type, ISet<Type> typesThatMustBeCloned, IDictionary<Type, Type> typesMap, object context)
			=> type.GetGenericArguments().Select(x => TypeMustBeCloned(x, typesThatMustBeCloned) ? CloneType(operation, moduleBuilder, x, typesThatMustBeCloned, typesMap, context) : x);

		private static Type CloneGenericTypeDefinition(Operation operation, ModuleBuilder moduleBuilder, Type type, ISet<Type> typesThatMustBeCloned, IDictionary<Type, Type> typesMap, object context)
		{
			var typeBuilder = moduleBuilder.DefineType(BuildGenericTypeName(type), TypeAttributes.Public, GetBaseType(type));
			typesMap[type] = typeBuilder;
			SetTypeAttributes(typeBuilder, type, context);
			typeBuilder.DefineGenericParameters(type.GetGenericArguments().Select(x => x.Name).ToArray());
			foreach (var prop in GetProps(operation, type))
			{
				CloneProp(operation, moduleBuilder, typeBuilder, prop, typesThatMustBeCloned, typesMap, context);
			}
			var genericTypeDefinitionClone = typeBuilder.CreateType();
			typesMap[type] = genericTypeDefinitionClone;
			return genericTypeDefinitionClone;
		}

		private static Type CloneArrayType(Operation operation, ModuleBuilder moduleBuilder, Type type, ISet<Type> typesThatMustBeCloned, IDictionary<Type, Type> typesMap, object context)
		{
			var elementType = CloneType(operation, moduleBuilder, type.GetElementType(), typesThatMustBeCloned, typesMap, context);
			var rank = type.GetArrayRank();
			var arrayTypeClone = rank > 1 ? elementType.MakeArrayType(rank) : elementType.MakeArrayType();
			typesMap[type] = arrayTypeClone;
			return arrayTypeClone;
		}

		private static void CloneProp(Operation operation, ModuleBuilder moduleBuilder, TypeBuilder typeBuilder, PropertyInfo prop, ISet<Type> typesThatMustBeCloned, IDictionary<Type, Type> typesMap, object context)
		{
			var propType = TypeMustBeCloned(prop.PropertyType, typesThatMustBeCloned) ? CloneType(operation, moduleBuilder, prop.PropertyType, typesThatMustBeCloned, typesMap, context) : prop.PropertyType;
			var propBuilder = typeBuilder.DefineProperty(prop.Name, PropertyAttributes.None, CallingConventions.Any, propType, null);
			var fieldBuilder = typeBuilder.DefineField(BuildFieldName(prop), propType, FieldAttributes.Private);
			SetGetterAndSetter(typeBuilder, prop, propType, propBuilder, fieldBuilder);
			SetPropAttributes(propBuilder, prop, context);
		}

		private static void SetGetterAndSetter(TypeBuilder typeBuilder, PropertyInfo prop, Type propType, PropertyBuilder propBuilder, FieldBuilder fieldBuilder)
		{
			var getterSetterAttr = MethodAttributes.Public | MethodAttributes.SpecialName;
			var getterBuilder = typeBuilder.DefineMethod(BuildGetterMethodName(prop), getterSetterAttr, propType, null);
			var getterILGenerator = getterBuilder.GetILGenerator();
			getterILGenerator.Emit(OpCodes.Ldarg_0);
			getterILGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
			getterILGenerator.Emit(OpCodes.Ret);
			var setterBuilder = typeBuilder.DefineMethod(BuildSetterMethodName(prop), getterSetterAttr, null, new Type[] { propType });
			var setterILGenerator = setterBuilder.GetILGenerator();
			setterILGenerator.Emit(OpCodes.Ldarg_0);
			setterILGenerator.Emit(OpCodes.Ldarg_1);
			setterILGenerator.Emit(OpCodes.Stfld, fieldBuilder);
			setterILGenerator.Emit(OpCodes.Ret);
			propBuilder.SetGetMethod(getterBuilder);
			propBuilder.SetSetMethod(setterBuilder);
		}

		private static void SetTypeAttributes(TypeBuilder typeBuilder, Type type, object context)
		{
			var interceptors = FilterInterceptors(type.GetCustomAttributes());
			foreach (var attributeData in CustomAttributeData.GetCustomAttributes(type).Where(x => !x.AttributeType.IsSubclassOf(typeof(InterceptorAttribute))))
			{
				typeBuilder.SetCustomAttribute(GetAttributeBuilder(attributeData, interceptors, context));
			}
		}

		private static void SetPropAttributes(PropertyBuilder propBuilder, PropertyInfo prop, object context)
		{
			var interceptors = FilterInterceptors(prop.GetCustomAttributes());
			foreach (var attributeData in CustomAttributeData.GetCustomAttributes(prop).Where(x => !x.AttributeType.IsSubclassOf(typeof(InterceptorAttribute))))
			{
				propBuilder.SetCustomAttribute(GetAttributeBuilder(attributeData, interceptors, context));
			}
		}

		private static IEnumerable<InterceptorAttribute> FilterInterceptors(IEnumerable<Attribute> attributes)
			=> attributes.Where(x => IsInterceptor(x)).Select(x => (InterceptorAttribute)x);

		private static CustomAttributeBuilder GetAttributeBuilder(CustomAttributeData attributeData, IEnumerable<InterceptorAttribute> interceptors, object context)
		{
			var interceptor = interceptors.FirstOrDefault(x => x.InterceptedAttributeType == attributeData.AttributeType);
			if (interceptor == null) return Utils.CreateAttributeBuilder(attributeData);
			var attributeParams = attributeData.ToAttributeParams();
			interceptor.Intercept(in attributeParams, context);
			return Utils.CreateAttributeBuilder(attributeParams);
		}

		private static IEnumerable<PropertyInfo> GetProps(Operation operation, Type type)
			=> operation switch
			{
				Operation.Serialization => type.GetProperties(Utils.PublicInstance).Where(x => x.CanRead),
				Operation.Deserialization => type.GetProperties(Utils.PublicInstance).Where(x => x.CanWrite),
				_ => throw new OperationNotSupportedException(operation)
			};

		private static bool TypeMustBeCloned(Type type, ISet<Type> typesThatMustBeCloned)
			=> type.IsGenericType && !type.IsGenericTypeDefinition
				? TypeMustBeCloned(type.GetGenericTypeDefinition(), typesThatMustBeCloned) || type.GetGenericArguments().Any(x => TypeMustBeCloned(x, typesThatMustBeCloned))
				: type.IsArray
					? TypeMustBeCloned(type.GetElementType(), typesThatMustBeCloned)
					: typesThatMustBeCloned.Contains(type);

		private static bool TypeCloned(Type type, IDictionary<Type, Type> typesMap) => typesMap.ContainsKey(type);

		private static bool TypeHasInterceptors(Type type) => type.GetCustomAttributes().Any(x => x.GetType().IsSubclassOf(typeof(InterceptorAttribute)));

		private static bool PropsHaveInterceptors(IEnumerable<PropertyInfo> props) => props.Any(x => PropHaveInterceptors(x));

		private static bool PropHaveInterceptors(PropertyInfo x) => x.GetCustomAttributes().Any(x => x.GetType().IsSubclassOf(typeof(InterceptorAttribute)));

		private static bool IsInterceptor(Attribute x) => x.GetType().IsSubclassOf(typeof(InterceptorAttribute));

		private static Type GetBaseType(Type type) => type.IsValueType ? (type.IsEnum ? typeof(Enum) : typeof(ValueType)) : typeof(object);

		private static AssemblyName BuildAssemblyName(Type type) => new AssemblyName($"{type.Name}_Clone_Assembly");

		private static string BuildModuleName(AssemblyName assemblyName) => $"{assemblyName.Name}_Module.dll";

		private static string BuildTypeName(Type type) => $"{type.Name}_Clone";

		private static string BuildGenericTypeName(Type type) => $"{type.Name.Split('`')[0]}_Clone`{type.GetGenericArguments().Length}";

		private static string BuildFieldName(PropertyInfo prop) => char.IsUpper(prop.Name[0]) ? $"_{prop.Name.LowerFirstLetter()}" : $"_{prop.Name}";

		private static string BuildGetterMethodName(PropertyInfo prop) => $"get_{prop.Name}";

		private static string BuildSetterMethodName(PropertyInfo prop) => $"set_{prop.Name}";
	}
}
