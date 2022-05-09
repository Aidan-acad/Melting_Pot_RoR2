using BepInEx.Configuration;
using MeltingPot.Utils;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;


namespace MeltingPot.Items
{
    public class GlassShield : ItemBase<GlassShield>
    {
        public static float triggerThreshold = 0.25f;

        public override string ItemName => "Glass Shield";
        public override string ItemLangTokenName => "GLASSSHIELD";

        public override string ItemPickupDesc => $"<style=cIsHealing>Blocks</style> the next lethal hit, then <style=cUtility>shatters</style>.";

        public override string ItemFullDescription =>
            $"<style=cIsHealing>Blocks</style> the next lethal hit, then <style=cUtility>shatters</style>.";

        public override string ItemLore =>
            "Something is printed on the underside.\n\n"
            + "\"Sustainable, eco-friendly armor, for those in need of ablation.\n\nJust sweep away when done.\"";
        public static BepInEx.Logging.ManualLogSource BSModLogger;

        public override string VoidCounterpart => null;
        public GameObject ItemModel;

        public static GameObject ItemBodyModelPrefab;
        public static GameObject GlassEffect;
        private static ItemIndex ConsumedShieldIndex =>
            ContentPackProvider.contentPack.itemDefs.Find("ConsumedGlassShield_ItemDef").itemIndex;

        private void AddConsumeLang()
        {
            LanguageAPI.Add(
                "MeltingPot_ITEM_CONSUMED" + ItemLangTokenName + "_NAME",
                "Shattered Shield"
            );
            LanguageAPI.Add(
                "MeltingPot_ITEM_CONSUMED" + ItemLangTokenName + "_PICKUP",
                "A life saved."
            );
            LanguageAPI.Add(
                "MeltingPot_ITEM_CONSUMED" + ItemLangTokenName + "_DESC",
                "Shattered and unusable."
            );
            LanguageAPI.Add(
                "MeltingPot_ITEM_CONSUMED" + ItemLangTokenName + "_LORE",
                "Better get sweeping!"
            );
        }

