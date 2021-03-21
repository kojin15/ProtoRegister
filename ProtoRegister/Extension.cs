using System;
using System.Collections.Generic;
using HarmonyLib;
using ProtoRegister.Proto;

namespace ProtoRegister {
    public static class Extension {
        public static int[] ToIntArray(this EItemDescType[] array) {
            return Array.ConvertAll(array, value => (int) value);
        }

        public static T[] Add<T>(this T[] array, T value) {
            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = value;
            return array;
        }

        public static void AddProtos<T>(this ProtoSet<T> protoSet, List<T> protos) where T : global::Proto {
            var dataArray = protoSet.dataArray;
            protoSet.Init(dataArray.Length + protos.Count);
            for (var index = 0; index < dataArray.Length; ++index) {
                protoSet.dataArray[index] = dataArray[index];
            }
            for (var index = 0; index < protos.Count; ++index) {
                protoSet.dataArray[dataArray.Length + index] = protos[index];
            }

            var dic1 = new Dictionary<int, int>();
            for (var index = 0; index < protoSet.dataArray.Length; ++index) {
                protoSet.dataArray[index].sid = protoSet.dataArray[index].SID;
                dic1[protoSet.dataArray[index].ID] = index;
            }
            Traverse.Create(protoSet).Field("dataIndices").SetValue(dic1);

            if (protoSet is ProtoSet<StringProto>) {
                var dic2 = Traverse.Create(protoSet).Field("nameIndices").GetValue<Dictionary<string, int>>();
                for (var length = dataArray.Length; length < protoSet.dataArray.Length; ++length) {
                    dic2[protoSet.dataArray[length].Name] = length;
                }
                Traverse.Create(protoSet).Field("nameIndices").SetValue(dic2);
            }
        }
    }
}