using BepInEx.Configuration;
using MeltingPot.Utils;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace MeltingPot.Items
{
    public class MidasCosh : ItemBase<MidasCosh>
    {
        public static float GoldDurationGrowth = 1f;

        private static float GoldImCD = 6f;
        public static float baseProcChance = 0.01f;
        public static float GoldDuration = 4f;
        public override string ItemName => "Midas Cosh";
        public override string ItemLangTokenName => "MIDASCOSH";
        public override string ItemPickupDesc =>
            $"Chance to stun enemies. Stunned enemies can be mugged.";
        public override string ItemFullDescription =>
            $"<style=cStack>{baseProcChance * 100}% + ({baseProcChance * 100}% per stack)</style>chance to <style=cIsUtility>Stun</style> enemies for <style=cStack>{GoldDuration} + ({GoldDurationGrowth} per additional stack) seconds</style>. Enemies stunned by this effect give their attacker <style=cShrine>Scaling Gold</style>";
        public override string ItemLore =>
            "[A sticky note attached to the handle]\n\n"
            + "This was me favourite club, could bash the noggin' of any beastie, gave me plenty of time to reach their pockets.\n\n"
            + "Ive made me fortune an' it's time for me to retire, so you best put it to good use.";

        public override string VoidCounterpart => null;
        public static BepInEx.Logging.ManualLogSource BSModLogger;
        public static GameObject GoldCoinEffect;

        public static GameObject GoldDetonationEffect;

        public static GameObject ItemModel;
        public static GameObject ItemBodyModelPrefab;

        public static BuffDef GoldImmunityBuff =>
            ContentPackProvider.contentPack.buffDefs.Find("MeltingPot_GoldStateImmunity");
        public static BuffDef GoldFrozenBuff =>
            ContentPackProvider.contentPack.buffDefs.Find("MeltingPot_GoldStateFrozen");

        public static Material MidasFrozenOverlay =>
            LegacyResourcesAPI.Load<Material>("Materials/matImmune");

        public override void Init(ConfigFile config, bool enabled)
        {
            CreateItem("MidasCosh_ItemDef", enabled);
            if (enabled)
            {
                ItemModel = Assets.mainAssetBundle.LoadAsset<GameObject>(
                    $"{ModelPath}/midas_cosh/midascosh.prefab"
                );
                CreateLang();
                CreateEffect();
                Hooks();
            }
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>(
                $"{ModelPath}/midas_cosh/displaymidascosh.prefab"
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
                        childName = "Arrow",
                        localPos = new Vector3(0.03453F, 0.00813F, 0.11169F),
                        localAngles = new Vector3(270.9361F, 141.5653F, 127.4167F),
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
                        localPos = new Vector3(2.1836F, 0.81396F, 0.20312F),
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
                        childName = "Chest",
                        localPos = new Vector3(-0.03298F, 0.21711F, -0.28224F),
                        localAngles = new Vector3(5.10691F, 179.9999F, 191.9932F),
                        localScale = new Vector3(0.04895F, 0.04084F, 0.05276F)
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
                        localPos = new Vector3(0.0691F, -0.00967F, 0.01725F),
                        localAngles = new Vector3(352.2195F, 0.81314F, 271.0739F),
                        localScale = new Vector3(0.08138F, 0.08292F, 0.11248F)
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
                        childName = "FootFrontL",
                        localPos = new Vector3(1.01201F, 0.32896F, -0.02394F),
                        localAngles = new Vector3(2.83203F, 0.24956F, 1.70784F),
                        localScale = new Vector3(0.37948F, 0.1509F, 0.30983F)
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
                        childName = "SpineChest2",
                        localPos = new Vector3(1.25818F, 0.22855F, 1.46486F),
                        localAngles = new Vector3(275.4225F, 170.3384F, 217.8794F),
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
                        childName = "Pelvis",
                        localPos = new Vector3(0.21691F, -0.12037F, -0.22717F),
                        localAngles = new Vector3(19.96512F, 15.38571F, 341.2212F),
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
                        childName = "Hat",
                        localPos = new Vector3(0.11191F, -0.00771F, 0.00755F),
                        localAngles = new Vector3(357.2404F, 89.84798F, 267.9087F),
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
                        childName = "Muzzle",
                        localPos = new Vector3(0F, 0.47463F, -2.31789F),
                        localAngles = new Vector3(0F, 90F, 90F),
                        localScale = new Vector3(0.17248F, 0.20638F, 0.19058F)
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
                        localPos = new Vector3(-0.95572F, 1.57584F, -0.80645F),
                        localAngles = new Vector3(346.3986F, 301.0652F, 230.3974F),
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
                        childName = "Backpack",
                        localPos = new Vector3(-0.10159F, 0.15489F, -0.0914F),
                        localAngles = new Vector3(355.7018F, 180F, 180F),
                        localScale = new Vector3(0.06144F, 0.06144F, 0.06144F)
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
                        localPos = new Vector3(-0.05618F, 0.34391F, -0.00022F),
                        localAngles = new Vector3(357.677F, 274.3389F, 270.2009F),
                        localScale = new Vector3(0.05527F, 0.05527F, 0.05527F)
                    }
                }
            );
            return rules;
        }

        public override void Hooks()
        {
            On.RoR2.GlobalEventManager.OnHitEnemy += Mugged;
            On.RoR2.SetStateOnHurt.OnTakeDamageServer += Goldify;
            On.RoR2.CharacterModel.UpdateOverlays += CharacterModel_UpdateOverlays;
        }

        public void CreateEffect()
        {
            GoldCoinEffect = PrefabAPI.InstantiateClone(
                LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/CoinImpact"),
                "GoldCoinEmitter"
            );

            var effectComponent = GoldCoinEffect.GetComponent<EffectComponent>();
            effectComponent.soundName = "Melting_Pot_Coin";

            GoldCoinEffect.AddComponent<NetworkIdentity>();

            if (GoldCoinEffect)
            {
                PrefabAPI.RegisterNetworkPrefab(GoldCoinEffect);
            }
            ContentAddition.AddEffect(GoldCoinEffect);

            GoldDetonationEffect = PrefabAPI.InstantiateClone(
                LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/GoldShoresArmorRemoval"),
                "GoldBodyObj"
            );

            var goldeffectComponent = GoldDetonationEffect.GetComponent<EffectComponent>();
            goldeffectComponent.soundName = "Melting_Pot_Gold";

            GoldDetonationEffect.AddComponent<NetworkIdentity>();

            if (GoldDetonationEffect)
            {
                PrefabAPI.RegisterNetworkPrefab(GoldDetonationEffect);
            }
            ContentAddition.AddEffect(GoldDetonationEffect);
        }

        private void Mugged(
            On.RoR2.GlobalEventManager.orig_OnHitEnemy orig,
            RoR2.GlobalEventManager self,
            RoR2.DamageInfo damageInfo,
            GameObject victim
        )
        {
            BSModLogger = MeltingPotPlugin.ModLogger;
            if (damageInfo.rejected || damageInfo.procCoefficient <= 0)
            {
                //BSModLogger.LogInfo($"Rejected");
                orig(self, damageInfo, victim);
                return;
            }
            if (victim && damageInfo.attacker)
            {
                //BSModLogger.LogInfo($"Victim and attacker exist");
                try
                {
                    //BSModLogger.LogInfo($"Body pre-assing");
                    var attacker = damageInfo.attacker.GetComponent<CharacterBody>();

                    //BSModLogger.LogInfo($"Attacker post assign");
                    var body = victim.GetComponent<CharacterBody>();

                    //BSModLogger.LogInfo($"Body post-assign");
                    if (body.HasBuff(GoldFrozenBuff))
                    {
                        //BSModLogger.LogInfo($"Money time");
                        var goldToGain = Run.instance.GetDifficultyScaledCost(
                            (int)(5f * damageInfo.procCoefficient)
                        );
                        attacker.master.GiveMoney((uint)goldToGain);

                        EffectData effectData = new EffectData
                        {
                            origin = body.corePosition,
                            genericFloat = 0.5f,
                            scale = 4.0f,
                        };
                        effectData.SetHurtBoxReference(body.mainHurtBox);
                        GameObject effectPrefab = GoldCoinEffect;
                        EffectManager.SpawnEffect(effectPrefab, effectData, true);
                    }
                }
                catch
                {
                    BSModLogger.LogError(
                        "Failure in characterbody assignment, issues with hurtboxes?"
                    );
                }
            }
            orig(self, damageInfo, victim);
            return;
        }

        private void Goldify(
            On.RoR2.SetStateOnHurt.orig_OnTakeDamageServer orig,
            SetStateOnHurt self,
            DamageReport damageReport
        )
        {
            var body = damageReport.attackerBody;
            var victimBody = damageReport.victimBody;
            if (body && victimBody)
            {
                if (victimBody.HasBuff(GoldImmunityBuff))
                {
                    orig(self, damageReport);
                    return;
                }
                var InventoryCount = GetCount(body);
                if (InventoryCount > 0)
                {
                    //BSModLogger = MeltingPotPlugin.ModLogger;
                    float m2Proc = 1 - 1 / (1 + baseProcChance * InventoryCount);

                    //BSModLogger.LogInfo($"proc chance : {m2Proc} - dr proc {damageReport.damageInfo.procCoefficient} calc proc = {m2Proc*damageReport.damageInfo.procCoefficient}");
                    if (
                        !Util.CheckRoll(
                            m2Proc * damageReport.damageInfo.procCoefficient * 100,
                            body.master
                        )
                    )
                    {
                        orig(self, damageReport);
                        return;
                    }
                    if (self.canBeStunned)
                    {
                        var timeToGold = (float)(GoldDuration + (1 * (InventoryCount - 1)));
                        self.SetStun(timeToGold);
                        victimBody.AddTimedBuff(GoldFrozenBuff, timeToGold);
                        victimBody.AddTimedBuff(GoldImmunityBuff, GoldImCD);
                        var victim = damageReport.victim;
                        EffectData effectData = new EffectData
                        {
                            origin = victimBody.corePosition,
                            genericFloat = 0.005f,
                            scale = 0.00000001f,
                        };
                        effectData.SetHurtBoxReference(victimBody.mainHurtBox);
                        GameObject effectPrefab = GoldDetonationEffect;
                        EffectManager.SpawnEffect(effectPrefab, effectData, true);
                        var goldToGain = Run.instance.GetDifficultyScaledCost(
                            (int)(5f * damageReport.damageInfo.procCoefficient)
                        );
                        body.master.GiveMoney((uint)goldToGain);
                    }
                }
            }
            orig(self, damageReport);
            return;
        }

        private void CharacterModel_UpdateOverlays(
            On.RoR2.CharacterModel.orig_UpdateOverlays orig,
            CharacterModel self
        )
        {
            orig(self);
            try
            {
                if (self)
                {
                    if (self.body && self.body.HasBuff(GoldFrozenBuff))
                    {
                        TemporaryOverlay overlay = self.gameObject.AddComponent<TemporaryOverlay>();
                        var overlayController = self.body.GetComponent<OverlayTracker>();
                        if (!overlayController)
                            overlayController = self.body.gameObject.AddComponent<OverlayTracker>();
                        overlayController.Body = self.body;
                        overlay.duration = GoldDuration;
                        overlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                        overlay.animateShaderAlpha = true;
                        overlay.destroyComponentOnEnd = true;
                        overlay.originalMaterial = MidasFrozenOverlay;
                        overlay.AddToCharacerModel(self.GetComponent<CharacterModel>());
                        overlayController.Overlay = overlay;
                        overlayController.Buff = GoldFrozenBuff.buffIndex;
                    }
                }
            }
            catch
            {
                return;
            }
        }
    }
}
