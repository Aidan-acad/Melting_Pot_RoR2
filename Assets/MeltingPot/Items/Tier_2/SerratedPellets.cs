using BepInEx.Configuration;
using MeltingPot.Utils;
using R2API;
using RoR2;
using UnityEngine.Networking;
using UnityEngine;

namespace MeltingPot.Items
{
    public class SerratedPellets : ItemBase<SerratedPellets>
    {
        public static float bleedStackBase = 15f;
        public static float stackDegen = 3f;
        public static DotController.DotIndex eternalHaemoDot { get; private set; }
        public override string ItemName => "Serrated Pellets";
        public override string ItemLangTokenName => "SERRATEDPELLETS";
        public override string ItemPickupDesc =>
            $"Apply <style=cDeath>Permanent Haemorrhage</style> to heavily bleeding enemies";
        public override string ItemFullDescription =>
            $"Apply <style=cDeath>Permanent Haemorrhage</style> to enemies with <style=cIsUtility>{bleedStackBase}</style> <style=cStack>(- {stackDegen} per stack, min of 1)</style> bleed stacks";
        public override string ItemLore =>
            "[On the evidence bag]\n\n"
            + "Evidence found on site at homicide #867. Modifed ammunition designed for maximum internal damage.\n\nThe ammunition bears marks of amateur tooling, believed custom made by Suspect #2, a.k.a 'Desperate Outlaw'.";

        public static BepInEx.Logging.ManualLogSource BSModLogger;

        public static GameObject ItemModel;

        public override string VoidCounterpart => null;

        public static GameObject ItemBodyModelPrefab;

