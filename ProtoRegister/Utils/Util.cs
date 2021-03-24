using System;
using System.Collections.Generic;
using System.Linq;

namespace ProtoRegister.Utils {
    public static class Util {
        public static void AddToArray<T>(ref T[] array, ICollection<T> values) {
            var oldLength = array.Length;
            Array.Resize(ref array,  oldLength + values.Count);
            for (var i = 0; i < values.Count; i++) {
                array[oldLength + i] = values.ElementAt(i);
            }
        }

        public static void AddToArray<T>(ref T[] array, T value) {
            Array.Resize(ref array,  array.Length + 1);
            array[array.Length - 1] = value;
        }

    }
}