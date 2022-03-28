using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using MeltingPot.Utils;
using UnityEngine.Networking;
using System.Linq;

namespace MeltingPot.Items
{
    public class SerratedPellets: ItemBase<SerratedPellets>
    {

        public static float bleedStackBase = 15f;
        public static float stackDegen = 3f;
        public override string ItemName => "Serrated Pellets";
        public override string ItemLangTokenName => "SERRATEDPELLETS";
        public override string ItemPickupDesc => $"Apply <style=cDeath>Haemorrhage</style> to heavily bleeding enemies";
        public override string ItemFullDescription => $"Apply <style=cDeath>Haemorrhage</style> to enemies with <style=cIsUtility>{bleedStackBase}</style> <style=cStack>(- {stackDegen} per stack)</style> bleed stacks";
        public override string ItemLore => "[On the evidence bag]\n\n" +
            "Evidence found on site at homicide #867. Modifed ammunition designed for maximum internal damage.\n\nThe ammunition bears marks of amateur tooling, believed custom made by Suspect #2, a.k.a 'Desperate Outlaw'.";


        public static BepInEx.Logging.ManualLogSource BSModLogger;

        public static GameObject ItemModel => Assets.mainAssetBundle.LoadAsset<GameObject>("assets/meltingpot/mpassets/itemprefabs/serrated_pellets/serratedpellets.prefab");

		public override string VoidCounterpart => null;

		public static GameObject ItemBodyModelPrefab;

