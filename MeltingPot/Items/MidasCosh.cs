using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using MeltingPot.Utils;
using static MeltingPot.MeltingPotPlugin;
using UnityEngine.Networking;

namespace MeltingPot.Items
{
    public class MidasCosh : ItemBase<MidasCosh>
    {
        private static float GoldDurationGrowth = 1f;

        private static float GoldImCD = 6f;
        private static float baseProcChance = 0.05f;
        private static float GoldDuration = 4f;
        public override string ItemName => "Midas Cosh";
        public override string ItemLangTokenName => "MIDAS_Cosh";

        public override string ItemPickupDesc => $"Chance to stun enemies. Stunned enemies can be mugged.";

        public override string ItemFullDescription => $"<style=cStack>{baseProcChance*100}% + ({baseProcChance*100}% per stack)</style>chance to <style=cIsUtility>Stun</style> enemies for <style=cStack>{GoldDuration} + ({GoldDurationGrowth} per additional stack) seconds</style>. Enemies stunned by this effect give their attacker <style=cShrine>Scaling Gold</style>";

        public override string ItemLore => "[A sticky note attached to the handle]\n\n" +
            "This was me favourite club, could bash the noggin' of any beastie, gave me plenty of time to reach their pockets.\n\n"+
            "Ive made me fortune an' it's time for me to retire, so you best put it to good use.";
        public static BepInEx.Logging.ManualLogSource BSModLogger;

        public override ItemTag[] ItemTags => new ItemTag[] { ItemTag.Damage };
        public override ItemTier Tier => ItemTier.Tier3;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("MidasCosh.prefab");
        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("MidasCoshIcon.png");

        public static GameObject ItemBodyModelPrefab;
        public static RoR2.BuffDef GoldImmunityBuff;
        public static RoR2.BuffDef GoldFrozenBuff;
        public static GameObject GoldCoinEffect;

        public static GameObject GoldDetonationEffect;



