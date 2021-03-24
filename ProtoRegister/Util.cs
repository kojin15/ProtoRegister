using System;
using System.Collections.Generic;
using HarmonyLib;
using ProtoRegister.Proto;
using UnityEngine;

namespace ProtoRegister {
    public static class Util {
        public static void AddToArray<T>(ref T[] array, List<T> values) {
            var oldSize = array.Length;
            Array.Resize(ref array,  oldSize + values.Count);
            for (var i = 0; i < values.Count; i++) {
                array[oldSize + i] = values[i];
            }
        }
        public static void AddToArray<T>(ref T[] array, T[] values) {
            var oldSize = array.Length;
            Array.Resize(ref array,  oldSize + values.Length);
            for (var i = 0; i < values.Length; i++) {
                array[oldSize + i] = values[i];
            }
        }

        public static void AddToArray<T>(ref T[] array, T value) {
            Array.Resize(ref array,  array.Length + 1);
            array[array.Length - 1] = value;
        }

    }
    
    public static class Extension {
        public static int[] ToIntArray(this EItemDescType[] array) => Array.ConvertAll(array, value => (int) value);
        

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

        public static void AddProtos<T>(this ProtoSet<T> protoSet, List<T> protos) where T : global::Proto {
            var oldSize = protoSet.Length;
            Util.AddToArray(ref protoSet.dataArray, protos);

            ref var dataIndices = ref AccessTools.FieldRefAccess<ProtoSet<T>, Dictionary<int, int>>(protoSet, "dataIndices");
            for (var index = oldSize; index < protoSet.Length; index++) {
                dataIndices[protoSet.dataArray[index].ID] = index;
            }

            if (protoSet is StringProtoSet stringProtoSet) {
                ref var nameIndices = ref AccessTools.FieldRefAccess<StringProtoSet, Dictionary<string, int>>(stringProtoSet, "nameIndices");
                for (var index = oldSize; index < protoSet.Length; index++) {
                    nameIndices[protoSet.dataArray[index].Name] = index;
                }
            }
        }
    }
}