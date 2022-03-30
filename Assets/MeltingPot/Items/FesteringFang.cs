using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using MeltingPot.Utils;
using UnityEngine.Networking;

namespace MeltingPot.Items
{
    public class FesteringFang : ItemBase<FesteringFang>
    {

        public static float blightChance = 0.1f;
        public override string ItemName => "Festering Fang";
        public override string ItemLangTokenName => "FESTERFANG";
        public override string ItemPickupDesc => $"Chance to apply <style=cArtifact>blight</style> on hit";
        public override string ItemFullDescription => $"<style=cArtifact>{blightChance*100}%</style> <style=cStack>(+{blightChance*100}</style> per stack) of applying <style=cArtifact>blight</style> on hit, Past 100% chance gain a chance to apply multiple stacks.";
        public override string ItemLore => "[A small note tagged to the Fang]\n\n" +
            "OBJECT: Canine Fang, ORIGIN: Subject `ACRID` \n\nRESEARCHERS NOTE: With the subject freed the Fang has undergone a most interesting mutation, the poison it secretes has become fouler, more viscous, capable of melting through whatever it touches. \n\nI fear the Fang cannot last much longer like this, the poison appearss to be dissolving it from the inside.";
        public override string VoidCounterpart => "PenitFang_ItemDef";

		public static BepInEx.Logging.ManualLogSource BSModLogger;
        public static GameObject ItemModel => Assets.mainAssetBundle.LoadAsset<GameObject>("assets/meltingpot/mpassets/itemprefabs/fester_fang/festerfang.prefab");
        public static GameObject ItemBodyModelPrefab;

