using BepInEx.Configuration;
using MeltingPot.Utils;
using R2API;
using RoR2;
using UnityEngine;
using static R2API.RecalculateStatsAPI;

namespace MeltingPot.Items
{
    public class FluffyManacles : ItemBase<FluffyManacles>
    {
        public static float armourGrowth = 3f;
        public static float moveGrowth = 0.025f;
        public override string ItemName => "Fluffy Manacles";
        public override string ItemLangTokenName => "FLUFFYMANACLES";
        public override string ItemPickupDesc =>
            $"Increases <style=cIsUtility>Armour</style>. Increases <style=cIsUtility>Speed</style>";
        public override string ItemFullDescription =>
            $"Increase <style=cIsUtility>Armour</style> by <style=cIsUtility>{armourGrowth}</style> <style=cStack>(+{armourGrowth} per stack)</style>. Increases <style=cIsUtility>Movespeed</style> by <style=cIsUtility>{moveGrowth * 100}%</style> <style=cStack>(+{moveGrowth * 100}% per stack)</style>";
        public override string ItemLore =>
            "[On the included instruction manual]\n\n"
            + "Comfortable and stylish, your **VOiD TM** solution for the combatant on the go ;)";
        public override string VoidCounterpart => "LeadFetters_ItemDef";

        public static GameObject ItemModel;
        public static GameObject ItemBodyModelPrefab;

        public override void Init(ConfigFile config, bool enabled)
        {
            CreateItem("FluffyManacles_ItemDef", enabled);
            if (enabled)
            {
                ItemModel = Assets.mainAssetBundle.LoadAsset<GameObject>(
                    $"{ModelPath}/fluffy_manacles/fluffymanacles.prefab"
                );
                ItemDef.requiredExpansion = MeltingPotPlugin.sotvRef;
                CreateLang();
                Hooks();
            }
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>(
                $"{ModelPath}/fluffy_manacles/displayfluffymanacles.prefab"
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
                        childName = "HandL",
                        localPos = new Vector3(0.10177F, -0.01659F, 0.0313F),
                        localAngles = new Vector3(11.6928F, 70.66875F, 13.80813F),
                        localScale = new Vector3(0.41208F, 0.41208F, 0.41208F)
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
                        childName = "CalfL",
                        localPos = new Vector3(-0.00586F, 0.5283F, 0.11241F),
                        localAngles = new Vector3(0F, 0F, 0F),
                        localScale = new Vector3(0.39482F, 0.39482F, 0.39482F)
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
                        childName = "LowerArmR",
                        localPos = new Vector3(0.02425F, 0.13954F, 0.13727F),
                        localAngles = new Vector3(9.32772F, 1.19292F, 359.4304F),
                        localScale = new Vector3(0.45736F, 0.45736F, 0.45736F)
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
                        childName = "Hip",
                        localPos = new Vector3(0.94959F, 2.90445F, 1.0189F),
                        localAngles = new Vector3(303.6498F, 0F, 0.00003F),
                        localScale = new Vector3(2.21552F, 2.21552F, 2.21552F)
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
                        childName = "Chest",
                        localPos = new Vector3(0.00221F, -0.15329F, -0.33745F),
                        localAngles = new Vector3(80.06202F, 186.1448F, 61.70652F),
                        localScale = new Vector3(0.35105F, 0.35105F, 0.35105F)
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
                        childName = "FootR",
                        localPos = new Vector3(0.09523F, -0.03076F, 0.09144F),
                        localAngles = new Vector3(2.57902F, 257.8782F, 192.3368F),
                        localScale = new Vector3(0.36728F, 0.36728F, 0.36728F)
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
                        localPos = new Vector3(-0.17674F, 0.42214F, 0.02984F),
                        localAngles = new Vector3(285.0966F, 8.15981F, 33.9588F),
                        localScale = new Vector3(0.34338F, 0.34338F, 0.34338F)
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
                        childName = "Base",
                        localPos = new Vector3(1.0597F, 0.40656F, 0.40148F),
                        localAngles = new Vector3(0F, 0F, 0F),
                        localScale = new Vector3(0.41689F, 0.41689F, 0.41689F)
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
                        childName = "ThighR",
                        localPos = new Vector3(-0.04278F, 0.50744F, 0.21864F),
                        localAngles = new Vector3(62.29462F, 265.2826F, 271.6754F),
                        localScale = new Vector3(0.40203F, 0.40203F, 0.40203F)
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
                        childName = "HandR",
                        localPos = new Vector3(-2.83763F, 0.58103F, 1.01706F),
                        localAngles = new Vector3(31.88393F, 52.11344F, 325.2458F),
                        localScale = new Vector3(4.68035F, 4.68035F, 4.68035F)
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
                        childName = "Pelvis",
                        localPos = new Vector3(0.13406F, -0.08647F, -0.16964F),
                        localAngles = new Vector3(54.59187F, 263.7668F, 101.0942F),
                        localScale = new Vector3(0.31492F, 0.31492F, 0.31492F)
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
                        localPos = new Vector3(-1.06741F, -0.00019F, 0.16832F),
                        localAngles = new Vector3(20.97257F, 293.0256F, 188.6494F),
                        localScale = new Vector3(1.65752F, 1.65752F, 1.65752F)
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
                        childName = "HandR",
                        localPos = new Vector3(-0.11409F, -1.75549F, 0.61701F),
                        localAngles = new Vector3(64.82258F, 2.07888F, 1.93165F),
                        localScale = new Vector3(6.84151F, 6.84151F, 6.84151F)
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
                        childName = "GunRoot",
                        localPos = new Vector3(-0.06007F, -0.02847F, -0.37051F),
                        localAngles = new Vector3(-0.00002F, 0F, 339.3182F),
                        localScale = new Vector3(0.30171F, 0.30171F, 0.30171F)
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
                        childName = "FootR",
                        localPos = new Vector3(0.07097F, -0.02897F, -0.13048F),
                        localAngles = new Vector3(16.17254F, 150.6366F, 225.7414F),
                        localScale = new Vector3(0.61803F, 0.61803F, 0.61803F)
                    }
                }
            );
            return rules;
        }

        public override void Hooks()
        {
            GetStatCoefficients += GrantStats;
        }

        private void GrantStats(CharacterBody sender, StatHookEventArgs args)
        {
            var count = GetCount(sender);
            if (count > 0)
            {
                args.armorAdd += armourGrowth * count;
                args.baseMoveSpeedAdd += moveGrowth * count * sender.baseMoveSpeed;
            }
        }
    }
}
