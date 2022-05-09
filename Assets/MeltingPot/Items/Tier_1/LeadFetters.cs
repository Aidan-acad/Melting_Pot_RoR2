using BepInEx.Configuration;
using MeltingPot.Utils;
using R2API;
using RoR2;
using UnityEngine;
using static R2API.RecalculateStatsAPI;

namespace MeltingPot.Items
{
    public class LeadFetters : ItemBase<LeadFetters>
    {
        public static float armourGrowth = 5f;
        public static float knockbackGrowth = 0.025f;
        public override string ItemName => "Lead Fetters";
        public override string ItemLangTokenName => "LEADFETTERS";

        public override string ItemPickupDesc =>
            $"Increase <style=cIsHealing>armor</style> and <style=cIsUtility>knockback reduction</style>.";

        public override string ItemFullDescription =>
            $"Increase <style=cIsHealing>armor</style> by <style=cIsHealing>{armourGrowth}</style> <style=cStack>(+{armourGrowth} per stack)</style>. Increase <style=cIsUtility>knockback reduction</style> by <style=cIsUtility>{knockbackGrowth * 100}%</style> <style=cStack>(+{knockbackGrowth}%, hyperbolically)</style>.";

        public override string ItemLore =>
            "[Etched onto the left cuff:]\n\n"
            + "Mile after long mile, legs grow weary and numb, loop following loop, ";
        public static BepInEx.Logging.ManualLogSource BSModLogger;

        public override string VoidCounterpart => null;
        public GameObject ItemModel;

        public static GameObject ItemBodyModelPrefab;

        public override void Init(ConfigFile config, bool enabled)
        {
            CreateItem("LeadFetters_ItemDef", enabled);
            if (enabled)
            {
                ItemModel = Assets.mainAssetBundle.LoadAsset<GameObject>(
                    $"{ModelPath}/lead_fetters/leadfetters.prefab"
                );
                CreateLang();
                Hooks();
            }
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>(
                $"{ModelPath}/lead_fetters/displayleadfetters.prefab"
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
                        childName = "CalfL",
                        localPos = new Vector3(0.00108F, 0.23116F, 0.03563F),
                        localAngles = new Vector3(1.48092F, 358.2705F, 2.32109F),
                        localScale = new Vector3(0.09879F, 0.09879F, 0.09879F)
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
                        childName = "CalfR",
                        localPos = new Vector3(0.01043F, 0.41196F, 0.01137F),
                        localAngles = new Vector3(358.7941F, 284.9673F, 14.14106F),
                        localScale = new Vector3(0.06638F, 0.06638F, 0.06638F)
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
                        childName = "LowerArmL",
                        localPos = new Vector3(0.36124F, 3.55723F, -0.14019F),
                        localAngles = new Vector3(0.05903F, 20.67022F, 358.7044F),
                        localScale = new Vector3(1.10378F, 1.10378F, 1.10378F)
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
                        childName = "CalfL",
                        localPos = new Vector3(-0.00473F, 0.13726F, 0.03904F),
                        localAngles = new Vector3(5.10691F, 179.9999F, 179.9995F),
                        localScale = new Vector3(0.12376F, 0.10327F, 0.1334F)
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
                        childName = "Stomach",
                        localPos = new Vector3(-0.00977F, 0.15513F, 0.02408F),
                        localAngles = new Vector3(359.9597F, 97.04926F, 185.5686F),
                        localScale = new Vector3(0.13958F, 0.13297F, 0.15055F)
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
                        childName = "ThighL",
                        localPos = new Vector3(0.00675F, 0.38848F, 0.00711F),
                        localAngles = new Vector3(352.2655F, 358.6343F, 358.3787F),
                        localScale = new Vector3(0.12799F, 0.13041F, 0.17692F)
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
                        childName = "Eye",
                        localPos = new Vector3(0.00022F, 0.78632F, -0.00442F),
                        localAngles = new Vector3(356.3995F, 180F, 180F),
                        localScale = new Vector3(0.17713F, 0.17713F, 0.17713F)
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
                        childName = "CalfR",
                        localPos = new Vector3(0.01841F, 0.30791F, 0.02499F),
                        localAngles = new Vector3(352.6463F, 217.5504F, 9.9277F),
                        localScale = new Vector3(0.12377F, 0.12525F, 0.1004F)
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
                        childName = "Neck",
                        localPos = new Vector3(-0.05163F, 2.12813F, -2.7641F),
                        localAngles = new Vector3(275.4226F, 170.3383F, 10.3181F),
                        localScale = new Vector3(1.86637F, 1.23874F, 1.75728F)
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
                        childName = "LowerArmR",
                        localPos = new Vector3(-0.00552F, 0.23558F, -0.00374F),
                        localAngles = new Vector3(0F, 0F, 351.1129F),
                        localScale = new Vector3(0.10606F, 0.10606F, 0.10606F)
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
                        childName = "MuzzleShotgun",
                        localPos = new Vector3(-0.00034F, -0.00866F, -0.54691F),
                        localAngles = new Vector3(357.2404F, 89.84797F, 267.9087F),
                        localScale = new Vector3(0.05463F, 0.06541F, 0.06041F)
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
                        childName = "CalfR",
                        localPos = new Vector3(-0.00673F, 0.33044F, 0.02356F),
                        localAngles = new Vector3(8.98607F, 24.80293F, 188.8286F),
                        localScale = new Vector3(0.13081F, 0.13081F, 0.13081F)
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
                        childName = "ForeArmL",
                        localPos = new Vector3(0.01124F, 0.31023F, 0.00132F),
                        localAngles = new Vector3(0F, 0F, 0F),
                        localScale = new Vector3(0.06659F, 0.06659F, 0.06659F)
                    }
                }
            );
            return rules;
        }

        public override void Hooks()
        {
            //On.RoR2.CharacterBody.FixedUpdate += ApplyWeightModification;
            GetStatCoefficients += GrantArmour;
            On.RoR2.HealthComponent.TakeDamage += ReduceKnockback;
        }

        private void GrantArmour(CharacterBody sender, StatHookEventArgs args)
        {
            var count = GetCount(sender);
            if (count > 0)
            {
                args.armorAdd += armourGrowth * count;
            }
        }

        private void ReduceKnockback(
            On.RoR2.HealthComponent.orig_TakeDamage orig,
            RoR2.HealthComponent self,
            RoR2.DamageInfo damageInfo
        )
        {
            var InventoryCount = GetCount(self.body);
            if (InventoryCount > 0)
            {
                var percentReduction = Mathf.Clamp(
                    1 / (1 + (InventoryCount * knockbackGrowth)),
                    0,
                    1
                );
                damageInfo.force *= percentReduction;
            }
            orig(self, damageInfo);
        }

        /* private void ApplyWeightModification(On.RoR2.CharacterBody.orig_FixedUpdate orig, RoR2.CharacterBody self)
         {
             orig(self);
             if (!self.characterMotor) return;
             var InventoryCount = GetCount(self);
             if (InventoryCount > 0 && Math.Abs(self.characterMotor.velocity.y) > 0.001f)
             {
                 self.characterMotor.velocity.y += Time.deltaTime * Physics.gravity.y * Mathf.Clamp(1 - 1 / (1 + (0.001f * InventoryCount)), 0, 1);
             }
         }*/
    }
}
