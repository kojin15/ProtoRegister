using System;
using System.Collections.Generic;

namespace ProtoRegister.Utils {
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
}