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
using System.Security;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace MeltingPot
{
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [BepInDependency("com.bepis.r2api")]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(R2API.ContentAddition), nameof(LanguageAPI), nameof(PrefabAPI), nameof(SoundAPI), nameof(OrbAPI),
        nameof(DotAPI), nameof(NetworkingAPI), nameof(DirectorAPI), nameof(RecalculateStatsAPI), nameof(UnlockableAPI))]
    public class MeltingPotPlugin : BaseUnityPlugin
    {
        public static PluginInfo pluginInfo;
        public const string ModGuid = "com.Shasocais.MeltingPot";
        public const string ModName = "Melting_Pot";
        public const string ModVer = "0.0.42";
        private static string[] blList = {
        };
        private static List<string> aiBlist = new List<string>(blList);

        public static BepInEx.Logging.ManualLogSource ModLogger;
        public static List<ItemBase> Items = new List<ItemBase>();
        public static Dictionary<ItemBase, bool> ItemStatusDictionary = new Dictionary<ItemBase, bool>();


        private void Awake() {
            pluginInfo = this.Info;
            ModLogger = this.Logger;
            Assets.PopulateAssets();
            ApplyShaders();
            AttachControllerFinderToObjects(Assets.mainAssetBundle);

            //Item Initialization
            var ItemTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ItemBase)));
			/*using (var bankStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MeltingPot_SoundBank.bnk")) {
                var bytes = new byte[bankStream.Length];
                bankStream.Read(bytes, 0, bytes.Length);
                SoundAPI.SoundBanks.Add(bytes);
            }*/
            ModLogger.LogInfo("----------------------ITEMS--------------------");

            ContentPackProvider.Initialize();
            foreach (var itemType in ItemTypes) {
                var item_to_ready = (ItemBase)System.Activator.CreateInstance(itemType);
                if (ValidateItem(item_to_ready, Items)) {
                    item_to_ready.Init(Config);
                    Debug.Log(MeltingPot.Items.BurningSoul.ItemBodyModelPrefab);
                    ModLogger.LogInfo("Item: " + item_to_ready.ItemName + " Initialized!");
                }
            }
            On.RoR2.ItemDisplayRuleSet.Init += ApplyAllItemRules;
            ModLogger.LogInfo("-----------------------------------------------");
            ModLogger.LogInfo("MELTINGPOT INITIALIZATIONS DONE");
            ModLogger.LogInfo($"Items Enabled: {ItemStatusDictionary.Count}");
            ModLogger.LogInfo("-----------------------------------------------");

        }

        public bool ValidateItem(ItemBase item, List<ItemBase> itemList) {
            var enabled = Config.Bind<bool>("Item: " + item.ItemName, "Enable Item?", true, "Should this item appear in runs?").Value;
            var aiBlacklist = false;
            if (aiBlist.Contains(item.ItemLangTokenName)) {
                aiBlacklist = Config.Bind<bool>("Item: " + item.ItemName, "Blacklist Item from AI Use?", true, "Should the AI not be able to obtain this item?").Value;
            } else {
                aiBlacklist = Config.Bind<bool>("Item: " + item.ItemName, "Blacklist Item from AI Use?", false, "Should the AI not be able to obtain this item?").Value;
            }

            ItemStatusDictionary.Add(item, enabled);

            if (enabled) {
                itemList.Add(item);
                if (aiBlacklist) {
                    item.AIBlacklisted = true;
                }
            }
            return enabled;
        }

        public static void ApplyShaders() {
            var materials = Assets.mainAssetBundle.LoadAllAssets<Material>();
            foreach (Material material in materials)
                if (material.shader.name.StartsWith("StubbedShader"))
                    material.shader = LegacyResourcesAPI.Load<Shader>("shaders" + material.shader.name.Substring(13));
        }

        public static void AttachControllerFinderToObjects(AssetBundle assetbundle) {
            if (!assetbundle) { return; }

            var gameObjects = assetbundle.LoadAllAssets<GameObject>();

            foreach (GameObject gameObject in gameObjects) {
                var foundRenderers = gameObject.GetComponentsInChildren<Renderer>().Where(x => x.sharedMaterial && x.sharedMaterial.shader.name.StartsWith("Hopoo Games"));

                foreach (Renderer renderer in foundRenderers) {
                    var controller = renderer.gameObject.AddComponent<MaterialControllerComponents.HGControllerFinder>();
                    controller.Renderer = renderer;
                }
            }

            gameObjects = null;
        }

        private static void ApplyAllItemRules(On.RoR2.ItemDisplayRuleSet.orig_Init orig) {
            orig();
            foreach (var bodyPrefab in BodyCatalog.allBodyPrefabs) {
                var characterModel = bodyPrefab.GetComponentInChildren<CharacterModel>();
                if (characterModel) {
                    if (!characterModel.itemDisplayRuleSet) {
                        characterModel.itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
                    }
                    var modelName = characterModel.name;
                    var bodyName = bodyPrefab.name;
                    bool allowDefault = true;
                    foreach (ItemBase customItem in ItemStatusDictionary.Keys) {
                        var customRules = customItem.ItemDisplayRules;
                        if (customRules != null) {
                            //if a specific rule for this model exists, or the model has no rules for this item
                            ItemDisplayRule[] rules = null;
                            if (customRules.TryGetRules(modelName, out rules) || customRules.TryGetRules(bodyName, out rules) || (
                                    allowDefault &&
                                    characterModel.itemDisplayRuleSet.GetItemDisplayRuleGroup(customItem.ItemDef.itemIndex).rules == null
                                )) {
                                characterModel.itemDisplayRuleSet.SetDisplayRuleGroup(customItem.ItemDef, new DisplayRuleGroup { rules = rules });
                            }
                        }
                    }
                    characterModel.itemDisplayRuleSet.GenerateRuntimeValues();
                }
            }
        }
    }
}