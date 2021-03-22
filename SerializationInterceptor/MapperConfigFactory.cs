using AutoMapper;
using SerializationInterceptor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SerializationInterceptor
{
    internal static class MapperConfigFactory
    {
        public static MapperConfiguration CreateMapperConfig(Type src, Type dest)
        {
            var encounteredMappings = new Dictionary<Type, HashSet<Type>>();
            return new MapperConfiguration(config => MapTypes(config, src, dest, encounteredMappings));
        }

        private static void MapTypes(IMapperConfigurationExpression config, Type src, Type dest, IDictionary<Type, HashSet<Type>> encounteredMappings)
        {
            if (src == dest) return;
            if (MappingExists(src, dest, encounteredMappings)) return;
            AddMapping(src, dest, encounteredMappings);
            config.CreateMap(src, dest);
            if (src.IsGenericType) MapGenericTypes(config, src, dest, encounteredMappings);
            else if (src.IsArray) MapArrayTypes(config, src, dest, encounteredMappings);
            else MapProperties(config, src, dest, encounteredMappings);
        }

        private static void AddMapping(Type src, Type dest, IDictionary<Type, HashSet<Type>> encounteredMappings)
        {
            if (encounteredMappings.ContainsKey(src)) encounteredMappings[src].Add(dest);
            else encounteredMappings[src] = new HashSet<Type> { dest };
        }

        private static bool MappingExists(Type src, Type dest, IDictionary<Type, HashSet<Type>> encounteredMappings)
            => encounteredMappings.ContainsKey(src) && encounteredMappings[src].Contains(dest);

        private static void MapGenericTypes(IMapperConfigurationExpression config, Type src, Type dest, IDictionary<Type, HashSet<Type>> encounteredMappings)
        {
            MapGenericArgs(config, src, dest, encounteredMappings);
            MapProperties(config, src, dest, encounteredMappings);
        }

        public static void MapArrayTypes(IMapperConfigurationExpression config, Type src, Type dest, IDictionary<Type, HashSet<Type>> encounteredMappings)
        {
            MapTypes(config, src.GetElementType(), dest.GetElementType(), encounteredMappings);
        }

        private static void MapGenericArgs(IMapperConfigurationExpression config, Type src, Type dest, IDictionary<Type, HashSet<Type>> encounteredMappings)
        {
            var srcArgs = src.GetGenericArguments();
            var destArgs = dest.GetGenericArguments();
            for (int i = 0; i < srcArgs.Length; i++)
            {
                MapTypes(config, srcArgs[i], destArgs[i], encounteredMappings);
            }
        }

        private static void MapProperties(IMapperConfigurationExpression config, Type src, Type dest, IDictionary<Type, HashSet<Type>> encounteredMappings)
        {
            var srcProps = src.GetProperties(Utils.PublicInstance);
            var destProps = dest.GetProperties(Utils.PublicInstance);
            foreach (var srcProp in srcProps)
            {
                var destProp = destProps.First(x => x.Name == srcProp.Name);
                MapTypes(config, srcProp.PropertyType, destProp.PropertyType, encounteredMappings);
            }
        }
    }
}
