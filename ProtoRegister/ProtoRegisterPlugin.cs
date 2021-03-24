using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using ProtoRegister.Protos;
using ProtoRegister.Utils;
using UnityEngine;

namespace ProtoRegister {
    [BepInDependency("Appun.plugins.dspmod.DSPJapanesePlugin", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(ModGuid, ModName, ModVersion)]
    public class ProtoRegisterPlugin : BaseUnityPlugin {
        public const string ModGuid = "kojin15.plugins.dsp.ProtoRegister";
        public const string ModName = "ProtoRegister";
        public const string ModVersion = "1.0.0";
        
        private void Awake() {
            ProtoRegister.Logger = this.Logger;
            ProtoRegister.Config = this.Config;
            
            ProtoRegister.AddStringProtos = new List<StringProto>();
            ProtoRegister.AddItemProtos = new List<ItemProto>();
            ProtoRegister.AddRecipeProtos = new List<RecipeProto>();
        }
        
        private void Start() {
            ProtoRegister.PreAddAction += PreLoad;
            ProtoRegister.PostAddAction += PostLoad;
            
            var harmony = new Harmony(ModGuid + ".patch");
            harmony.PatchAll(typeof(ProtoRegisterPatch));
        }
        
        private static void PreLoad() {
            var stringSapling = new StringProtoJP(28500, "sapling", "Sapling", "苗木");
            ProtoRegister.RegisterString(stringSapling);
        }
        
        private static void PostLoad() {
            var bundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("ProtoRegister.resources"));
            var iconSapling = bundle.LoadAsset<Sprite>("iconSapling");
            
            var itemSapling = new ItemProto {
                Name = "sapling",
                ID = 9150,
                GridIndex = 1506,
                Type = EItemType.Resource,
                StackSize = 200,
                Description = "",
                DescFields = new [] {EItemDescType.MadeIn}.ToIntArray()
            }.SetIconSprite(iconSapling);
            ProtoRegister.RegisterItem(itemSapling);

            var recipeSapling = new RecipeProto {
                Name = "sapling",
                ID = 220,
                GridIndex = 1610,
                Type = ERecipeType.Assemble,
                Items = new[] {1030},
                ItemCounts = new[] {1},
                Results = new[] {itemSapling.ID},
                ResultCounts = new[] {5},
                TimeSpend = 30,
                Handcraft = true,
                Description = "",
                preTech = LDB.techs.Select(1121)
            }.SetIconSprite(iconSapling);
            ProtoRegister.RegisterRecipe(recipeSapling);
        }
    }
}