        public override void Init(ConfigFile config, bool enabled) {
            CreateItem("SerratedPellets_ItemDef", enabled);
            if (enabled) {
                CreateLang();
                Hooks();
            }
        }
        public override ItemDisplayRuleDict CreateItemDisplayRules() {
            ItemBodyModelPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>("assets/meltingpot/mpassets/itemprefabs/serrated_pellets/displayserratedpellets.prefab");
            Vector3 generalScale = new Vector3(1f, 1f, 1f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
            /*rules.Add("mdlCommandoDualies", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Base",
                    localPos = new Vector3(0.20272F, 0.12738F, -0.07128F),
                    localAngles = new Vector3(87.9534F, 290.8185F, 285.439F),
                    localScale = new Vector3(0.03109F, 0.0366F, 0.03109F)
                }
            });
            rules.Add("mdlHuntress", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Arrow",
                    localPos = new Vector3(0.03453F, 0.00813F, 0.11169F),
                    localAngles = new Vector3(270.9361F, 141.5653F, 127.4167F),
                    localScale = new Vector3(0.0442F, 0.04967F, 0.04712F)
                }
            });
            rules.Add("mdlToolbot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HandR",
                    localPos = new Vector3(2.1836F, 0.81396F, 0.20312F),
                    localAngles = new Vector3(86.41363F, 131.8821F, 124.6506F),
                    localScale = new Vector3(0.72109F, 0.72109F, 0.72109F)
                }
            });
            rules.Add("mdlEngi", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(-0.03298F, 0.21711F, -0.28224F),
                    localAngles = new Vector3(5.10691F, 179.9999F, 191.9932F),
                    localScale = new Vector3(0.04895F, 0.04084F, 0.05276F)
                }
            });
            rules.Add("mdlMage", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(-0.13007F, 0.23976F, -0.33291F),
                    localAngles = new Vector3(359.9597F, 97.04926F, 4.91771F),
                    localScale = new Vector3(0.05801F, 0.05526F, 0.06258F)
                },
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0.09444F, 0.23997F, -0.36069F),
                    localAngles = new Vector3(359.9597F, 97.04926F, 4.91771F),
                    localScale = new Vector3(0.05801F, 0.05526F, 0.06258F)
                }
            });
            rules.Add("mdlMerc", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HandR",
                    localPos = new Vector3(0.0691F, -0.00967F, 0.01725F),
                    localAngles = new Vector3(352.2195F, 0.81314F, 271.0739F),
                    localScale = new Vector3(0.08138F, 0.08292F, 0.11248F)
                }
            });
            rules.Add("mdlTreebot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "FootFrontL",
                    localPos = new Vector3(1.01201F, 0.32896F, -0.02394F),
                    localAngles = new Vector3(2.83203F, 0.24956F, 1.70784F),
                    localScale = new Vector3(0.37948F, 0.1509F, 0.30983F)
                }
            });
            rules.Add("mdlLoader", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MechHandR",
                    localPos = new Vector3(0.04684F, 0.20022F, 0.11708F),
                    localAngles = new Vector3(352.6463F, 217.5504F, 357.871F),
                    localScale = new Vector3(0.03105F, 0.03143F, 0.02519F)
                },
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MechHandR",
                    localPos = new Vector3(-0.008F, 0.1977F, 0.15883F),
                    localAngles = new Vector3(352.6463F, 217.5504F, 357.871F),
                    localScale = new Vector3(0.03105F, 0.03143F, 0.02519F)
                },
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MechHandR",
                    localPos = new Vector3(-0.092F, 0.19681F, 0.19888F),
                    localAngles = new Vector3(352.6463F, 217.5504F, 357.871F),
                    localScale = new Vector3(0.03105F, 0.03143F, 0.02519F)
                }
            });
            rules.Add("mdlCroco", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "SpineChest2",
                    localPos = new Vector3(1.25818F, 0.22855F, 1.46486F),
                    localAngles = new Vector3(275.4225F, 170.3384F, 217.8794F),
                    localScale = new Vector3(0.47896F, 0.45861F, 0.45097F)
                }
            });
            rules.Add("mdlCaptain", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(0.21691F, -0.12037F, -0.22717F),
                    localAngles = new Vector3(19.96512F, 15.38571F, 341.2212F),
                    localScale = new Vector3(0.04729F, 0.04729F, 0.04729F)
                }
            });
            rules.Add("mdlBandit2", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Hat",
                    localPos = new Vector3(0.11191F, -0.00771F, 0.00755F),
                    localAngles = new Vector3(357.2404F, 89.84798F, 267.9087F),
                    localScale = new Vector3(0.02096F, 0.02508F, 0.02316F)
                }
            });
            rules.Add("mdlEngiTurret", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Muzzle",
                    localPos = new Vector3(0F, 0.47463F, -2.31789F),
                    localAngles = new Vector3(0F, 90F, 90F),
                    localScale = new Vector3(0.17248F, 0.20638F, 0.19058F)
                }
            });
            rules.Add("mdlScav", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "LowerArmL",
                    localPos = new Vector3(-0.95572F, 1.57584F, -0.80645F),
                    localAngles = new Vector3(346.3986F, 301.0652F, 230.3974F),
                    localScale = new Vector3(1.35F, 1.8F, 1.35F)
                }
            });
            rules.Add("mdlRailGunner", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0.08158F, 0.364F, 0.04397F),
                    localAngles = new Vector3(357.7899F, 260.6965F, 270.743F),
                    localScale = new Vector3(0.16226F, 0.16226F, 0.16226F)
                }
            });
            rules.Add("mdlVoidSurvivor", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0.08158F, 0.364F, 0.04397F),
                    localAngles = new Vector3(357.7899F, 260.6965F, 270.743F),
                    localScale = new Vector3(0.16226F, 0.16226F, 0.16226F)
                }
            });*/
            return rules;
        }

        public override void Hooks() {
            On.RoR2.GlobalEventManager.OnHitEnemy += applyHaemorrhage;
        }
        private void applyHaemorrhage(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, global::RoR2.GlobalEventManager self, global::RoR2.DamageInfo damageInfo, GameObject victim) {
            if (NetworkServer.active) {
                if (damageInfo.attacker && victim) {
                    if (damageInfo.attacker.GetComponent<CharacterBody>() && victim.GetComponent<CharacterBody>()) {
                        var count = GetCount(damageInfo.attacker.GetComponent<CharacterBody>());
                        if (count > 0) {
                            if (victim.GetComponent<CharacterBody>().GetBuffCount(BuffCatalog.FindBuffIndex("bdBleeding")) >= Mathf.Max((bleedStackBase - (count - 1) * stackDegen), 0)) {
                                DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.SuperBleed, 15f);
                            }
                        }
                    }
                }
			}
            orig(self, damageInfo, victim);
        }

    }
}