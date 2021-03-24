using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using BepInEx.Logging;
using ProtoRegister.Utils;

namespace ProtoRegister {
    public static class ProtoRegister {
        internal static ManualLogSource Logger;
        internal static ConfigFile Config;

        private const string SectionString = "String";
        private const string SectionItem = "Item";
        private const string SectionRecipe = "Recipe";

        public static Action PreAddAction, PostAddAction;

        internal static List<StringProto> AddStringProtos;
        internal static List<ItemProto> AddItemProtos;
        internal static List<RecipeProto> AddRecipeProtos;

        public static void RegisterString(StringProto proto) {
            BindConfig(proto);
            AddStringProtos.Add(proto);
        }
        
        public static void RegisterItem(ItemProto proto) {
            BindConfig(proto);
            AddItemProtos.Add(proto);
        }
        
        public static void RegisterRecipe(RecipeProto proto) {
            BindConfig(proto);
            AddRecipeProtos.Add(proto);
        }

        private static void BindConfig(Proto proto) {
            switch (proto) {
                case StringProto _string:
                    Config.Bind(ref _string.ID, SectionString, _string.Name + ":ID");
                    break;
                case ItemProto item:
                    Config.Bind(ref item.ID, SectionItem, item.Name + ":ID");
                    Config.Bind(ref item.GridIndex, SectionItem, item.Name + ":GridIndex");
                    break;
                case RecipeProto recipe:
                    Config.Bind(ref recipe.ID, SectionRecipe, recipe.Name + ":ID");
                    Config.Bind(ref recipe.GridIndex, SectionRecipe, recipe.Name + ":GridIndex");
                    break;
            }
        }
    }
}