        public override void Init(ConfigFile config, bool enabled)
        {
            CreateItem("GlassShield_ItemDef", enabled);
            if (enabled)
            {
                ItemModel = Assets.mainAssetBundle.LoadAsset<GameObject>(
                    $"{ModelPath}/glass_shield/GlassShield.prefab"
                );
                CreateLang();
                AddConsumeLang();
                CreateEffect();
                Hooks();
            }
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>(
                $"{ModelPath}/glass_shield/displayGlassShield.prefab"
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
                        childName = "Head",
                        localPos = new Vector3(-0.07409F, 0.29233F, 0.14135F),
                        localAngles = new Vector3(330.2333F, 328.2883F, 12.3869F),
                        localScale = new Vector3(0.15709F, 0.15709F, 0.15709F)
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
                        childName = "BowHinge2L",
                        localPos = new Vector3(-0.04748F, 0.3491F, 0.00022F),
                        localAngles = new Vector3(354.7574F, 264.3474F, 0.36243F),
                        localScale = new Vector3(0.10543F, 0.10543F, 0.10543F)
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
                        childName = "HeadCenter",
                        localPos = new Vector3(-0.97854F, 1.78231F, -0.50568F),
                        localAngles = new Vector3(306.1261F, 180F, 180F),
                        localScale = new Vector3(1.53385F, 1.53385F, 1.53385F)
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
                        childName = "UpperArmR",
                        localPos = new Vector3(-0.09657F, 0.05082F, -0.05364F),
                        localAngles = new Vector3(353.3935F, 254.4767F, 0F),
                        localScale = new Vector3(0.22708F, 0.22708F, 0.22708F)
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
                        childName = "FootL",
                        localPos = new Vector3(-0.00446F, 0.14667F, 0.10535F),
                        localAngles = new Vector3(37.34278F, 183.3341F, 187.1953F),
                        localScale = new Vector3(0.44928F, 0.44928F, 0.44928F)
                    },
                    new RoR2.ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemBodyModelPrefab,
                        childName = "FootR",
                        localPos = new Vector3(-0.00446F, 0.14667F, 0.10535F),
                        localAngles = new Vector3(37.34278F, 183.3341F, 187.1953F),
                        localScale = new Vector3(0.44928F, 0.44928F, 0.44928F)
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
                        childName = "HandR",
                        localPos = new Vector3(0.00675F, 0.18433F, 0.09622F),
                        localAngles = new Vector3(349.0229F, 0F, 0F),
                        localScale = new Vector3(0.34188F, 0.34188F, 0.34188F)
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
                        localPos = new Vector3(0.00022F, 0.99952F, -0.04007F),
                        localAngles = new Vector3(279.4944F, 180F, 180F),
                        localScale = new Vector3(1F, 1F, 1F)
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
                        childName = "LowerArmL",
                        localPos = new Vector3(-0.07675F, 0.13288F, 0.0281F),
                        localAngles = new Vector3(0F, 283.181F, 0F),
                        localScale = new Vector3(0.26725F, 0.26725F, 0.26725F)
                    },
                    new RoR2.ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemBodyModelPrefab,
                        childName = "LowerArmR",
                        localPos = new Vector3(0.06686F, 0.13311F, 0.02449F),
                        localAngles = new Vector3(1.03995F, 81.24062F, 179.5811F),
                        localScale = new Vector3(0.26725F, 0.26725F, 0.26725F)
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
                        childName = "ClavicleR",
                        localPos = new Vector3(-0.09817F, 4.30673F, 1.32012F),
                        localAngles = new Vector3(327.9636F, 0F, 0F),
                        localScale = new Vector3(3.06854F, 3.06854F, 3.06854F)
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
                        childName = "HandR",
                        localPos = new Vector3(-0.11999F, 0.10207F, -0.00917F),
                        localAngles = new Vector3(0F, 267.2908F, 0F),
                        localScale = new Vector3(0.33909F, 0.33909F, 0.33909F)
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
                        childName = "CalfR",
                        localPos = new Vector3(0.01893F, -0.00802F, -0.07664F),
                        localAngles = new Vector3(1.83612F, 173.9581F, 179.8057F),
                        localScale = new Vector3(0.26099F, 0.26099F, 0.26099F)
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
                        childName = "Muzzle",
                        localPos = new Vector3(0F, 0.07968F, 0.18019F),
                        localAngles = new Vector3(0F, 0F, 0F),
                        localScale = new Vector3(1.99791F, 1.99791F, 1.99791F)
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
                        childName = "Head",
                        localPos = new Vector3(-5.43913F, 3.966F, -0.13567F),
                        localAngles = new Vector3(298.3723F, 235.688F, 9.20145F),
                        localScale = new Vector3(4.28934F, 4.28934F, 4.28934F)
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
                        childName = "LowerArmR",
                        localPos = new Vector3(-0.00983F, -0.03352F, -0.00602F),
                        localAngles = new Vector3(48.41741F, 202.1044F, 196.8987F),
                        localScale = new Vector3(0.20419F, 0.20419F, 0.20419F)
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
                        localPos = new Vector3(0.01128F, 0.22631F, 0.11565F),
                        localAngles = new Vector3(301.5562F, 0F, 0F),
                        localScale = new Vector3(0.32879F, 0.32879F, 0.32879F)
                    }
                }
            );
            return rules;
        }

        public override void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += NegateAndConsume;
        }

        private void ExchangeItem(CharacterBody targetBody)
        {
            targetBody.inventory.RemoveItem(ItemDef.itemIndex);
            targetBody.inventory.GiveItem(ConsumedShieldIndex);
        }
        public void CreateEffect() {
            GlassEffect = PrefabAPI.InstantiateClone(
                LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BearVoidProc"),
                "GlassShieldProc"
            );
            var effectComponent = GlassEffect.GetComponent<EffectComponent>();
            effectComponent.soundName = "ShieldBreakPlay";

            GlassEffect.AddComponent<NetworkIdentity>();

            if (GlassEffect) {
                PrefabAPI.RegisterNetworkPrefab(GlassEffect);
            }
            ContentAddition.AddEffect(GlassEffect);
        }
        private void NegateAndConsume(
            On.RoR2.HealthComponent.orig_TakeDamage orig,
            RoR2.HealthComponent self,
            RoR2.DamageInfo damageInfo
        )
        {
            if (damageInfo.damage > self.combinedHealth * triggerThreshold)
            {
                var InventoryCount = GetCount(self.body);
                if (InventoryCount > 0 && damageInfo.damage > self.combinedHealth)
                {
                    damageInfo.rejected = true;
                    EffectManager.SpawnEffect(GlassEffect, new EffectData { origin = self.body.transform.position, scale = 1f, }, true);
                    //Util.PlaySound("ShieldBreakPlay", self.body.gameObject);
                    ExchangeItem(self.body);
                }
            }
            orig(self, damageInfo);
        }
    }
}
