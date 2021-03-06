using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using HarmonyLib;
using ProtoRegister.Protos;
using UnityEngine;

namespace ProtoRegister.Utils {
    public static class Extension {
        public static R Let<T, R>(this T value, Func<T, R> func) => func(value);

        public static T Also<T>(this T value, Action<T> action) {
            action(value);
            return value;
        }
        
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action) {
            foreach (var value in sequence) {
                action(value);
            }
        }

        public static void ForEachIndexed<T>(this IEnumerable<T> sequence, Action<T, int> action) {
            var index = 0;
            foreach (var value in sequence) {
                action(value, index++);
            }
        }

        public static string JoinToString<T>(this IEnumerable<T> sequence, string separator) {
            return string.Join(separator, sequence.Select(it => it.ToString()).ToArray());
        }

        public static string JoinToString<T, R>(this IEnumerable<T> sequence, string separator, Func<T, R> func)
            => JoinToString(sequence.Select(func), separator);

        public static int[] ToIntArray(this IEnumerable<EItemDescType> array) => array.Select(it => (int) it).ToArray();
        
        public static ConfigEntry<T> Bind<T>(this ConfigFile config, ref T field, string section, string key, ConfigDescription configDescription = null) {
            var defaultValue = field;
            var entry = config.Bind(section, key, defaultValue, configDescription);
            field = entry.Value;
            return entry;
        }
        
        public static ConfigEntry<T> Bind<T>(this ConfigFile config, ref T field, string section, string key, string description) {
            var defaultValue = field;
            var entry = config.Bind(section, key, defaultValue, description);
            field = entry.Value;
            return entry;
        }

        public static ItemProto SetIconSprite(this ItemProto proto, Sprite icon) {
            ref var iconSprite = ref AccessTools.FieldRefAccess<ItemProto, Sprite>(proto, "_iconSprite");
            iconSprite = icon;
            return proto;
        }
        
        public static RecipeProto SetIconSprite(this RecipeProto proto, Sprite icon) {
            ref var iconSprite = ref AccessTools.FieldRefAccess<RecipeProto, Sprite>(proto, "_iconSprite");
            iconSprite = icon;
            return proto;
        }
        
        public static void Add<T>(this ProtoSet<T> protoSet, T proto) where T : Proto {
            var oldLength = protoSet.Length;
            Util.AddToArray(ref protoSet.dataArray, proto);

            var dataIndices = AccessTools.FieldRefAccess<ProtoSet<T>, Dictionary<int, int>>(protoSet, "dataIndices");
            dataIndices.Add(proto.ID, oldLength);

            if (protoSet is StringProtoSet stringProtoSet) {
                var nameIndices = AccessTools.FieldRefAccess<StringProtoSet, Dictionary<string, int>>(stringProtoSet, "nameIndices");
                nameIndices.Add(proto.Name, oldLength);
            }
        }

        public static void Add<T>(this ProtoSet<T> protoSet, ICollection<T> protos) where T : Proto {
            var oldLength = protoSet.Length;
            Util.AddToArray(ref protoSet.dataArray, protos);

            var dataIndices = AccessTools.FieldRefAccess<ProtoSet<T>, Dictionary<int, int>>(protoSet, "dataIndices");
            protos.ForEachIndexed((proto, index) => dataIndices.Add(proto.ID, index + oldLength));

            if (protoSet is StringProtoSet stringProtoSet) {
                var nameIndices = AccessTools.FieldRefAccess<StringProtoSet, Dictionary<string, int>>(stringProtoSet, "nameIndices");
                protos.ForEachIndexed((proto, index) => nameIndices.Add(proto.Name, index + oldLength));
            }
        }
        
        public static T GetPropValue<T>(this Type type, object instance, string name, object[] index = null) where T : class 
            => type.GetProperty(name, typeof(T))?.GetValue(instance, index) as T;

        public static T GetStaticPropValue<T>(this Type type, string name, object[] index = null) where T : class
            => GetPropValue<T>(type, type, name, index);

        public static void SetPropValue<T>(this Type type, object instance, string name, T value, object[] index = null) where T : class 
            => type.GetProperty(name, typeof(T))?.SetValue(instance, value, index);

        public static void SetStaticPropValue<T>(this Type type, string name, T value, object[] index = null) where T : class
            => SetPropValue(type, type, name, value, index);

        public static T Copy<T>(this T orig) where T : class {
            var type = orig.GetType();
            var instance = Activator.CreateInstance(type);
            type.GetFields().Where(field => !field.IsLiteral && !field.IsStatic).ForEach(field => {
                field.SetValue(instance, field.GetValue(orig));
            });
            type.GetProperties().Where(property => property.CanRead && property.CanWrite).ForEach(property => {
                property.SetValue(instance, property.GetValue(orig, null), null);
            });
            return instance as T;
        }
    }
}