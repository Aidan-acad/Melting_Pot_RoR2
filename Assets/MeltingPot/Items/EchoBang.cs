using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using MeltingPot.Utils;
using static MeltingPot.MeltingPotPlugin;
using System.Linq;
using UnityEngine.Networking;

namespace MeltingPot.Items
{
	class EchoBang: ItemBase<EchoBang>
	{
        private static float fireDelay = 0.5f;
        public static float scaling = 0.2f;
        public static float radiusGrowth = 2.5f;
        public static float baseRadius = 10f;
        public override string ItemName => "Reverb Ring";
        public override string ItemLangTokenName => "REVERBRING";

        public override string ItemPickupDesc => $"Discharges a blast {fireDelay}s after an attack for <style=cStack>{scaling * 100}% (+{scaling*100}% per stack)</style> of the damage in a <style=cStack>{baseRadius}m (+{radiusGrowth}m per stack)</style> radius";

        public override string ItemFullDescription => $"Discharges a blast {fireDelay}s after an attack for <style=cStack>{scaling * 100}% (+{scaling*100}% per stack)</style> of the damage in a <style=cStack>{baseRadius}m (+{radiusGrowth}m per stack)</style> radius";

        public override string ItemLore => "[A faint reverb can be heard emenating from the core of the ring.]\n\n" +
            "Echoes of your memories ring out as you touch the ring's surface, voices of forgotten friends fading in the light.";


        public override string VoidCounterpart => null;
        public GameObject ItemModel => Assets.mainAssetBundle.LoadAsset<GameObject>("assets/meltingpot/mpassets/itemprefabs/reverb_ring/reverbring.prefab");

        public static GameObject ItemBodyModelPrefab;
        public static GameObject EchoEffect;
        public static BepInEx.Logging.ManualLogSource BSModLogger;

        private class EchoController : CharacterBody.ItemBehavior
        {
            private List<(float, float, int)> StoredEchos = new List<(float,float,int)>();
            public void Awake() {
                this.body = base.gameObject.GetComponent<CharacterBody>();
            }

            public void queueEcho(float damage, int count) {
                BSModLogger = ModLogger;
                (float, float, int) echo = (damage, fireDelay, count-1);
                this.StoredEchos.Add(echo);
                //BSModLogger.LogInfo("Echo queued");
                this.body.statsDirty = true;
            }

            public void FixedUpdate() {
                BSModLogger = ModLogger;
                StoredEchos = StoredEchos.Select(c => { c.Item2 -= Time.fixedDeltaTime; return c; }).ToList();
                var FiringEchos = GetEchos();
                foreach ((float, float, int) echo in FiringEchos) {
                    try {
                        BlastAttack blastAttack = new BlastAttack {
                            attacker = body.gameObject,
                            inflictor = body.gameObject,
                            teamIndex = TeamComponent.GetObjectTeam(body.gameObject),
                            position = body.corePosition,
                            procCoefficient = 0f,
                            radius = baseRadius + (echo.Item3 * radiusGrowth),
                            baseForce = 25f,
                            bonusForce = Vector3.up * 2f,
                            baseDamage = echo.Item1 * (scaling + (scaling * echo.Item3)),
                            falloffModel = BlastAttack.FalloffModel.Linear,
                            crit = Util.CheckRoll(body.crit, body.master),
                            damageColorIndex = DamageColorIndex.Item,
                            attackerFiltering = AttackerFiltering.NeverHitSelf
                        };
                        blastAttack.Fire();
                        //AkSoundEngine.PostEvent(2600490428, body.gameObject);
                        //Util.PlaySound("EchoReverbPlay", body.gameObject);
                        EffectData effectData = new EffectData {
                            origin = body.corePosition,
                            scale = 2*baseRadius + (echo.Item3 * radiusGrowth) * 2,
                        };
                        EffectManager.SpawnEffect(EchoEffect, effectData, true);
                    }
					catch {
                        BSModLogger.LogInfo("Failed to Fire Echo");
					}
                }
                StoredEchos.RemoveAll(c => {return c.Item2 < 0f;});
            }

            private List<(float, float, int)> GetEchos() {
                var primedEchos = StoredEchos.Where(w => w.Item2 < 0f);

                return primedEchos.ToList();
            }

        }

        private void HandleEchoCtrl(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, global::RoR2.CharacterBody self) {

            self.AddItemBehavior<EchoController>(base.GetCount(self));
            orig(self);
        }

        public override void Init(ConfigFile config, bool enabled) {
            CreateItem("ReverbRing_ItemDef", enabled);
            if (enabled) {
                CreateLang();
                CreateEffect();
                Hooks();
            }
        }


