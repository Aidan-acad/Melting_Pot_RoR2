using BepInEx.Configuration;
using MeltingPot.Utils;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace MeltingPot.Items
{
    public class SappingBloom : ItemBase<SappingBloom>
    {
        public static float weakenChance = 0.1f;
        public override string ItemName => "Sapping Bloom";
        public override string ItemLangTokenName => "SAPPINGBLOOM";
        public override string ItemPickupDesc =>
            $"Chance to apply <style=cIsHealing>Weaken</style> on hit";
        public override string ItemFullDescription =>
            $"<style=cIsHealing>{weakenChance * 100}%</style> <style=cStack>(+{weakenChance * 100}</style> per stack, stacking hyperbolically) of applying <style=cIsHealing>weaken</style> on hit";
        public override string ItemLore =>
            "[A small note attached to the bundle]\n\n"
            + "OBJECT: Organic Clipping, ORIGIN: Subject `REX` \n\nRESEARCHERS NOTE: A strange fusion of organic and mechanical parts, Subject REX is capable of weakening all who come in contact with it's pollen. \n\n We have successfully obtained a cutting of REX's floral growths, though Technician #68 has yet to awaken after the operation.";

        public static BepInEx.Logging.ManualLogSource BSModLogger;

        public static GameObject ItemModel;

        public override string VoidCounterpart => null;

        public static GameObject ItemBodyModelPrefab;

        public override void Init(ConfigFile config, bool enabled)
        {
            CreateItem("SappingBloom_ItemDef", enabled);
            if (enabled)
            {
                ItemModel = Assets.mainAssetBundle.LoadAsset<GameObject>(
                    $"{ModelPath}/sapping_bloom/sappingbloom.prefab"
                );
                CreateLang();
                Hooks();
            }
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>(
                $"{ModelPath}/sapping_bloom/displaysappingbloom.prefab"
            );
            Vector3 generalScale = new Vector3(1f, 1f, 1f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
            rules.Add(
                "mdlCommandoDualies",
                new RoR2.ItemDisplayRule[]
                {
                    new RoR2.ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemBodyModelPrefab,
                        childName = "Chest",
                        localPos = new Vector3(-0.2173F, 0.37225F, 0.01727F),
                        localAngles = new Vector3(5.25706F, 262.131F, 17.43484F),
                        localScale = new Vector3(0.03F, 0.03F, 0.03F)
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
                        childName = "HeadCenter",
                        localPos = new Vector3(0.08288F, 0.0779F, -0.12605F),
                        localAngles = new Vector3(15.02269F, 255.3352F, 66.15241F),
                        localScale = new Vector3(0.0442F, 0.04967F, 0.04712F)
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
                        childName = "HandR",
                        localPos = new Vector3(-0.03729F, 0.79903F, 0.03344F),
                        localAngles = new Vector3(86.41363F, 131.8821F, 124.6506F),
                        localScale = new Vector3(0.72109F, 0.72109F, 0.72109F)
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
                        childName = "WristDisplay",
                        localPos = new Vector3(-0.0667F, -0.00665F, -0.0137F),
                        localAngles = new Vector3(275.7034F, 246.841F, 293.262F),
                        localScale = new Vector3(0.03554F, 0.02965F, 0.03831F)
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
                        childName = "Chest",
                        localPos = new Vector3(0.00291F, 0.26155F, -0.16848F),
                        localAngles = new Vector3(0F, 275.5362F, 0F),
                        localScale = new Vector3(0.05801F, 0.05526F, 0.06258F)
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
                        childName = "UpperArmR",
                        localPos = new Vector3(-0.11578F, -0.00002F, -0.00001F),
                        localAngles = new Vector3(354.0546F, 180F, 180F),
                        localScale = new Vector3(0.04033F, 0.04109F, 0.05574F)
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
                        childName = "FlowerBase",
                        localPos = new Vector3(0.60067F, 0.70101F, 0.14027F),
                        localAngles = new Vector3(356.7191F, 156.3135F, 4.03449F),
                        localScale = new Vector3(0.1481F, 0.1481F, 0.1481F)
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
                        childName = "Stomach",
                        localPos = new Vector3(0.19558F, 0.21344F, -0.04659F),
                        localAngles = new Vector3(354.6892F, 155.1198F, 5.51975F),
                        localScale = new Vector3(0.03105F, 0.03143F, 0.02519F)
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
                        childName = "LowerArmL",
                        localPos = new Vector3(-0.83194F, 2.7323F, 0.10009F),
                        localAngles = new Vector3(5.89639F, 164.3329F, 214.7509F),
                        localScale = new Vector3(0.47896F, 0.45861F, 0.45097F)
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
                        localPos = new Vector3(-0.17689F, 0.12234F, -0.13224F),
                        localAngles = new Vector3(336.4875F, 79.99247F, 354.2115F),
                        localScale = new Vector3(0.04729F, 0.04729F, 0.04729F)
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
                        childName = "Pelvis",
                        localPos = new Vector3(-0.02105F, -0.10069F, 0.11409F),
                        localAngles = new Vector3(2.34426F, 278.7445F, 202.4919F),
                        localScale = new Vector3(0.02096F, 0.02508F, 0.02316F)
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
                        localPos = new Vector3(-0.76816F, 0.79213F, 0F),
                        localAngles = new Vector3(0F, 0F, 0F),
                        localScale = new Vector3(0.04302F, 0.05148F, 0.04754F)
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
                        childName = "LowerArmL",
                        localPos = new Vector3(0.2513F, 5.53174F, -0.65762F),
                        localAngles = new Vector3(51.32129F, 209.6494F, 187.5173F),
                        localScale = new Vector3(1.35F, 1.8F, 1.35F)
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
                        childName = "FootL",
                        localPos = new Vector3(-0.00886F, -0.04126F, -0.02112F),
                        localAngles = new Vector3(332.8833F, 180F, 192.8111F),
                        localScale = new Vector3(0.04949F, 0.04949F, 0.04949F)
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
                        childName = "ShoulderL",
                        localPos = new Vector3(0.00268F, 0.14242F, 0.08598F),
                        localAngles = new Vector3(357.7899F, 260.6965F, 270.743F),
                        localScale = new Vector3(0.04438F, 0.04438F, 0.04438F)
                    }
                }
            );
            return rules;
        }

        public override void Hooks()
        {
            On.RoR2.GlobalEventManager.OnHitEnemy += applyweaken;
        }

        private void applyweaken(
            On.RoR2.GlobalEventManager.orig_OnHitEnemy orig,
            global::RoR2.GlobalEventManager self,
            global::RoR2.DamageInfo damageInfo,
            GameObject victim
        )
        {
            try
            {
                if (NetworkServer.active)
                {
                    if (
                        damageInfo.attacker.GetComponent<CharacterBody>()
                        && victim.GetComponent<CharacterBody>()
                    )
                    {
                        var count = GetCount(damageInfo.attacker.GetComponent<CharacterBody>());
                        if (count > 0)
                        {
                            if (
                                Util.CheckRoll(
                                    (1 - Mathf.Clamp(1 / (1 + (count * weakenChance)), 0, 1))
                                        * 100f
                                        * damageInfo.procCoefficient
                                )
                            )
                            {
                                damageInfo.damageType |= DamageType.WeakOnHit;
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