        public override void Init(ConfigFile config, bool enabled) {
            CreateItem("FesterFang_ItemDef", enabled);
            if (enabled) {
                ItemDef.requiredExpansion = MeltingPotPlugin.sotvRef;
                CreateLang();
                Hooks();
            }
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules() {
            ItemBodyModelPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>("assets/meltingpot/mpassets/itemprefabs/fester_fang/displayfesterfang.prefab");
            Vector3 generalScale = new Vector3(1f, 1f, 1f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
            rules.Add("mdlCommandoDualies", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MuzzleLeft",
                    localPos = new Vector3(-0.19686F, 0.00684F, -0.15061F),
                    localAngles = new Vector3(86.04195F, 301.2451F, 224.1474F),
                    localScale = new Vector3(0.03109F, 0.0366F, 0.03109F)
                },
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MuzzleRight",
                    localPos = new Vector3(-0.19686F, 0.00684F, -0.15061F),
                    localAngles = new Vector3(81.90211F, 283.6174F, 206.5877F),
                    localScale = new Vector3(0.03109F, 0.0366F, 0.03109F)
                }
            });
            rules.Add("mdlHuntress", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0.0342F, 0.0789F, 0.10822F),
                    localAngles = new Vector3(355.5165F, 80.96111F, 342.9368F),
                    localScale = new Vector3(0.01855F, 0.02085F, 0.01978F)
                }
            });
            rules.Add("mdlToolbot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.92833F, 2.42578F, -1.21285F),
                    localAngles = new Vector3(4.651F, 275.5355F, 289.1668F),
                    localScale = new Vector3(0.22182F, 0.22182F, 0.22182F)
                },
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(1.30207F, 2.42594F, -1.21282F),
                    localAngles = new Vector3(4.651F, 275.5355F, 289.1668F),
                    localScale = new Vector3(0.22182F, 0.22182F, 0.22182F)
                }
            });
            rules.Add("mdlEngi", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(-0.16571F, 0.03522F, -0.15658F),
                    localAngles = new Vector3(0.00014F, 74.02521F, 178.5567F),
                    localScale = new Vector3(0.03317F, 0.03345F, 0.03575F)
                }
            });
            rules.Add("mdlMage", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.0011F, 0.20717F, -0.13495F),
                    localAngles = new Vector3(353.7739F, 274.1951F, 204.5648F),
                    localScale = new Vector3(0.01945F, 0.01853F, 0.02098F)
                },
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.00114F, 0.21513F, -0.057F),
                    localAngles = new Vector3(353.7739F, 274.1951F, 204.5648F),
                    localScale = new Vector3(0.01945F, 0.02369F, 0.02098F)
                },
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.00114F, 0.21462F, 0.00036F),
                    localAngles = new Vector3(353.7739F, 274.1951F, 204.5648F),
                    localScale = new Vector3(0.01945F, 0.03646F, 0.02098F)
                }
            });
            rules.Add("mdlMerc", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HandL",
                    localPos = new Vector3(0.17868F, 0.1382F, 0.07297F),
                    localAngles = new Vector3(46.2136F, 352.0412F, 109.7653F),
                    localScale = new Vector3(0.03819F, 0.03892F, 0.03991F)
                }
            });
            rules.Add("mdlTreebot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "FootBackL",
                    localPos = new Vector3(-0.01263F, 1.44804F, 0.02164F),
                    localAngles = new Vector3(359.4587F, 94.65348F, 180.6844F),
                    localScale = new Vector3(0.08077F, 0.08077F, 0.08077F)
                }
            });
            rules.Add("mdlLoader", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.00146F, 0.18753F, -0.12083F),
                    localAngles = new Vector3(8.07365F, 63.92918F, 15.44215F),
                    localScale = new Vector3(0.01496F, 0.01515F, 0.01214F)
                }
            });
            rules.Add("mdlCroco", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.94377F, 4.56508F, -0.36384F),
                    localAngles = new Vector3(292.4587F, 348.0707F, 193.7799F),
                    localScale = new Vector3(0.20933F, 0.20044F, 0.1971F)
                }
            });
            rules.Add("mdlCaptain", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HandR",
                    localPos = new Vector3(-0.02086F, 0.21162F, -0.03956F),
                    localAngles = new Vector3(3.08573F, 179.3505F, 192.6827F),
                    localScale = new Vector3(0.02F, 0.02F, 0.02F)
                }
            });
            rules.Add("mdlBandit2", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Hat",
                    localPos = new Vector3(-0.11117F, 0.03828F, -0.00788F),
                    localAngles = new Vector3(350.6534F, 282.4872F, 292.205F),
                    localScale = new Vector3(0.01855F, 0.01968F, 0.01928F)
                }
            });
            rules.Add("mdlEngiTurret", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "LegBar1",
                    localPos = new Vector3(0F, 1.24355F, -0.77905F),
                    localAngles = new Vector3(358.8841F, 269.3587F, 274.5442F),
                    localScale = new Vector3(0.24041F, 0.28766F, 0.26564F)
                }
            });
            rules.Add("mdlScav", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Backpack",
                    localPos = new Vector3(-14.1216F, 13.15656F, 1.79306F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.21702F, 0.28935F, 0.21702F)
                }
            });
            rules.Add("mdlRailGunner", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "GunRoot",
                    localPos = new Vector3(0F, -0.07569F, -0.30281F),
                    localAngles = new Vector3(85.25381F, 180F, 180F),
                    localScale = new Vector3(0.02103F, 0.02103F, 0.02103F)
                }
            });
            rules.Add("mdlVoidSurvivor", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0.17477F, -0.03393F, -0.23181F),
                    localAngles = new Vector3(2.91521F, 238.1028F, 323.6552F),
                    localScale = new Vector3(0.02942F, 0.02942F, 0.02942F)
                },
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(-0.14927F, -0.05054F, -0.15467F),
                    localAngles = new Vector3(337.1328F, 279.521F, 338.6101F),
                    localScale = new Vector3(0.02942F, 0.02942F, 0.02942F)
                }
            });
            return rules;
        }

        public override void Hooks() {
            On.RoR2.GlobalEventManager.OnHitEnemy += applyblight;
        }

        private void applyblight(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, global::RoR2.GlobalEventManager self, global::RoR2.DamageInfo damageInfo, GameObject victim) {
            try {
                if (NetworkServer.active && !damageInfo.rejected) {
                    if (damageInfo.attacker.GetComponent<CharacterBody>() && victim.GetComponent<CharacterBody>()) {
                        var count = GetCount(damageInfo.attacker.GetComponent<CharacterBody>());
                        if (count > 0) {
                            var cumulativeChance = blightChance * count;
                            for (int x = 0; x < blightChance * count; x++) {
                                if (Util.CheckRoll((Mathf.Clamp(cumulativeChance, 0, 1)) * 100f * damageInfo.procCoefficient)) {
                                    DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.Blight, 5f);
                                }
                                cumulativeChance -= 1;
                            }
                        }
                    }
                }
            }
            catch { }
            orig(self, damageInfo, victim);
        }

    }
}