        public override void Init(ConfigFile config)
        {

            CreateItem();
            CreateLang();
            CreateBuff();
            CreateEffect();
            Hooks();
        }


        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = ItemModel;
            ItemBodyModelPrefab.AddComponent<RoR2.ItemDisplay>();
            ItemBodyModelPrefab.GetComponent<RoR2.ItemDisplay>().rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            Vector3 generalScale = new Vector3(1f, 1f, 1f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
            rules.Add("mdlCommandoDualies", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(0.12081F, -0.08941F, 0.11147F),
                    localAngles = new Vector3(12.16602F, 358.043F, 354.1957F),
                    localScale = new Vector3(0.03082F, 0.03082F, 0.03082F)
                }
            });
            rules.Add("mdlHuntress", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Arrow",
                    localPos = new Vector3(-0.01845F, -0.00063F, 0.00333F),
                    localAngles = new Vector3(270.9363F, 141.5657F, 129.9665F),
                    localScale = new Vector3(0.02374F, 0.02374F, 0.02374F)
                }
            });
            rules.Add("mdlToolbot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HandR",
                    localPos = new Vector3(0.21469F, 1.03886F, -1.29432F),
                    localAngles = new Vector3(86.41363F, 131.8821F, 124.6506F),
                    localScale = new Vector3(0.53377F, 0.53377F, 0.53377F)
                }
            });
            rules.Add("mdlEngi", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(-0.18044F, 0.2924F, -0.26926F),
                    localAngles = new Vector3(5.10691F, 179.9999F, 179.9995F),
                    localScale = new Vector3(0.04209F, 0.03512F, 0.04537F)
                }
            });
            rules.Add("mdlMage", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(-0.00334F, -0.11353F, -0.29828F),
                    localAngles = new Vector3(359.9597F, 97.04926F, 358.8439F),
                    localScale = new Vector3(0.04639F, 0.04419F, 0.05004F)
                }
            });
            rules.Add("mdlMerc", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HandR",
                    localPos = new Vector3(-0.15112F, 0.17003F, -0.01842F),
                    localAngles = new Vector3(352.2195F, 0.81314F, 271.0739F),
                    localScale = new Vector3(0.04718F, 0.04807F, 0.06521F)
                }
            });
            rules.Add("mdlTreebot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "FootFrontL",
                    localPos = new Vector3(0.00777F, -0.02718F, -0.04929F),
                    localAngles = new Vector3(2.83203F, 0.24956F, 1.70784F),
                    localScale = new Vector3(0.22501F, 0.10614F, 0.18524F)
                }
            });
            rules.Add("mdlLoader", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MechHandR",
                    localPos = new Vector3(0.07179F, 0.04686F, 0.13934F),
                    localAngles = new Vector3(352.6463F, 217.5504F, 355.9489F),
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
                    localPos = new Vector3(0.59988F, 0.68765F, 1.74379F),
                    localAngles = new Vector3(275.4225F, 170.3385F, 235.5306F),
                    localScale = new Vector3(0.25765F, 0.22358F, 0.24259F)
                }
            });
            rules.Add("mdlCaptain", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(0.13706F, -0.16623F, -0.11384F),
                    localAngles = new Vector3(31.55873F, 10.76436F, 348.9504F),
                    localScale = new Vector3(0.03896F, 0.03896F, 0.03896F)
                }
            });
            rules.Add("mdlBandit2", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Hat",
                    localPos = new Vector3(0.1105F, 0.02167F, 0.00647F),
                    localAngles = new Vector3(357.2404F, 89.84798F, 267.9087F),
                    localScale = new Vector3(0.01093F, 0.01308F, 0.01208F)
                }
            });
            return rules;
        }

        public override void Hooks()
        {
            On.RoR2.GlobalEventManager.OnHitEnemy += Mugged;
            On.RoR2.SetStateOnHurt.OnTakeDamageServer += Goldify;
            On.RoR2.CharacterModel.UpdateOverlays += CharacterModel_UpdateOverlays;
        }

        private void CreateBuff()
        {

            GoldImmunityBuff = ScriptableObject.CreateInstance<BuffDef>();
            GoldImmunityBuff.name = "Melting Pot: Gold State Immunity";
            GoldImmunityBuff.buffColor = new Color(200, 180, 0);
            GoldImmunityBuff.canStack = false;
            GoldImmunityBuff.isDebuff = false;
            GoldImmunityBuff.iconSprite = MainAssets.LoadAsset<Sprite>("MidasCoshPrevent.PNG");

            BuffAPI.Add(new CustomBuff(GoldImmunityBuff)); 
            
            GoldFrozenBuff = ScriptableObject.CreateInstance<BuffDef>();
            GoldFrozenBuff.name = "Melting Pot: Gold State Frozen";
            GoldFrozenBuff.buffColor = Color.clear;
            GoldFrozenBuff.canStack = false;
            GoldFrozenBuff.isDebuff = false;
            GoldFrozenBuff.iconSprite = MainAssets.LoadAsset<Sprite>("MidasCoshMoneyTime.PNG");

            BuffAPI.Add(new CustomBuff(GoldFrozenBuff));

        }

        public void CreateEffect()
        {
            GoldCoinEffect = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/CoinImpact"), "GoldCoinEmitter");

            var mpCoinSoundDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            mpCoinSoundDef.eventName = "Melting_Pot_Coin";
            SoundAPI.AddNetworkedSoundEvent(mpCoinSoundDef);

            var effectComponent = GoldCoinEffect.GetComponent<EffectComponent>();
            effectComponent.soundName = "Melting_Pot_Coin";

            GoldCoinEffect.AddComponent<NetworkIdentity>();

            if (GoldCoinEffect) { PrefabAPI.RegisterNetworkPrefab(GoldCoinEffect); }
            EffectAPI.AddEffect(GoldCoinEffect);

            GoldDetonationEffect = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Effects/GoldShoresArmorRemoval"), "GoldBodyObj");

            var mpGoldSoundDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            mpGoldSoundDef.eventName = "Melting_Pot_Gold";
            SoundAPI.AddNetworkedSoundEvent(mpGoldSoundDef);

            var goldeffectComponent = GoldDetonationEffect.GetComponent<EffectComponent>();
            goldeffectComponent.soundName = "Melting_Pot_Gold";

            GoldDetonationEffect.AddComponent<NetworkIdentity>();

            if (GoldDetonationEffect) { PrefabAPI.RegisterNetworkPrefab(GoldDetonationEffect); }
            EffectAPI.AddEffect(GoldDetonationEffect);
        }
        private void Mugged(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, RoR2.GlobalEventManager self, RoR2.DamageInfo damageInfo, GameObject victim)
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

                        BSModLogger.LogInfo($"Money time");
                        var goldToGain = Run.instance.GetDifficultyScaledCost((int)(5f * damageInfo.procCoefficient));
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
                    BSModLogger.LogError("Failure in characterbody assignment, issues with hurtboxes?");
                }
            }
            orig(self, damageInfo, victim);
            return;
        }
        private void Goldify(On.RoR2.SetStateOnHurt.orig_OnTakeDamageServer orig, SetStateOnHurt self, DamageReport damageReport)
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
                    float m2Proc = 1-1/(1+baseProcChance*InventoryCount);

                    //BSModLogger.LogInfo($"proc chance : {m2Proc} - dr proc {damageReport.damageInfo.procCoefficient} calc proc = {m2Proc*damageReport.damageInfo.procCoefficient}");
                    if (!Util.CheckRoll(m2Proc * damageReport.damageInfo.procCoefficient*100, body.master)) {
                        orig(self, damageReport);
                        return; 
                    }
                    if (self.canBeStunned)
                    { 
                        var timeToGold = (float) (GoldDuration + (1 * (InventoryCount - 1)));
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
                        var goldToGain = Run.instance.GetDifficultyScaledCost((int)(5f * damageReport.damageInfo.procCoefficient));
                        body.master.GiveMoney((uint)goldToGain);
                    }
                }
            }
            orig(self, damageReport);
            return;
        }

        private void CharacterModel_UpdateOverlays(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
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
                        if (!overlayController) overlayController = self.body.gameObject.AddComponent<OverlayTracker>();
                        else return;
                        overlayController.Body = self.body;
                        overlay.duration = float.PositiveInfinity;
                        overlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                        overlay.animateShaderAlpha = true;
                        overlay.destroyComponentOnEnd = true;
                        overlay.originalMaterial = Resources.Load<Material>("Materials/matImmune");
                        overlay.AddToCharacerModel(self);
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