        public override ItemDisplayRuleDict CreateItemDisplayRules() {
            ItemBodyModelPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>("assets/meltingpot/mpassets/itemprefabs/reverb_ring/displayreverbring.prefab");
            Vector3 generalScale = new Vector3(1f, 1f, 1f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
            rules.Add("mdlCommandoDualies", new RoR2.ItemDisplayRule[]
            {
            new RoR2.ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "LeftJet",
                localPos = new Vector3(0.00452F, 0.00459F, 0.00347F),
                localAngles = new Vector3(305.6413F, 57.161F, 278.5891F),
                localScale = new Vector3(0.05356F, 0.05356F, 0.05356F)
            },
            new RoR2.ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "RightJet",
                localPos = new Vector3(0.01635F, -0.00389F, 0.00694F),
                localAngles = new Vector3(305.6413F, 57.16099F, 283.5067F),
                localScale = new Vector3(0.05356F, 0.05356F, 0.05356F)
            }
            });
            rules.Add("mdlHuntress", new RoR2.ItemDisplayRule[]
            {
            new RoR2.ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Muzzle",
                localPos = new Vector3(-0.00057F, -0.02425F, -0.02508F),
                localAngles = new Vector3(284.2652F, 5.62172F, 333.7986F),
                localScale = new Vector3(0.03809F, 0.03809F, 0.03809F)
            }
            });
            rules.Add("mdlToolbot", new RoR2.ItemDisplayRule[]
            {
            new RoR2.ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "MainWheelR",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(2.96075F, 16.36797F, 73.31649F),
                localScale = new Vector3(1.0242F, 1.0242F, 1.0242F)
            },
            new RoR2.ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "MainWheelL",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(2.96075F, 16.36797F, 73.31649F),
                localScale = new Vector3(1.0242F, 1.0242F, 1.0242F)
            }
            });
            rules.Add("mdlEngi", new RoR2.ItemDisplayRule[]
            {
            new RoR2.ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "HandL",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 327.7332F),
                localScale = new Vector3(0.08404F, 0.07013F, 0.09059F)
            }
            });
            rules.Add("mdlMage", new RoR2.ItemDisplayRule[]
            {
            new RoR2.ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(0.00369F, 0.31456F, -0.08522F),
                localAngles = new Vector3(15.25314F, 1.71923F, 342.6355F),
                localScale = new Vector3(0.12123F, 0.11548F, 0.13077F)
            }
            });
            rules.Add("mdlMerc", new RoR2.ItemDisplayRule[]
            {
            new RoR2.ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(-0.00079F, 0.18529F, 0.16395F),
                localAngles = new Vector3(75.19482F, 207.9967F, 179.2638F),
                localScale = new Vector3(0.02F, 0.02F, 0.02F)
            }
            });
            rules.Add("mdlTreebot", new RoR2.ItemDisplayRule[]
            {
            new RoR2.ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "MuzzleSyringe",
                localPos = new Vector3(-0.00062F, -0.09426F, -0.67915F),
                localAngles = new Vector3(285.4125F, 343.9919F, 353.6093F),
                localScale = new Vector3(0.09791F, 0.09791F, 0.09791F)
            }
            });
            rules.Add("mdlLoader", new RoR2.ItemDisplayRule[]
            {
            new RoR2.ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "FootR",
                localPos = new Vector3(0.00683F, -0.0432F, -0.00979F),
                localAngles = new Vector3(51.55743F, 5.87579F, 346.787F),
                localScale = new Vector3(0.07998F, 0.07998F, 0.07998F)
            }
            });
            rules.Add("mdlCroco", new RoR2.ItemDisplayRule[]
            {
            new RoR2.ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(0F, -0.75966F, -0.45724F),
                localAngles = new Vector3(25.33484F, 17.02349F, 340.3375F),
                localScale = new Vector3(1.82282F, 1.82282F, 1.82282F)
            }
            });
            rules.Add("mdlCaptain", new RoR2.ItemDisplayRule[]
            {
            new RoR2.ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Pelvis",
                localPos = new Vector3(-0.05528F, -0.11759F, -0.17834F),
                localAngles = new Vector3(292.9919F, 355.4008F, 353.8277F),
                localScale = new Vector3(0.05F, 0.05F, 0.05F)
            }
            });
            rules.Add("mdlBandit2", new RoR2.ItemDisplayRule[]
            {
            new RoR2.ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Pelvis",
                localPos = new Vector3(-0.01769F, -0.0691F, -0.03464F),
                localAngles = new Vector3(23.09562F, 346.2865F, 341.3144F),
                localScale = new Vector3(0.2F, 0.2F, 0.2F)
            }
            });
            rules.Add("mdlEngiTurret", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Base",
                    localPos = new Vector3(0F, 1.19592F, 0F),
                    localAngles = new Vector3(17.30171F, 1.57215F, 338.808F),
                    localScale = new Vector3(0.28784F, 0.34441F, 0.31805F)
                }
            });
            rules.Add("mdlScav", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Stomach",
                    localPos = new Vector3(-5.12259F, -0.9507F, -8.53168F),
                    localAngles = new Vector3(317.3377F, 294.7297F, 49.32809F),
                    localScale = new Vector3(1.5F, 1.5F, 1.5F)
                }
            });
            rules.Add("mdlRailGunner", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "FootL",
                    localPos = new Vector3(0.0272F, -0.09449F, -0.08415F),
                    localAngles = new Vector3(39.98871F, 0.0054F, 0.0046F),
                    localScale = new Vector3(0.09149F, 0.09149F, 0.09149F)
                }
            });
            rules.Add("mdlVoidSurvivor", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ForeArmL",
                    localPos = new Vector3(0.00689F, 0.24979F, 0.00478F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.09718F, 0.09718F, 0.09718F)
                }
            });
            return rules;
        }

        public override void Hooks() {
            //On.RoR2.GenericSkill.OnExecute += SkillEcho;
            On.RoR2.CharacterBody.OnInventoryChanged += HandleEchoCtrl;
            On.RoR2.OverlapAttack.Fire += MeleeEcho;
            On.RoR2.BulletAttack.Fire += RangedEcho;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += ProjectileEcho;
        }

        /*private void SkillEcho(On.RoR2.GenericSkill.orig_OnExecute orig, global::RoR2.GenericSkill self) {
            orig(self);
            var owner = self.characterBody;
            if (owner) {
                var body = owner;
                if (body) {
                    var InventoryCount = GetCount(body);
                    if (InventoryCount > 0) {
                        var echoCtrl = owner.GetComponent<EchoController>();
                        if (!echoCtrl) { return; }
                        echoCtrl.queueEcho(self., InventoryCount);
                    }
                }
            }
        }*/

        private void ProjectileEcho(On.RoR2.Projectile.ProjectileManager.orig_FireProjectile_FireProjectileInfo orig, RoR2.Projectile.ProjectileManager self, RoR2.Projectile.FireProjectileInfo fireProjectileInfo) {
            orig(self, fireProjectileInfo);
            if (fireProjectileInfo.procChainMask.mask < 1) {
                var owner = fireProjectileInfo.owner;
                if (owner) {
                    var body = owner.GetComponent<RoR2.CharacterBody>();
                    if (body) {
                        var InventoryCount = GetCount(body);
                        if (InventoryCount > 0) {
                            var echoCtrl = owner.GetComponent<EchoController>();
                            if (!echoCtrl) { return; }
                            echoCtrl.queueEcho(fireProjectileInfo.damage, InventoryCount);
                        }
                    }
                }
            }
        }

        private bool MeleeEcho(On.RoR2.OverlapAttack.orig_Fire orig, RoR2.OverlapAttack self, List<RoR2.HurtBox> hitResults) {
            var owner = self.attacker;
            if (owner) {
                var body = owner.GetComponent<RoR2.CharacterBody>();
                if (body) {
                    var InventoryCount = GetCount(body);
                    if (InventoryCount > 0) {

                        var cooldownHandler = owner.GetComponent<MeleeCooldownHandler>();
                        if (!cooldownHandler) { cooldownHandler = owner.AddComponent<MeleeCooldownHandler>(); }
                        if (!cooldownHandler.MeleeTracker.ContainsKey(self)) {
                            var echoCtrl = owner.GetComponent<EchoController>();
                            if (!echoCtrl) { return orig(self, hitResults); }
                            cooldownHandler.MeleeTracker.Add(self, 0);
                            echoCtrl.queueEcho(self.damage, InventoryCount);
                        }
                    }
                }
            }
            return orig(self, hitResults);
        }

        public class MeleeCooldownHandler : MonoBehaviour
        {
            public Dictionary<RoR2.OverlapAttack, float> MeleeTracker = new Dictionary<RoR2.OverlapAttack, float>();

            public void FixedUpdate() {
                foreach (RoR2.OverlapAttack attack in MeleeTracker.Keys.ToList()) {
                    var time = MeleeTracker[attack];
                    time += Time.fixedDeltaTime;

                    if (time > 5) {
                        MeleeTracker.Remove(attack);
                    } else {
                        MeleeTracker[attack] = time;
                    }
                }
            }
        }

        private void RangedEcho(On.RoR2.BulletAttack.orig_Fire orig, RoR2.BulletAttack self) {
            orig(self);
            var owner = self.owner;
            if (owner) {
                var body = owner.GetComponent<RoR2.CharacterBody>();
                if (body) {
                    var InventoryCount = GetCount(body);
                    if (InventoryCount > 0) {
                        var echoCtrl = owner.GetComponent<EchoController>();
                        if (!echoCtrl) { return; }
                        echoCtrl.queueEcho(self.damage, InventoryCount);
                    }
                }
            }
        }

        private static GameObject LoadEffect(string soundName, bool parentToTransform) {

            GameObject newEffect = Assets.mainAssetBundle.LoadAsset<GameObject>("assets/meltingpot/mpassets/effects/EchoRing.prefab");

            newEffect.AddComponent<DestroyOnTimer>().duration = 12;
            newEffect.AddComponent<NetworkIdentity>();
            newEffect.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            var effect = newEffect.AddComponent<EffectComponent>();
            effect.applyScale = true;
            effect.effectIndex = EffectIndex.Invalid;
            effect.parentToReferencedTransform = parentToTransform;
            effect.positionAtReferencedTransform = true;
            effect.soundName = soundName;

            return newEffect;
        }

        public void CreateEffect() {
            EchoEffect = LoadEffect("EchoReverbPlay", false);

            if (EchoEffect) { PrefabAPI.RegisterNetworkPrefab(EchoEffect); }
            ContentAddition.AddEffect(EchoEffect);


        }
    }
}
