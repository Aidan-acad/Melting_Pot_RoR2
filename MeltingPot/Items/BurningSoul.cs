using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using MeltingPot.Utils;
using static R2API.RecalculateStatsAPI;
using static MeltingPot.MeltingPotPlugin;
using System.Linq;
using UnityEngine.Networking;

namespace MeltingPot.Items
    {
    public class BurningSoul : ItemBase<BurningSoul>
    {
        public static BuffDef soulBurnBuff { get; private set; }
        public static BuffDef soulBurnSelfBuff { get; private set; }

        public static DotController.DotIndex soulBurnDot { get; private set; }
        public static DotController.DotIndex soulBurnSelfDot { get; private set; }

        private static int health_threshold = 1000;
        private static float enemy_burn_percent = 0.04f;
        private static float enemy_burn_percent_growth = 0.015f;
        private static float self_burn_percent = 0.01f;
        private static float self_burn_percent_growth = 0.005f;
        public override string ItemName => "Burning Soul";
        public override string ItemLangTokenName => "BURNING_SOUL";

        public override string ItemPickupDesc => $"Doubles <style=cIsHealing>Health Regen</style>. Attacks <style=cIsDamage>burn</style> enemies for max health damage at the cost of your own";

        public override string ItemFullDescription => $"Increases <style=cIsHealing>health regen by 100%</style>. Once over <style=cIsHealth>{health_threshold} health</style>, burn enemies for <style=cIsDamage>{enemy_burn_percent * 100}%</style><style=cStack>(+{enemy_burn_percent_growth * 100} per stack)</style> of your <style=cIsHealth>Maximum Health</style> over <style=cStack>4 seconds</style> <style=cDeath>BUT</style> all attacks cost <style=cIsHealth>{self_burn_percent * 100}%</style><style=cStack>(+{self_burn_percent_growth * 100}% per stack)</style> of your current health.";

        public override string ItemLore => "[A whispering scream emanates from the bowl]\n\n" +
            "Best not to peer too closely into the fire, lest its tongues choose to taste blood";
        public static BepInEx.Logging.ManualLogSource BSModLogger;

        public override ItemTag[] ItemTags => new ItemTag[] { ItemTag.Damage };
        public override ItemTier Tier => ItemTier.Lunar;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("BurningSoulRedux.prefab");
        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("BurningSoul_Icon.png");

        public static GameObject ItemBodyModelPrefab;
        public static RoR2.BuffDef BurningSoulActiveBuff;
        public GameObject SoulInflictor = new GameObject("Burning Soul Damage");
        public static GameObject FireEffect;

        private class BurningSoulController : MonoBehaviour
        {
            private float heldHealth = 0;
            private float healthdamage = 0;
            private float selfDamage = 0;

            public float getMaxHealth() { return heldHealth; }
            public float getInflictedDmg() { return healthdamage; }
            public float getSelfDmg() { return selfDamage; }

            public void setMaxHealth(float input) { heldHealth = input; }
            public void setInflictedDmg(float input) { healthdamage = input; }
            public void setSelfDmg(float input) { selfDamage = input; }
        }


        public override void Init(ConfigFile config)
        {
            CreateLang();
            CreateEffect();
            CreateItem();
            CreateBuff();
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
                    childName = "Head",
                    localPos = new Vector3(0.00108F, 0.23116F, 0.03563F),
                    localAngles = new Vector3(348.7211F, 359.9553F, 0.02765F),
                    localScale = new Vector3(0.2F, 0.2F, 0.2F)
                }
            });
            rules.Add("mdlHuntress", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0.00115F, 0.22517F, 0.03147F),
                    localAngles = new Vector3(341.3349F, 359.916F, 0.04797F),
                    localScale = new Vector3(0.2F, 0.2F, 0.2F)
                }
            });
            rules.Add("mdlToolbot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0.03597F, 2.49258F, 1.46246F),
                    localAngles = new Vector3(58.21444F, 359.9872F, 0.12899F),
                    localScale = new Vector3(1.3F, 1.3F, 1.3F)
                }
            });
            rules.Add("mdlEngi", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MuzzleLeft",
                    localPos = new Vector3(0.02968F, -0.5256F, -0.22816F),
                    localAngles = new Vector3(5.10691F, 179.9999F, 179.9995F),
                    localScale = new Vector3(0.03714F, 0.03714F, 0.03714F)
                },
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MuzzleRight",
                    localPos = new Vector3(-0.03153F, -0.52563F, -0.22815F),
                    localAngles = new Vector3(5.10691F, 179.9999F, 179.9995F),
                    localScale = new Vector3(0.03714F, 0.03714F, 0.03714F)
                }
            });
            rules.Add("mdlMage", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.7326F, 0.50043F, -0.41105F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.2F, 0.2F, 0.2F)
                }
            });
            rules.Add("mdlMerc", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0.00092F, 0.28475F, 0.01279F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.2F, 0.2F, 0.2F)
                }
            });
            rules.Add("mdlTreebot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "FlowerBase",
                    localPos = new Vector3(0.1698F, 1.77142F, -0.00851F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.2378F, 0.2378F, 0.2378F)
                }
            });
            rules.Add("mdlLoader", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0.001F, 0.19032F, -0.39663F),
                    localAngles = new Vector3(323.2881F, 0.09313F, 359.7203F),
                    localScale = new Vector3(0.2F, 0.2F, 0.2F)
                }
            });
            rules.Add("mdlCroco", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(-0.07026F, 1.16075F, -2.07301F),
                    localAngles = new Vector3(275.6001F, 353.5875F, 6.07229F),
                    localScale = new Vector3(0.76731F, 0.76731F, 0.76731F)
                }
            });
            rules.Add("mdlCaptain", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(-1.15025F, 0.40931F, -0.35049F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.10606F, 0.10606F, 0.10606F)
                }
            });
            rules.Add("mdlBandit2", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MuzzleShotgun",
                    localPos = new Vector3(-0.08031F, -0.00362F, -0.55244F),
                    localAngles = new Vector3(359.9369F, 0.46435F, 93.25298F),
                    localScale = new Vector3(0.02685F, 0.02685F, 0.02705F)
                }
            });
            return rules;
        }

        public override void Hooks()
        {
            On.RoR2.CharacterBody.FixedUpdate += ApplyBuffAsIndicatorForReady;
            On.RoR2.GlobalEventManager.OnHitEnemy += FlamingSoul;
            On.RoR2.OverlapAttack.Fire += MeleeDrain;
            On.RoR2.BulletAttack.Fire += RangedDrain;
            //On.RoR2.GenericSkill.OnExecute += SkillDrain;
            R2API.RecalculateStatsAPI.GetStatCoefficients += GrantBaseRegen;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += ProjectileDrain;
        }

        public void CreateEffect()
        {
            /*var all_of_em = Resources.LoadAll<GameObject>("Prefabs/Effects/ImpactEffects");
            BSModLogger = ModLogger;
            foreach (var item in all_of_em)
            {
                BSModLogger.LogInfo($"effect -> {item}");
            }*/

            FireEffect = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/ExplosionSolarFlare"), "FireEffect");

            var mpFireSoundDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            mpFireSoundDef.eventName = "Melting_Pot_Flames";
            SoundAPI.AddNetworkedSoundEvent(mpFireSoundDef);

            var fireEffectComponent = FireEffect.GetComponent<EffectComponent>();
            fireEffectComponent.soundName = "Melting_Pot_Flames";

            FireEffect.AddComponent<NetworkIdentity>();

            if (FireEffect) { PrefabAPI.RegisterNetworkPrefab(FireEffect); }
            EffectAPI.AddEffect(FireEffect);
        }

        private void GrantBaseRegen(CharacterBody sender, StatHookEventArgs args)
        {
            if (GetCount(sender) > 0)
            {
                args.regenMultAdd += 1;
            }
        }

        private void ApplyBuffAsIndicatorForReady(On.RoR2.CharacterBody.orig_FixedUpdate orig, RoR2.CharacterBody self)
        {
            if (self)
            {
                var InventoryCount = GetCount(self);
                if (InventoryCount > 0)
                {
                    var soulCtrl = self.gameObject.GetComponent<BurningSoulController>();
                    if (!soulCtrl) { soulCtrl = self.gameObject.AddComponent<BurningSoulController>(); }
                    if (self.healthComponent.fullCombinedHealth > health_threshold)
                    {
                        if (!self.HasBuff(BurningSoulActiveBuff))
                        {
                            self.AddBuff(BurningSoulActiveBuff);
                        }
                        if (soulCtrl.getMaxHealth() != self.healthComponent.fullCombinedHealth)
                        {
                            soulCtrl.setMaxHealth(self.healthComponent.fullCombinedHealth);
                            soulCtrl.setInflictedDmg((float)Math.Ceiling((enemy_burn_percent + (InventoryCount - 1) * enemy_burn_percent_growth) * soulCtrl.getMaxHealth() / 4));
                        }
                        soulCtrl.setSelfDmg((float)Math.Floor(self.healthComponent.combinedHealth * (self_burn_percent + self_burn_percent_growth * (GetCount(self) - 1))));
                    }
                    else
                    {
                        if (self.HasBuff(BurningSoulActiveBuff))
                        {
                            self.RemoveBuff(BurningSoulActiveBuff);
                        }
                    }
                }
                else
                {
                    //var soulCtrl = self.gameObject.GetComponent<BurningSoulController>();
                    //if (soulCtrl) { UnityEngine.Object.Destroy(soulCtrl); }
                    if (self.HasBuff(BurningSoulActiveBuff))
                    {
                        self.RemoveBuff(BurningSoulActiveBuff);
                    }
                }
            }
            orig(self);
        }

        private void CreateBuff()
        {
            BurningSoulActiveBuff = ScriptableObject.CreateInstance<RoR2.BuffDef>();
            BurningSoulActiveBuff.name = "Melting Pot : Burning Soul";
            BurningSoulActiveBuff.buffColor = Color.white;
            BurningSoulActiveBuff.canStack = false;
            BurningSoulActiveBuff.isDebuff = false;
            BurningSoulActiveBuff.iconSprite = ItemIcon;

            BuffAPI.Add(new CustomBuff(BurningSoulActiveBuff));

            soulBurnBuff = ScriptableObject.CreateInstance<BuffDef>();
            soulBurnBuff.buffColor = Color.cyan;
            soulBurnBuff.canStack = true;
            soulBurnBuff.isDebuff = true;
            soulBurnBuff.name = "MPSOULBURN";
            soulBurnBuff.iconSprite = MainAssets.LoadAsset<Sprite>("BurningSoul_Icon.png");

            CustomBuff sbCustomBuff = new CustomBuff(soulBurnBuff);
            BuffAPI.Add(sbCustomBuff);

            DotController.DotDef sbDotDef = new DotController.DotDef
            {
                interval = 1f,
                damageCoefficient = 1,
                damageColorIndex = DamageColorIndex.DeathMark,
                associatedBuff = soulBurnBuff
            };
            soulBurnDot = DotAPI.RegisterDotDef(sbDotDef, (dotController, dotStack) =>
            {
                CharacterBody attackerBody = dotStack.attackerObject.GetComponent<CharacterBody>();
                if (attackerBody && attackerBody.gameObject.GetComponent<BurningSoulController>())
                {
                    //float damageMultiplier = dmgCoefficient + dmgStack * (GetCount(attackerBody) - 1);
                    float burnDamage = attackerBody.gameObject.GetComponent<BurningSoulController>().getInflictedDmg();
                    dotStack.damage = burnDamage;
                }
            });

            soulBurnSelfBuff = ScriptableObject.CreateInstance<BuffDef>();
            soulBurnSelfBuff.buffColor = Color.red;
            soulBurnSelfBuff.canStack = true;
            soulBurnSelfBuff.isDebuff = true;
            soulBurnSelfBuff.name = "MPSOULBURNSELF";
            soulBurnSelfBuff.iconSprite = MainAssets.LoadAsset<Sprite>("BurningSoul_Icon.png");

            CustomBuff sbSelfCustomBuff = new CustomBuff(soulBurnSelfBuff);
            BuffAPI.Add(sbSelfCustomBuff);

            DotController.DotDef sbSelfDotDef = new DotController.DotDef
            {
                interval = 0.5f,
                damageCoefficient = 1f,
                damageColorIndex = DamageColorIndex.Item,
                associatedBuff = soulBurnSelfBuff
            };
            soulBurnSelfDot = DotAPI.RegisterDotDef(sbSelfDotDef, (dotController, dotStack) =>
            {
                CharacterBody attackerBody = dotStack.attackerObject.GetComponent<CharacterBody>();
                if (attackerBody && attackerBody.gameObject.GetComponent<BurningSoulController>())
                {
                    float burnDamage = (float)Math.Ceiling(attackerBody.gameObject.GetComponent<BurningSoulController>().getSelfDmg());
                    dotStack.damage = burnDamage;
                    dotStack.damageType = DamageType.BypassArmor;
                }
            });
        }
        private bool MeleeDrain(On.RoR2.OverlapAttack.orig_Fire orig, RoR2.OverlapAttack self, List<RoR2.HurtBox> hitResults)
            {

            var owner = self.attacker;
            if (owner)
            {
                var body = owner.GetComponent<RoR2.CharacterBody>();
                if (body)
                {
                    var InventoryCount = GetCount(body);
                    if (InventoryCount > 0)
                    {
                        if (body.HasBuff(BurningSoulActiveBuff))
                        {
                            var cooldownHandler = owner.GetComponent<MeleeCooldownHandler>();
                            if (!cooldownHandler) { cooldownHandler = owner.AddComponent<MeleeCooldownHandler>(); }

                            if (!cooldownHandler.MeleeTracker.ContainsKey(self))
                            {
                                cooldownHandler.MeleeTracker.Add(self, 0);
                                DotController.InflictDot(owner, owner, soulBurnSelfDot, 1f);
                            }
                            }
                        }
                    }
                }
            return orig(self, hitResults);
        }

        private void ProjectileDrain(On.RoR2.Projectile.ProjectileManager.orig_FireProjectile_FireProjectileInfo orig, RoR2.Projectile.ProjectileManager self, RoR2.Projectile.FireProjectileInfo fireProjectileInfo)
        {
            var owner = fireProjectileInfo.owner;
            BSModLogger = MeltingPotPlugin.ModLogger;
            if (owner)
            {
                var body = owner.GetComponent<RoR2.CharacterBody>();
                if (body)
                {
                    var InventoryCount = GetCount(body);
                    if (InventoryCount > 0)
                    {
                        if (body.HasBuff(BurningSoulActiveBuff))
                        {
                            BSModLogger.LogInfo($"Melting Pot -- Applying DOT {soulBurnSelfDot}");
                            CmdApplyDot(owner, soulBurnSelfDot, 1f);
                            //DotController.InflictDot(owner, owner, soulBurnSelfDot, 1f);
                        }
                    }
                }
            }
            orig(self, fireProjectileInfo);
        }

        public class MeleeCooldownHandler : MonoBehaviour
        {
            public Dictionary<RoR2.OverlapAttack, float> MeleeTracker = new Dictionary<RoR2.OverlapAttack, float>();

            public void FixedUpdate()
            {
                foreach (RoR2.OverlapAttack attack in MeleeTracker.Keys.ToList())
                {
                    var time = MeleeTracker[attack];
                    time += Time.fixedDeltaTime;

                    if (time > 5)
                    {
                        MeleeTracker.Remove(attack);
                    }
                    else
                    {
                        MeleeTracker[attack] = time;
                    }
                }
            }
        }
        private void SkillDrain(On.RoR2.GenericSkill.orig_OnExecute orig, global::RoR2.GenericSkill self)
            {
                var owner = self.characterBody;
                if (owner)
                {
                    var body = owner;
                    if (body)
                    {
                        var InventoryCount = GetCount(body);
                        if (InventoryCount > 0)
                        {
                            if (body.HasBuff(BurningSoulActiveBuff))
                            {
                            //DotController.InflictDot(owner.gameObject, owner.gameObject, soulBurnSelfDot, 1f);
                            CmdApplyDot(owner.gameObject, soulBurnSelfDot, 1f);
                            /*var soul = body.GetComponent<RoR2.HealthComponent>();
                            var burn_to_apply = soul.combinedHealth * (0.01 + 0.005 * (InventoryCount - 1));
                            DamageInfo di = new DamageInfo
                            {
                                position = body.corePosition,
                                attacker = owner.gameObject,
                                inflictor = BurningSoul.instance.SoulInflictor,
                                crit = false,
                                damage = (float)burn_to_apply,
                                damageColorIndex = DamageColorIndex.Bleed,
                                damageType = DamageType.NonLethal,
                                force = Vector3.zero,
                                procCoefficient = 0,
                                procChainMask = default(ProcChainMask)
                            };
                            soul.TakeDamage(di);*/
                        }
                        }
                    }
                }
                orig(self);
            }
        private void RangedDrain(On.RoR2.BulletAttack.orig_Fire orig, RoR2.BulletAttack self)
        {
            var owner = self.owner;
            BSModLogger = MeltingPotPlugin.ModLogger;
            if (owner)
            {
                var body = owner.GetComponent<RoR2.CharacterBody>();
                if (body)
                {
                    var InventoryCount = GetCount(body);
                    if (InventoryCount > 0)
                    {
                        if (body.HasBuff(BurningSoulActiveBuff))
                        {
                            BSModLogger.LogInfo($"Melting Pot -- Applying DOT {soulBurnSelfDot}");
                            CmdApplyDot(owner, soulBurnSelfDot, 1f);
                            //DotController.InflictDot(owner, owner, soulBurnSelfDot, 1f);
                            /*var soul = body.GetComponent<RoR2.HealthComponent>();
                            var burn_to_apply = soul.combinedHealth * (0.01 + 0.005 * (InventoryCount - 1));
                            DamageInfo di = new DamageInfo
                            {
                                position = body.corePosition,
                                attacker = owner,
                                inflictor = BurningSoul.instance.SoulInflictor,
                                crit = false,
                                damage = (float)burn_to_apply,
                                damageColorIndex = DamageColorIndex.Bleed,
                                damageType = DamageType.NonLethal,
                                force = Vector3.zero,
                                procCoefficient = 0,
                                procChainMask = default(ProcChainMask)
                            };
                            soul.TakeDamage(di);*/
                        }
                    }
                }
            }
            orig(self);
        }

        [Command]
        void CmdApplyDot(GameObject target, DotController.DotIndex burnDot, float burnDuration)
        {
            DotController.InflictDot(target, target, burnDot, burnDuration);
        }

        private void FlamingSoul(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, RoR2.GlobalEventManager self, RoR2.DamageInfo damageInfo, GameObject victim)
            {
                if (damageInfo.rejected || damageInfo.procCoefficient <= 0)
                {
                    orig(self, damageInfo, victim);
                    return;
                }

                var attacker = damageInfo.attacker;
                if (attacker)
                {
                    var body = attacker.GetComponent<CharacterBody>();
                    var victimHealth = victim.GetComponent<HealthComponent>();
                    if (body && victimHealth)
                    {
                        var InventoryCount = GetCount(body);
                        if (InventoryCount > 0 && body.HasBuff(BurningSoulActiveBuff))
                        {
                            var ratio = damageInfo.damage / body.damage;
                            int stacksToApply = (int)Math.Ceiling(ratio);
                            EffectData effectData = new EffectData
                            {
                                origin = victim.GetComponent<CharacterBody>().corePosition,
                                genericFloat = 0.5f,
                                scale = 1.5f,
                            };
                            effectData.SetHurtBoxReference(victim.GetComponent<CharacterBody>().mainHurtBox);
                            GameObject effectPrefab = FireEffect;
                            EffectManager.SpawnEffect(effectPrefab, effectData, true);
                            for (int i = 0; i < stacksToApply; i++)
                            {
                                DotController.InflictDot(victim, damageInfo.attacker, soulBurnDot, 4f);
                            }
                    }
                    }
                }
                orig(self, damageInfo, victim);
            }
        }
    }