﻿using BepInEx.Configuration;
using MeltingPot.Utils;
using R2API;
using RoR2;
using System.Linq;
using UnityEngine;

namespace MeltingPot.Items
{
    public class SwirlingPotion : ItemBase<SwirlingPotion>
    {
        public static float buffChance = 0.025f;
        public static float durationGrowth = 0.5f;
        public override string ItemName => "Swirling Potion";
        public override string ItemLangTokenName => "SWIRLINGPOTION";

        public override string ItemPickupDesc =>
            $"Chance on receiving buff to receive another random temporary buff";

        public override string ItemFullDescription =>
            $"On receiving a buff <style=cIsUtility>{buffChance*100}%</style> <style=cStack>(+{buffChance*100}% stacking hyperbolically)</style> chance to receive another <style=cEvent>randomly selected</style> buff for <style=cIsUtility>{durationGrowth*2}</style> <style=cStack>(+ {durationGrowth} per stack)</style> seconds";

        public override string ItemLore =>
            "[Left inside]\n\n"
            + "The inside is stained lightly from extensive use, mostly water, but who really knows?\n\nIt's a bucket";

        public override string VoidCounterpart => null;
        public GameObject ItemModel;

        public static GameObject ItemBodyModelPrefab;
        public static BuffDef[] buffCatalog;
        private static System.Random idGen;

