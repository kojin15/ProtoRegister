using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using ProtoRegister.Proto;

namespace ProtoRegister.Patch {
    [HarmonyPatch(typeof(VFPreload), "InvokeOnLoad")]
    internal static class InvokeOnLoadPatch {
        private static void Prefix() {
            ProtoRegister.Logger.LogInfo("InvokeOnLoadPatch.Prefix invoked.");
            
            LDB.strings.AddProtos(ProtoRegister.AddStringProtos);
            JPTranslatePatch();
        }
            
        private static void Postfix() {
            ProtoRegister.Logger.LogInfo("InvokeOnLoadPatch.Postfix invoked.");
            
            LDB.items.AddProtos(ProtoRegister.AddItemProtos);
            foreach (var itemProto in ProtoRegister.AddItemProtos) {
                ProtoRegister.BindConfig(itemProto);
            }
            
            LDB.recipes.AddProtos(ProtoRegister.AddRecipeProtos);
            foreach (var recipeProto in ProtoRegister.AddRecipeProtos) {
                ProtoRegister.BindConfig(recipeProto);
            }
            
            foreach (var proto in ProtoRegister.AddRecipeProtos.OfType<CustomRecipeProto>()) {
                if (proto.preTechID == 0) continue;
                var newArray = LDB.techs.Select(proto.preTechID).UnlockRecipes.Add(proto.ID);
                LDB.techs.Select(proto.preTechID).UnlockRecipes = newArray;
            }
        }
        
        private static void JPTranslatePatch() {
            var type = AccessTools.TypeByName("DSPJapanesePlugin.DSPJapaneseMod");
            if (type == null) return;
            
            ProtoRegister.Logger.LogInfo("Add to translation dictionary.");
            if (AccessTools.Property(type, "JPDictionary").GetValue(type, null) is Dictionary<string, string> dic) {
                foreach (var protoJP in ProtoRegister.AddStringProtos.OfType<StringProtoJP>()) { 
                    dic.Add(protoJP.name, protoJP.JAJP);
                }
            }
        }
    }

    [HarmonyPatch(typeof (GameHistoryData), "Import")]
    internal static class GameHistoryDataPatch {
        private static void Postfix(GameHistoryData __instance) {
            ProtoRegister.Logger.LogInfo("GameHistoryDataPatch.Postfix invoked.");
            foreach (var proto in ProtoRegister.AddRecipeProtos.OfType<CustomRecipeProto>()) {
                if (proto.preTech != null && __instance.TechState(proto.preTechID).unlocked && !__instance.RecipeUnlocked(proto.ID)) {
                    __instance.UnlockRecipe(proto.ID);
                }
            }
        }
    }
}