        public override void Init(ConfigFile config, bool enabled)
        {
            CreateItem("SerratedPellets_ItemDef", enabled);
            if (enabled)
            {
                ItemModel = Assets.mainAssetBundle.LoadAsset<GameObject>(
                    $"{ModelPath}/serrated_pellets/serratedpellets.prefab"
                );
                CreateLang();
                Hooks();
                CreateDoTs();
            }
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>(
                $"{ModelPath}/serrated_pellets/displayserratedpellets.prefab"
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
                        childName = "Base",
                        localPos = new Vector3(0.20272F, 0.12738F, -0.07128F),
                        localAngles = new Vector3(87.9534F, 290.8185F, 285.439F),
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
                        childName = "Pelvis",
                        localPos = new Vector3(-0.11802F, -0.13208F, -0.029F),
                        localAngles = new Vector3(359.2511F, 358.1559F, 315.8063F),
                        localScale = new Vector3(0.01663F, 0.01869F, 0.01773F)
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
                        childName = "Chest",
                        localPos = new Vector3(2.1521F, 2.43764F, 3.52967F),
                        localAngles = new Vector3(0F, 300.4704F, 0F),
                        localScale = new Vector3(0.21022F, 0.21022F, 0.21022F)
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
                        localPos = new Vector3(0.13442F, -0.14084F, 0.19087F),
                        localAngles = new Vector3(0F, 324.3133F, 337.8347F),
                        localScale = new Vector3(0.01815F, 0.01815F, 0.01815F)
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
                        childName = "LowerArmL",
                        localPos = new Vector3(0.03843F, 0.2324F, -0.0982F),
                        localAngles = new Vector3(0F, 0F, 88.35533F),
                        localScale = new Vector3(0.01767F, 0.01767F, 0.01767F)
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
                        childName = "Pelvis",
                        localPos = new Vector3(-0.13314F, -0.0207F, -0.09892F),
                        localAngles = new Vector3(0F, 57.55802F, 0F),
                        localScale = new Vector3(0.00947F, 0.00947F, 0.00947F)
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
                        childName = "MuzzleSyringe",
                        localPos = new Vector3(0.00392F, -0.06523F, 0.10526F),
                        localAngles = new Vector3(0.0119F, 271.5156F, 1.89173F),
                        localScale = new Vector3(0.03872F, 0.03923F, 0.03923F)
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
                        childName = "Chest",
                        localPos = new Vector3(-0.15838F, 0.34649F, 0.29866F),
                        localAngles = new Vector3(352.6463F, 217.5504F, 357.871F),
                        localScale = new Vector3(0.01527F, 0.01527F, 0.01527F)
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
                        childName = "SpineChest1",
                        localPos = new Vector3(2.18567F, 0.98864F, 0.86324F),
                        localAngles = new Vector3(30.2686F, 55.8666F, 98.72421F),
                        localScale = new Vector3(0.17421F, 0.16681F, 0.16403F)
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
                        localPos = new Vector3(-0.17376F, -0.19516F, -0.12771F),
                        localAngles = new Vector3(26.71924F, 49.32847F, 355.0345F),
                        localScale = new Vector3(0.0245F, 0.0245F, 0.0245F)
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
                        localPos = new Vector3(0.04402F, -0.03157F, -0.01616F),
                        localAngles = new Vector3(0F, 269.1563F, 0F),
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
                        localPos = new Vector3(0F, 1.05336F, -1.52765F),
                        localAngles = new Vector3(0F, 90F, 43.98207F),
                        localScale = new Vector3(0.05417F, 0.06482F, 0.05986F)
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
                        childName = "Backpack",
                        localPos = new Vector3(9.02812F, 9.00357F, -0.80617F),
                        localAngles = new Vector3(0F, 0F, 25.75339F),
                        localScale = new Vector3(0.33301F, 0.33301F, 0.33301F)
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
                        childName = "ThighR",
                        localPos = new Vector3(-0.10985F, -0.02754F, 0.01784F),
                        localAngles = new Vector3(344.9477F, 260.5255F, 270.7689F),
                        localScale = new Vector3(0.01283F, 0.01283F, 0.01283F)
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
                        childName = "Chest",
                        localPos = new Vector3(-0.13249F, 0.08097F, 0.23791F),
                        localAngles = new Vector3(9.26107F, 229.0715F, 18.46826F),
                        localScale = new Vector3(0.01383F, 0.01383F, 0.01383F)
                    }
                }
            );
            return rules;
        }

        private void CreateDoTs()
        {
            DotController.DotDef eternalHaemoDotDef = new DotController.DotDef
            {
                interval = 0.25f,
                damageCoefficient = 0.333f,
                damageColorIndex = DamageColorIndex.SuperBleed,
                associatedBuff = RoR2Content.Buffs.SuperBleed
            };
            eternalHaemoDot = DotAPI.RegisterDotDef(eternalHaemoDotDef);
        }

        public override void Hooks()
        {
            On.RoR2.GlobalEventManager.OnHitEnemy += applyHaemorrhage;
        }

        private void applyHaemorrhage(
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
                    if (DotController.GetDotDef(eternalHaemoDot).associatedBuff == null)
                    {
                        MeltingPotPlugin.ModLogger.LogInfo("Re-applying assoc buff");
                        DotController.dotDefs[(int)eternalHaemoDot].associatedBuff = RoR2Content
                            .Buffs
                            .SuperBleed;
                    }
                    //MeltingPotPlugin.ModLogger.LogInfo($"Buffname - {DotController.dotDefs[(int)eternalHaemoDot].associatedBuff.name}");
                    if (damageInfo.attacker && victim)
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
                                    victim
                                        .GetComponent<CharacterBody>()
                                        .GetBuffCount(BuffCatalog.FindBuffIndex("bdBleeding"))
                                    >= Mathf.Max((bleedStackBase - (count - 1) * stackDegen), 1)
                                )
                                {
                                    DotController.InflictDot(
                                        victim,
                                        damageInfo.attacker,
                                        eternalHaemoDot,
                                        float.PositiveInfinity
                                    );
                                    try
                                    {
                                        DotController dotController;
                                        DotController.dotControllerLocator.TryGetValue(
                                            victim.GetInstanceID(),
                                            out dotController
                                        );
                                        for (
                                            int x = 0;
                                            x
                                                < Mathf.Max(
                                                    (bleedStackBase - (count - 1) * stackDegen),
                                                    1
                                                );
                                            x++
                                        )
                                        {
                                            dotController.RemoveDotStackAtServer(
                                                ((int)DotController.DotIndex.Bleed)
                                            );
                                        }
                                    }
                                    catch
                                    {
                                        // Failed probably due to not having bleed for some reason
                                    }
                                }
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