        public override void Init(ConfigFile config, bool enabled)
        {
            CreateItem("SwirlingPotion_ItemDef", enabled);
            if (enabled)
            {
                ItemModel = Assets.mainAssetBundle.LoadAsset<GameObject>(
                    $"{ModelPath}/swirling_potion/swirlingpotion.prefab"
                );
                CreateLang();
                Hooks();
            }
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>(
                $"{ModelPath}/just_bucket/displayjustbucket.prefab"
            );
            Vector3 generalScale = new Vector3(1f, 1f, 1f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
            return rules;
            rules.Add(
                "mdlCommandoDualies",
                new RoR2.ItemDisplayRule[]
                {
                    new RoR2.ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemBodyModelPrefab,
                        childName = "Head",
                        localPos = new Vector3(0.02823F, 0.28457F, -0.04327F),
                        localAngles = new Vector3(347.428F, 5.31903F, 167.7263F),
                        localScale = new Vector3(0.5458F, 0.5458F, 0.5458F)
                    }
                }
            );
            rules.Add(
                "mdlHuntress",
                new RoR2.ItemDisplayRule[]
                {
                    new RoR2.ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemBodyModelPrefab,
                        childName = "Head",
                        localPos = new Vector3(0.09035F, 0.30118F, -0.17646F),
                        localAngles = new Vector3(325.5183F, 11.308F, 149.5922F),
                        localScale = new Vector3(0.27296F, 0.27296F, 0.27296F)
                    }
                }
            );
            rules.Add(
                "mdlToolbot",
                new RoR2.ItemDisplayRule[]
                {
                    new RoR2.ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemBodyModelPrefab,
                        childName = "Neck",
                        localPos = new Vector3(-1.925F, 1.55704F, -1.83531F),
                        localAngles = new Vector3(0F, 0F, 0F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add(
                "mdlEngi",
                new RoR2.ItemDisplayRule[]
                {
                    new RoR2.ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemBodyModelPrefab,
                        childName = "HeadCenter",
                        localPos = new Vector3(-0.08227F, 0.22103F, -0.21335F),
                        localAngles = new Vector3(20.18572F, 358.2665F, 179.9301F),
                        localScale = new Vector3(0.14738F, 0.14738F, 0.14738F)
                    }
                }
            );
            rules.Add(
                "mdlMage",
                new RoR2.ItemDisplayRule[]
                {
                    new RoR2.ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemBodyModelPrefab,
                        childName = "Head",
                        localPos = new Vector3(-0.01007F, 0.21082F, -0.09254F),
                        localAngles = new Vector3(354.5292F, 1.63182F, 185.2126F),
                        localScale = new Vector3(0.25139F, 0.25139F, 0.25139F)
                    }
                }
            );
            rules.Add(
                "mdlMerc",
                new RoR2.ItemDisplayRule[]
                {
                    new RoR2.ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemBodyModelPrefab,
                        childName = "Head",
                        localPos = new Vector3(0.07967F, 0.25218F, 0.04636F),
                        localAngles = new Vector3(323.5562F, 263.9417F, 169.6504F),
                        localScale = new Vector3(0.22611F, 0.22611F, 0.22611F)
                    }
                }
            );
            rules.Add(
                "mdlTreebot",
                new RoR2.ItemDisplayRule[]
                {
                    new RoR2.ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemBodyModelPrefab,
                        childName = "PlatformBase",
                        localPos = new Vector3(0.8597F, 2.57321F, -0.34448F),
                        localAngles = new Vector3(14.31126F, 104.2635F, 3.59551F),
                        localScale = new Vector3(0.21396F, 0.21396F, 0.21396F)
                    }
                }
            );
            rules.Add(
                "mdlLoader",
                new RoR2.ItemDisplayRule[]
                {
                    new RoR2.ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemBodyModelPrefab,
                        childName = "FootL",
                        localPos = new Vector3(-0.01514F, -0.00521F, 0.00453F),
                        localAngles = new Vector3(327.554F, 128.2742F, 208.1566F),
                        localScale = new Vector3(0.72437F, 0.72437F, 0.72437F)
                    }
                }
            );
            rules.Add(
                "mdlCroco",
                new RoR2.ItemDisplayRule[]
                {
                    new RoR2.ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemBodyModelPrefab,
                        childName = "MouthMuzzle",
                        localPos = new Vector3(-0.00319F, -0.27435F, -0.01359F),
                        localAngles = new Vector3(313.655F, 175.0468F, 2.53212F),
                        localScale = new Vector3(5.57909F, 5.57909F, 5.57909F)
                    }
                }
            );
            rules.Add(
                "mdlCaptain",
                new RoR2.ItemDisplayRule[]
                {
                    new RoR2.ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemBodyModelPrefab,
                        childName = "Head",
                        localPos = new Vector3(0.12628F, 0.06212F, -0.01986F),
                        localAngles = new Vector3(28.20466F, 95.05783F, 341.879F),
                        localScale = new Vector3(0.07361F, 0.07361F, 0.07361F)
                    }
                }
            );
            rules.Add(
                "mdlBandit2",
                new RoR2.ItemDisplayRule[]
                {
                    new RoR2.ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemBodyModelPrefab,
                        childName = "FootL",
                        localPos = new Vector3(-0.0004F, 0.02846F, -0.04664F),
                        localAngles = new Vector3(313.5162F, 181.5179F, 180.9492F),
                        localScale = new Vector3(0.62782F, 0.62782F, 0.62782F)
                    }
                }
            );
            rules.Add(
                "mdlRailGunner",
                new RoR2.ItemDisplayRule[]
                {
                    new RoR2.ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemBodyModelPrefab,
                        childName = "Backpack",
                        localPos = new Vector3(-0.17597F, 0.43614F, 0.04376F),
                        localAngles = new Vector3(359.2964F, 35.50795F, 357.1917F),
                        localScale = new Vector3(0.14836F, 0.14836F, 0.14836F)
                    }
                }
            );
            rules.Add(
                "mdlVoidSurvivor",
                new RoR2.ItemDisplayRule[]
                {
                    new RoR2.ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemBodyModelPrefab,
                        childName = "Head",
                        localPos = new Vector3(-0.16807F, 0.13819F, -0.06966F),
                        localAngles = new Vector3(325.9055F, 127.957F, 126.0954F),
                        localScale = new Vector3(0.26691F, 0.26691F, 0.26691F)
                    }
                }
            );
            rules.Add(
                "mdlEngiTurret",
                new RoR2.ItemDisplayRule[]
                {
                    new RoR2.ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemBodyModelPrefab,
                        childName = "Head",
                        localPos = new Vector3(0F, 0.83468F, -0.02207F),
                        localAngles = new Vector3(0F, 0F, 0F),
                        localScale = new Vector3(1.54859F, 1.54859F, 1.54859F)
                    }
                }
            );
            rules.Add(
                "mdlScav",
                new RoR2.ItemDisplayRule[]
                {
                    new RoR2.ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemBodyModelPrefab,
                        childName = "FootR",
                        localPos = new Vector3(-0.05243F, -0.02199F, -0.63673F),
                        localAngles = new Vector3(352.546F, 265.745F, 175.5882F),
                        localScale = new Vector3(3.19819F, 3.19819F, 3.19819F)
                    }
                }
            );
            return rules;
        }

        public override void Hooks()
        {
            On.RoR2.CharacterBody.AddBuff_BuffIndex += CompoundBuffs;
            On.RoR2.Run.Awake += PopulateBuffCatalog;
        }

        private void PopulateBuffCatalog(On.RoR2.Run.orig_Awake orig, global::RoR2.Run self) {
			try {
                buffCatalog = RoR2.BuffCatalog.buffDefs.Where(buff => !buff.isDebuff && !DotController.dotDefs.Any(dDef => dDef.associatedBuff == buff) && !buff.isHidden && !buff.isCooldown).ToArray();
                idGen = new System.Random();
			}
			catch {
                MeltingPotPlugin.ModLogger.LogError("Couldn't populate BuffCatalog!");
			}
            
            orig(self);
        }

        private void CompoundBuffs(On.RoR2.CharacterBody.orig_AddBuff_BuffDef orig, global::RoR2.CharacterBody self, global::RoR2.BuffDef buffDef) {
            orig(self, buffDef);
            ApplyBuffs(self, buffDef);
		}

        private void CompoundBuffs(On.RoR2.CharacterBody.orig_AddBuff_BuffIndex orig, global::RoR2.CharacterBody self, global::RoR2.BuffIndex buffIndex) {
            orig(self, buffIndex);
            BuffDef buffDef = BuffCatalog.buffDefs[(int)buffIndex];
            if (buffDef) {
                ApplyBuffs(self, buffDef);
			}
        }

        private void ApplyBuffs(global::RoR2.CharacterBody self, global::RoR2.BuffDef buffDef) {
            int count = GetCount(self);
            if (self && buffCatalog.Contains(buffDef) && count > 0) {

                if (
                    Util.CheckRoll(
                        (1 - Mathf.Clamp(1 / (1 + (count * buffChance)), 0, 1))
                            * 100f
                    )
                ) {
                    // Pick random debuff
                    int id = idGen.Next(buffCatalog.Length);
                    BuffDef nextBuff = buffCatalog[id];
                    self.AddTimedBuff(nextBuff, durationGrowth * (count + 1));
                }
            }
        }
    }
}
