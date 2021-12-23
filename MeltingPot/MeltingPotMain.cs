using MeltingPot.Items;
using MeltingPot.Utils;
using BepInEx;
using R2API;
using R2API.Networking;
using R2API.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using RoR2;

namespace MeltingPot
{
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [BepInDependency("com.bepis.r2api")]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(BuffAPI), nameof(LanguageAPI), nameof(ResourcesAPI), nameof(PrefabAPI), nameof(SoundAPI), nameof(OrbAPI),
        nameof(DotAPI), nameof(NetworkingAPI), nameof(EffectAPI), nameof(DirectorAPI), nameof(ProjectileAPI), nameof(ArtifactAPI), nameof(RecalculateStatsAPI), nameof(UnlockableAPI))]
    public class MeltingPotPlugin : BaseUnityPlugin
    {
        public const string ModGuid = "com.Shasocais.MeltingPot";
        public const string ModName = "Melting Pot";
        public const string ModVer = "0.0.25";
        private static string[] blList = {
            "REACTIVE_PLATE"
        };
        private static List<string> aiBlist = new List<string>(blList);

        public static BepInEx.Logging.ManualLogSource ModLogger;

        public static AssetBundle MainAssets;

        public static Dictionary<string, string> ShaderLookup = new Dictionary<string, string>()
        {
            {"fake ror/hopoo games/deferred/hgstandard", "shaders/deferred/hgstandard"},
            {"fake ror/hopoo games/fx/hgcloud intersection remap", "shaders/fx/hgintersectioncloudremap" },
            {"fake ror/hopoo games/fx/hgcloud remap", "shaders/fx/hgcloudremap" },
            {"fake ror/hopoo games/fx/hgdistortion", "shaders/fx/hgdistortion" },
            {"fake ror/hopoo games/deferred/hgsnow topped", "shaders/deferred/hgsnowtopped" }
        };
        //public List<ItemBase> Items = new List<ItemBase>();
        public static List<ItemBase> Items = new List<ItemBase>();
        public static Dictionary<ItemBase, bool> ItemStatusDictionary = new Dictionary<ItemBase, bool>();

        private void Awake()
        {
            ModLogger = this.Logger;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MeltingPot.Assets.meltingpotassets"))
            {
                MainAssets = AssetBundle.LoadFromStream(stream);
                ModLogger.LogInfo("----Assets loaded----");
            }
            ShaderConversion(MainAssets);

            AttachControllerFinderToObjects(MainAssets);

            using (var bankStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MeltingPot.Assets.MeltingPot_SoundBank.bnk")) 
            {
                var bytes = new byte[bankStream.Length];
                bankStream.Read(bytes, 0, bytes.Length);
                SoundAPI.SoundBanks.Add(bytes);
			}
            //Item Initialization
            var ItemTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ItemBase)));

            ModLogger.LogInfo("----------------------ITEMS--------------------");

            foreach (var itemType in ItemTypes)
            {
                var item_to_ready = (ItemBase)System.Activator.CreateInstance(itemType);
                if (ValidateItem(item_to_ready, Items))
                {
                    item_to_ready.Init(Config);
                    ModLogger.LogInfo("Item: " + item_to_ready.ItemName + " Initialized!");
                }
            }

            ModLogger.LogInfo("-----------------------------------------------");
            ModLogger.LogInfo("MELTINGPOT INITIALIZATIONS DONE");
            ModLogger.LogInfo($"Items Enabled: {ItemStatusDictionary.Count}");
            ModLogger.LogInfo("-----------------------------------------------");


        }

        public bool ValidateItem(ItemBase item, List<ItemBase> itemList)
        {
            var enabled = Config.Bind<bool>("Item: " + item.ItemName, "Enable Item?", true, "Should this item appear in runs?").Value;
            var aiBlacklist = false;
            if (aiBlist.Contains(item.ItemLangTokenName)) {
                aiBlacklist = Config.Bind<bool>("Item: " + item.ItemName, "Blacklist Item from AI Use?", true, "Should the AI not be able to obtain this item?").Value;
            } else {
                aiBlacklist = Config.Bind<bool>("Item: " + item.ItemName, "Blacklist Item from AI Use?", false, "Should the AI not be able to obtain this item?").Value;
            }

            ItemStatusDictionary.Add(item, enabled);

            if (enabled)
            {
                itemList.Add(item);
                if (aiBlacklist)
                {
                    item.AIBlacklisted = true;
                }
            }
            return enabled;
        }

        public static void ShaderConversion(AssetBundle assets)
        {
            var materialAssets = assets.LoadAllAssets<Material>().Where(material => material.shader.name.StartsWith("Fake RoR"));

            foreach (Material material in materialAssets)
            {
                var replacementShader = Resources.Load<Shader>(ShaderLookup[material.shader.name.ToLower()]);
                if (replacementShader) { material.shader = replacementShader; }

            }
        }

        public static void AttachControllerFinderToObjects(AssetBundle assetbundle)
        {
            if (!assetbundle) { return; }

            var gameObjects = assetbundle.LoadAllAssets<GameObject>();

            foreach (GameObject gameObject in gameObjects)
            {
                var foundRenderers = gameObject.GetComponentsInChildren<Renderer>().Where(x => x.sharedMaterial && x.sharedMaterial.shader.name.StartsWith("Hopoo Games"));

                foreach (Renderer renderer in foundRenderers)
                {
                    var controller = renderer.gameObject.AddComponent<MaterialControllerComponents.HGControllerFinder>();
                    controller.Renderer = renderer;
                }
            }

            gameObjects = null;
        }
    }
}