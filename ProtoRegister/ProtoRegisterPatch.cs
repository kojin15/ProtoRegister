using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using ProtoRegister.Protos;
using ProtoRegister.Utils;

namespace ProtoRegister {
    public static class ProtoRegisterPatch {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(VFPreload), "InvokeOnLoad")]
        private static void InvokeOnLoadPrePatch() {
            ProtoRegister.Logger.LogInfo("InvokeOnLoadPatch.Prefix invoked.");
            
            ProtoRegister.PreAddAction?.Invoke();
            
            LDB.strings.Add(ProtoRegister.AddStringProtos);
            JPTranslatePatch();
        }
        
        private static void JPTranslatePatch() {
            var type = AccessTools.TypeByName("DSPJapanesePlugin.DSPJapaneseMod");
            if (type == null) return;
                    
            ProtoRegister.Logger.LogInfo("Add to translation dictionary.");
            if (AccessTools.Property(type, "JPDictionary").GetValue(type, null) is Dictionary<string, string> dic) {
                ProtoRegister.AddStringProtos.OfType<StringProtoJP>()
                    .ForEach(protoJP => dic.Add(protoJP.name, protoJP.JAJP));
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(VFPreload), "InvokeOnLoad")]
        private static void InvokeOnLoadPostPatch() {
            ProtoRegister.Logger.LogInfo("InvokeOnLoadPatch.Postfix invoked.");
            
            ProtoRegister.PostAddAction?.Invoke();
            
            LDB.items.Add(ProtoRegister.AddItemProtos);
            LDB.recipes.Add(ProtoRegister.AddRecipeProtos);
            ProtoRegister.AddRecipeProtos
                .Where(proto => proto.preTech != null)
                .ForEach(proto => Util.AddToArray(ref proto.preTech.UnlockRecipes, proto.ID));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof (GameHistoryData), "Import")]
        private static void GameHistoryDataPostPatch(GameHistoryData __instance) {
            ProtoRegister.Logger.LogInfo("GameHistoryDataPatch.Postfix invoked.");
            ProtoRegister.AddRecipeProtos
                .Where(proto => proto.preTech != null && __instance.TechState(proto.preTech.ID).unlocked && !__instance.RecipeUnlocked(proto.ID))
                .ForEach(proto => __instance.UnlockRecipe(proto.ID));
        }
    }
}