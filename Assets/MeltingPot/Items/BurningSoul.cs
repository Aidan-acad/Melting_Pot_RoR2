using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using MeltingPot.Utils;
using static R2API.RecalculateStatsAPI;
using System.Linq;
using UnityEngine.Networking;

namespace MeltingPot.Items
    {
    public class BurningSoul : ItemBase<BurningSoul>
    {
        public static BuffDef soulBurnBuff => ContentPackProvider.contentPack.buffDefs.Find("MeltingPot_SoulBurn");
        public static BuffDef soulBurnSelfBuff => ContentPackProvider.contentPack.buffDefs.Find("MeltingPot_SelfSoulburn");
        public static BuffDef BurningSoulActiveBuff => ContentPackProvider.contentPack.buffDefs.Find("MeltingPot_SoulBurnActive");
        public static DotController.DotIndex soulBurnDot { get; private set; }
        public static DotController.DotIndex soulBurnSelfDot { get; private set; }

        private static int health_threshold = 1000;
        private static float enemy_burn_percent = 0.04f;
        private static float enemy_burn_percent_growth = 0.015f;
        private static float self_burn_percent = 0.01f;
        private static float self_burn_percent_growth = 0.005f;

        private static float burnTimeOut = 0.01f;

        public override string ItemName => "Burning Soul";
        public override string ItemLangTokenName => "BURNINGSOUL";

        public override string ItemPickupDesc => $"Doubles <style=cIsHealing>Health Regen</style>. Attacks <style=cIsDamage>burn</style> enemies for max health damage at the cost of your own";

        public override string ItemFullDescription => $"Increases <style=cIsHealing>health regen by 100%</style>. Once over <style=cIsHealth>{health_threshold} health</style>, burn enemies for <style=cIsDamage>{enemy_burn_percent * 100}%</style><style=cStack>(+{enemy_burn_percent_growth * 100} per stack)</style> of your <style=cIsHealth>Maximum Health</style> over <style=cStack>4 seconds</style> <style=cDeath>BUT</style> all attacks cost <style=cIsHealth>{self_burn_percent * 100}%</style><style=cStack>(+{self_burn_percent_growth * 100}% per stack)</style> of your current health.";

        public override string ItemLore => "[A whispering scream emanates from the bowl]\n\n" +
            "Best not to peer too closely into the fire, lest its tongues choose to taste blood";
        public static BepInEx.Logging.ManualLogSource BSModLogger;

        public static GameObject ItemModel => Assets.mainAssetBundle.LoadAsset<GameObject>("assets/meltingpot/mpassets/itemprefabs/burning_soul/burningsoul.prefab");
        public static GameObject ItemBodyModelPrefab;

        public GameObject SoulInflictor = new GameObject("Burning Soul Damage");
        public static GameObject FireEffect;

        private class BurningSoulController : CharacterBody.ItemBehavior
        {
            private float heldHealth = 0;
            private float healthdamage = 0;
            private float selfDamage = 0;

            public void Awake()
            {
                this.body = base.gameObject.GetComponent<CharacterBody>();
            }

            public void FixedUpdate()
            {
                timeSinceBurnt += Time.fixedDeltaTime;
                if (burnt)
                {
                    burnt = false;
                    timeSinceBurnt = 0f;
                    DotController.InflictDot(base.gameObject, base.gameObject, BurningSoul.soulBurnSelfDot, 1f);
                }
                if (this.body.healthComponent.fullCombinedHealth > health_threshold)
                {
                    if (!body.HasBuff(BurningSoulActiveBuff))
                    {
                        body.AddBuff(BurningSoulActiveBuff);
                    }
                    if (getMaxHealth() != body.healthComponent.fullCombinedHealth)
                    {
                        setMaxHealth(body.healthComponent.fullCombinedHealth);
                        setInflictedDmg((float)Math.Ceiling((enemy_burn_percent + (stack - 1) * enemy_burn_percent_growth) * getMaxHealth() / 4));
                    }
                    setSelfDmg((float)Math.Floor(body.healthComponent.combinedHealth * (self_burn_percent + self_burn_percent_growth * (stack - 1))));
                    setSelfBurn(getSelfBurn() + Time.fixedDeltaTime);
                }
                else
                {
                    if (body.HasBuff(BurningSoulActiveBuff))
                    {
                        body.RemoveBuff(BurningSoulActiveBuff);
                    }
                }
            }
            public float getMaxHealth() { return heldHealth; }
            public float getInflictedDmg() { return healthdamage; }
            public float getSelfDmg() { return selfDamage; }

            public void setMaxHealth(float input) { heldHealth = input; }
            public void setInflictedDmg(float input) { healthdamage = input; }
            public void setSelfDmg(float input) { selfDamage = input; }

            private float timeSinceBurnt = 0f;
            public float getSelfBurn() { return timeSinceBurnt; }
            public void setSelfBurn(float input) { timeSinceBurnt = input; }

            public bool burnt = false;
            public bool getBurnt() { return burnt; }
            public void setBurnt(bool input) { burnt = input; }

        }

        private void HandleBurningCtrl(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, global::RoR2.CharacterBody self)
        {
            self.AddItemBehavior<BurningSoulController>(base.GetCount(self));
            orig(self);
        }


        public override void Init(ConfigFile config)
        {
            CreateItem("BurningSoul_ItemDef");
            CreateLang();
            CreateEffect();
            CreateDoTs();
            Hooks();

        }


        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>("assets/meltingpot/mpassets/itemprefabs/burning_soul/displayburningsoul.prefab");
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
                    localPos = new Vector3(0.00158F, 0.08626F, 0.02043F),
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
                    localPos = new Vector3(0.00196F, 0.02299F, -0.00822F),
                    localAngles = new Vector3(2.53829F, 0.0134F, 359.9932F),
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
                    localPos = new Vector3(0.03488F, 1.93909F, 1.62974F),
                    localAngles = new Vector3(58.21444F, 359.9872F, 0.12899F),
                    localScale = new Vector3(2.40581F, 2.22827F, 2.40581F)
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
                    localPos = new Vector3(-0.73058F, 0.11307F, -0.41461F),
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
                    childName = "Chest",
                    localPos = new Vector3(-0.01718F, 0.1667F, -0.28077F),
                    localAngles = new Vector3(288.5123F, 351.7566F, 11.24601F),
                    localScale = new Vector3(0.14446F, 0.21848F, 0.14009F)
                }
            });
            rules.Add("mdlTreebot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "FlowerBase",
                    localPos = new Vector3(0.02919F, 0.33675F, -0.00851F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.69771F, 0.69771F, 0.69771F)
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
                    localPos = new Vector3(-0.0569F, -0.00901F, -0.55248F),
                    localAngles = new Vector3(359.9369F, 0.46435F, 93.25299F),
                    localScale = new Vector3(0.01819F, 0.01819F, 0.01833F)
                }
            });
            rules.Add("mdlEngiTurret", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0F, 0.72996F, 0F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.46031F, 0.55077F, 0.50861F)
                }
            });
            rules.Add("mdlScav", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0.32969F, 5.07232F, 0.09527F),
                    localAngles = new Vector3(331.1071F, 180.0001F, 180F),
                    localScale = new Vector3(5.29656F, 5.29656F, 5.29656F)
                }
            });
            return rules;
        }

        public override void Hooks()
        {
            On.RoR2.CharacterBody.OnInventoryChanged += HandleBurningCtrl;
            //On.RoR2.CharacterBody.FixedUpdate += ApplyBuffAsIndicatorForReady;
            On.RoR2.GlobalEventManager.OnHitEnemy += FlamingSoul;
            On.RoR2.OverlapAttack.Fire += MeleeDrain;
            On.RoR2.BulletAttack.Fire += RangedDrain;
            //On.RoR2.GenericSkill.OnExecute += SkillDrain;
            GetStatCoefficients += GrantBaseRegen;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += ProjectileDrain;
        }

        public void CreateEffect()
        {
            FireEffect = R2API.PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/ExplosionSolarFlare"), "FireEffect");

            var fireEffectComponent = FireEffect.GetComponent<EffectComponent>();
            fireEffectComponent.soundName = "Melting_Pot_Flames";

            FireEffect.AddComponent<NetworkIdentity>();

            if (FireEffect) { R2API.PrefabAPI.RegisterNetworkPrefab(FireEffect); }
            EffectAPI.AddEffect(FireEffect);
        }

        private void GrantBaseRegen(CharacterBody sender, StatHookEventArgs args)
        {
            if (GetCount(sender) > 0)
            {
                args.regenMultAdd += 1;
            }
        }

        private void CreateDoTs()
        {
            DotController.DotDef sbDotDef = new DotController.DotDef {
                interval = 1f,
                damageCoefficient = 1,
                damageColorIndex = DamageColorIndex.DeathMark,
                associatedBuff = soulBurnBuff
            };
            soulBurnDot = DotAPI.RegisterDotDef(sbDotDef, (dotController, dotStack) =>
            {
                CharacterBody attackerBody = dotStack.attackerObject.GetComponent<CharacterBody>();
                if (attackerBody && attackerBody.gameObject.GetComponent<BurningSoulController>()) {
                    //float damageMultiplier = dmgCoefficient + dmgStack * (GetCount(attackerBody) - 1);
                    float burnDamage = attackerBody.gameObject.GetComponent<BurningSoulController>().getInflictedDmg();
                    dotStack.damage = burnDamage;
                    dotStack.damageType = DamageType.DoT;
                }
            });
            DotController.DotDef sbSelfDotDef = new DotController.DotDef {
                interval = 0.5f,
                damageCoefficient = 1f,
                damageColorIndex = DamageColorIndex.Item,
                associatedBuff = soulBurnSelfBuff
            };
            soulBurnSelfDot = DotAPI.RegisterDotDef(sbSelfDotDef, (dotController, dotStack) =>
            {
                CharacterBody attackerBody = dotStack.attackerObject.GetComponent<CharacterBody>();
                if (attackerBody && attackerBody.GetComponent<BurningSoulController>()) {
                    float burnDamage = (float)Math.Ceiling(attackerBody.GetComponent<BurningSoulController>().getSelfDmg());
                    dotStack.damage = burnDamage;
                    //dotStack.damageType = DamageType.BypassArmor;
                    dotStack.damageType = DamageType.NonLethal | DamageType.BypassArmor | DamageType.DoT | DamageType.Silent;
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

                                var soulCtrl = body.GetComponent<BurningSoulController>();
                                if (!soulCtrl) { soulCtrl = body.AddItemBehavior<BurningSoulController>(InventoryCount); }
                                if (soulCtrl.getSelfBurn() > burnTimeOut)
                                {
                                    cooldownHandler.MeleeTracker.Add(self, 0);

                                    BSModLogger.LogInfo($"Melting Pot -Melee- Applying DOT {soulBurnSelfDot}");
                                    soulCtrl.burnt = true;
                                }
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
                            var soulCtrl = body.GetComponent<BurningSoulController>();
                            if (!soulCtrl) { soulCtrl = body.AddItemBehavior<BurningSoulController>(InventoryCount); }
                            if (soulCtrl.getSelfBurn() > burnTimeOut)
                            {
                                BSModLogger.LogInfo($"Melting Pot -Projectile- Applying DOT {soulBurnSelfDot}");
                                soulCtrl.burnt = true;
                            }
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
                            var soulCtrl = body.GetComponent<BurningSoulController>();
                            if (!soulCtrl) { soulCtrl = body.AddItemBehavior<BurningSoulController>(InventoryCount); }
                            if (soulCtrl.getSelfBurn() > burnTimeOut)
                            {
                                BSModLogger.LogInfo($"Melting Pot -Skill- Applying DOT {soulBurnSelfDot}");
                                soulCtrl.burnt = true;
                            }
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
                            var soulCtrl = body.GetComponent<BurningSoulController>();
                            if (!soulCtrl) { soulCtrl = body.AddItemBehavior<BurningSoulController>(InventoryCount); }
                            if (soulCtrl.getSelfBurn() > burnTimeOut)
                            {
                                BSModLogger.LogInfo($"Melting Pot -Ranged- Applying DOT {soulBurnSelfDot}");
                                soulCtrl.burnt = true;
                            }
                        }
                    }
                }
            }
            orig(self);
        }

        /*[Command]
        void CmdApplyDot(GameObject target, DotController.DotIndex burnDot, float burnDuration)
        {
            DotController.InflictDot(target, target, burnDot, burnDuration);
        }*/

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
                                scale = 2f,
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