using System.Collections.Generic;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using ProtoRegister.Proto;

namespace ProtoRegister {
    public static class ProtoRegister {
        internal static ManualLogSource Logger;
        internal static ConfigFile Config;

        private const string SectionItem = "Item";
        private const string SectionRecipe = "Recipe";
        
        internal static List<StringProto> AddStringProtos;
        internal static List<ItemProto> AddItemProtos;
        internal static List<RecipeProto> AddRecipeProtos;

        public static void RegisterString(StringProto proto) {
            AddStringProtos.Add(proto);
        }
        
        public static void RegisterItem(ItemProto proto) {
            if (proto is CustomItemProto itemProto) {
                var icon = itemProto.customIcon;
                Traverse.Create(proto).Field("_iconSprite").SetValue(icon);
            }
            AddItemProtos.Add(proto);
        }
        
        public static void RegisterRecipe(RecipeProto proto) {
            if (proto is CustomRecipeProto recipeProto) {
                var icon = recipeProto.customIcon;
                Traverse.Create(proto).Field("_iconSprite").SetValue(icon);
            }
            AddRecipeProtos.Add(proto);
        }
        
        internal static void BindConfig(global::Proto proto) {
            switch (proto) {
                case ItemProto item:
                    item.ID = Config.Bind(SectionItem, item.Name + ":ID", item.ID).Value;
                    item.GridIndex = Config.Bind(SectionItem, item.Name + ":GridIndex", item.GridIndex).Value;
                    break;
                case RecipeProto recipe:
                    recipe.ID = Config.Bind(SectionRecipe, recipe.Name + ":ID", recipe.ID).Value;
                    recipe.GridIndex = Config.Bind(SectionRecipe, recipe.Name + ":GridIndex", recipe.GridIndex).Value;
                    break;
            }
        }
    }
}