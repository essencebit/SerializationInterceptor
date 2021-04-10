using SerializationInterceptor.Exceptions;
using SerializationInterceptor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SerializationInterceptor
{
    internal static class PureAutoMapper
    {
        public static object Map(object src, Type destType, AbstractConcreteMap abstractConcreteMap = null)
        {
            var srcType = src.GetType();
            var referenceMap = new Dictionary<object, object>();
            return Map(src, srcType, destType, referenceMap, abstractConcreteMap ?? new AbstractConcreteMap());
        }

        private static object Map(object src, Type srcType, Type destType, IDictionary<object, object> referenceMap, AbstractConcreteMap abstractConcreteMap)
        {
            if (src == null) return null;
            if (srcType == destType) return src;
            if (Mapped(src, referenceMap)) return GetDest(src, referenceMap);
            if (srcType.IsEnumerable()) return MapEnumerable(src, srcType, destType, referenceMap, abstractConcreteMap);
            return MapSimpleObject(src, srcType, destType, referenceMap, abstractConcreteMap);
        }

        private static object MapEnumerable(object srcEnumerable, Type srcType, Type destType, IDictionary<object, object> referenceMap, AbstractConcreteMap abstractConcreteMap)
        {
            if (srcEnumerable.GetType().IsArray) return MapArray((Array)srcEnumerable, srcType, destType, referenceMap, abstractConcreteMap);
            if (srcEnumerable.GetType().IsGenericDictionary()) return MapGenericDictionary(srcEnumerable, srcType, destType, referenceMap, abstractConcreteMap);
            if (srcEnumerable.GetType().IsGenericCollection()) return MapGenericCollection(srcEnumerable, srcType, destType, referenceMap, abstractConcreteMap);
            if (srcEnumerable.IsEmptyEnumerable()) return MapEmptyEnumerable(srcEnumerable, srcType, destType, referenceMap);
            throw new EnumerableNotSupportedException($"Enumerable of type {srcEnumerable.GetType().GetTypePrettyName()} not supported. Allowed only arrays and types from System.Collections.Generic that implement {typeof(ICollection<>).GetTypePrettyName()} interface");
        }

        private static object MapSimpleObject(object src, Type srcType, Type destType, IDictionary<object, object> referenceMap, AbstractConcreteMap abstractConcreteMap)
        {
            if ((destType.IsAbstract || destType.IsInterface) && !abstractConcreteMap.ContainsKey(destType)) throw new TypeNotConcreteException(destType);
            var dest = Activator.CreateInstance(abstractConcreteMap.ContainsKey(destType) ? abstractConcreteMap[destType] : destType);
            MapReferences(src, dest, referenceMap);
            MapProps(src, srcType, dest, destType, referenceMap, abstractConcreteMap);
            return dest;
        }

        private static object MapArray(Array srcArray, Type srcType, Type destType, IDictionary<object, object> referenceMap, AbstractConcreteMap abstractConcreteMap)
        {
            var srcElementType = srcType.IsArray ? srcType.GetElementType() : srcType.GetGenericArguments()[0];
            var destElementType = destType.IsArray ? destType.GetElementType() : destType.GetGenericArguments()[0];
            var rank = srcArray.GetType().GetArrayRank();
            var lengthForRank = new int[rank];
            for (int i = 0; i < rank; i++) lengthForRank[i] = srcArray.GetLength(i);
            var destArray = Array.CreateInstance(destElementType, lengthForRank);
            MapReferences(srcArray, destArray, referenceMap);
            var currentPosition = new int[rank];
            var currentRank = 0;
            Utils.InvokeGenericMethod(typeof(PureAutoMapper), nameof(MapArrayElements), Utils.PrivateStatic, new[] { srcElementType, destElementType }, srcArray, destArray, referenceMap, lengthForRank, currentPosition, currentRank, abstractConcreteMap);
            return destArray;
        }

        private static void MapArrayElements<TSrcElement, TDestElement>(Array srcArray, Array destArray, IDictionary<object, object> referenceMap, int[] lengthForRank, int[] currentPosition, int currentRank, AbstractConcreteMap abstractConcreteMap)
        {
            var maxRank = lengthForRank.Length - 1;
            for (int currentPositionForCurrentRank = 0; currentPositionForCurrentRank < lengthForRank[currentRank]; currentPositionForCurrentRank++)
            {
                currentPosition[currentRank] = currentPositionForCurrentRank;
                if (currentRank < maxRank)
                {
                    for (int rank = currentRank + 1; rank <= maxRank; rank++) currentPosition[rank] = 0;
                    MapArrayElements<TSrcElement, TDestElement>(srcArray, destArray, referenceMap, lengthForRank, currentPosition, currentRank + 1, abstractConcreteMap);
                }
                else
                {
                    var srcElement = (TSrcElement)srcArray.GetValue(currentPosition);
                    var destElement = (TDestElement)Map(srcElement, typeof(TSrcElement), typeof(TDestElement), referenceMap, abstractConcreteMap);
                    destArray.SetValue(destElement, currentPosition);
                }
            }
        }

        private static object MapGenericDictionary(object srcDictionary, Type srcType, Type destType, IDictionary<object, object> referenceMap, AbstractConcreteMap abstractConcreteMap)
        {
            var srcGenericArgs = srcType.GetGenericArguments();
            if (srcGenericArgs.Length == 1) srcGenericArgs = srcGenericArgs[0].GetGenericArguments();
            var destGenericArgs = destType.GetGenericArguments();
            if (destGenericArgs.Length == 1) destGenericArgs = destGenericArgs[0].GetGenericArguments();
            var destDictionaryType = srcDictionary.GetType().GetGenericTypeDefinition().MakeGenericType(destGenericArgs);
            var destDictionary = Activator.CreateInstance(destDictionaryType);
            MapReferences(srcDictionary, destDictionary, referenceMap);
            Utils.InvokeGenericMethod(typeof(PureAutoMapper), nameof(MapGenericDictionaryItems), Utils.PrivateStatic, srcGenericArgs.Concat(destGenericArgs).ToArray(), srcDictionary, destDictionary, referenceMap, abstractConcreteMap);
            return destDictionary;
        }

        private static void MapGenericDictionaryItems<TSrcKey, TSrcValue, TDestKey, TDestValue>(IDictionary<TSrcKey, TSrcValue> srcDictionary, IDictionary<TDestKey, TDestValue> destDictionary, IDictionary<object, object> referenceMap, AbstractConcreteMap abstractConcreteMap)
        {
            foreach (var srcItem in srcDictionary)
            {
                var destItemKey = (TDestKey)Map(srcItem.Key, typeof(TSrcKey), typeof(TDestKey), referenceMap, abstractConcreteMap);
                var destItemValue = (TDestValue)Map(srcItem.Value, typeof(TSrcValue), typeof(TDestValue), referenceMap, abstractConcreteMap);
                destDictionary.Add(destItemKey, destItemValue);
            }
        }

        private static object MapGenericCollection(object srcCollection, Type srcType, Type destType, IDictionary<object, object> referenceMap, AbstractConcreteMap abstractConcreteMap)
        {
            var srcItemType = srcType.GetGenericArguments()[0];
            var destItemType = destType.GetGenericArguments()[0];
            var destCollectionType = srcCollection.GetType().GetGenericTypeDefinition().MakeGenericType(destItemType);
            var destCollection = Activator.CreateInstance(destCollectionType);
            MapReferences(srcCollection, destCollection, referenceMap);
            Utils.InvokeGenericMethod(typeof(PureAutoMapper), nameof(MapGenericCollectionItems), Utils.PrivateStatic, new[] { srcItemType, destItemType }, srcCollection, destCollection, referenceMap, abstractConcreteMap);
            return destCollection;
        }

        private static void MapGenericCollectionItems<TSrcItem, TDestItem>(ICollection<TSrcItem> srcCollection, ICollection<TDestItem> destCollection, IDictionary<object, object> referenceMap, AbstractConcreteMap abstractConcreteMap)
        {
            foreach (var srcItem in srcCollection)
            {
                var destItem = (TDestItem)Map(srcItem, typeof(TSrcItem), typeof(TDestItem), referenceMap, abstractConcreteMap);
                destCollection.Add(destItem);
            }
        }

        private static object MapEmptyEnumerable(object srcEmptyEnumerable, Type srcType, Type destType, IDictionary<object, object> referenceMap)
        {
            var destEmptyEnumerable = Utils.InvokeGenericMethod(typeof(Enumerable), nameof(Enumerable.Empty), Utils.PublicStatic, destType.GetGenericArguments());
            MapReferences(srcEmptyEnumerable, destEmptyEnumerable, referenceMap);
            return destEmptyEnumerable;
        }

        private static void MapProps(object src, Type srcType, object dest, Type destType, IDictionary<object, object> referenceMap, AbstractConcreteMap abstractConcreteMap)
        {
            var srcProps = GetSrcProperties(srcType);
            var destProps = GetDestProperties(destType);
            foreach (var srcProp in srcProps)
            {
                var srcPropValue = srcProp.GetValue(src);
                var destProp = destProps.FirstOrDefault(x => x.Name == srcProp.Name);
                if (destProp == null) continue;
                var destPropValue = Map(srcPropValue, srcProp.PropertyType, destProp.PropertyType, referenceMap, abstractConcreteMap);
                destProp.SetValue(dest, destPropValue);
            }
        }

        private static IEnumerable<PropertyInfo> GetSrcProperties(Type srcType) => srcType.GetProperties(Utils.PublicInstance).Where(x => x.CanRead);

        private static IEnumerable<PropertyInfo> GetDestProperties(Type destType) => destType.GetProperties(Utils.PublicInstance).Where(x => x.CanWrite);

        private static bool Mapped(object src, IDictionary<object, object> referenceMap) => referenceMap.ContainsKey(src);

        private static void MapReferences(object src, object dest, IDictionary<object, object> referenceMap) => referenceMap.Add(src, dest);

        private static object GetDest(object src, IDictionary<object, object> referenceMap) => referenceMap[src];
    }
}
