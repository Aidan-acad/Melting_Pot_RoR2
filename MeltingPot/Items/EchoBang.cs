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
	class EchoBang: ItemBase<EchoBang>
	{
        public override string ItemName => "Reverb Ring";
        public override string ItemLangTokenName => "REVERB_RING";

        public override string ItemPickupDesc => $"Discharges a blast 0.5s after an attack for <style=cStack>20% (+5% per stack)</style> of the damage in a <style=cStack>10m (+2.5m per stack)</style> radius";

        public override string ItemFullDescription => $"Discharges a blast 0.5s after an attack for <style=cStack>20% (+5% per stack)</style> of the damage in a <style=cStack>10m (+2.5m per stack)</style> radius";

        public override string ItemLore => "[A faint reverb can be heard emenating from the core of the ring.]\n\n" +
            "Echoes of your memories ring out as you touch the ring's surface, voices of forgotten friends fading in the light.";

        public override ItemTag[] ItemTags => new ItemTag[] { ItemTag.Damage };
        public override ItemTier Tier => ItemTier.Tier2;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("ReverbRing.prefab");
        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("reverbRing_ICON.png");

        public GameObject ItemEffect => MainAssets.LoadAsset<GameObject>("EchoRing.prefab");

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
                (float, float, int) echo = (damage, 1f, count-1);
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
                            radius = 10f + (echo.Item3 * 2.5f),
                            baseForce = 25f,
                            bonusForce = Vector3.up * 2f,
                            baseDamage = echo.Item1 * (0.2f + (0.1f * echo.Item3)),
                            falloffModel = BlastAttack.FalloffModel.Linear,
                            crit = Util.CheckRoll(body.crit, body.master),
                            damageColorIndex = DamageColorIndex.Item,
                            attackerFiltering = AttackerFiltering.NeverHit
                        };
                        blastAttack.Fire();
                        AkSoundEngine.PostEvent(2600490428, body.gameObject);
                        /*EffectData effectData = new EffectData {
                            origin = body.corePosition,
                            scale = 1f,
                        };
                        EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/JellyfishNova"), effectData, true);*/
                        EffectData effectData = new EffectData {
                            origin = body.corePosition,
                            scale = 20f + (echo.Item3 * 5f),
                        };
                        EffectManager.SpawnEffect(EchoEffect, effectData, true);
                        //BSModLogger.LogInfo("Firing Echo");
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

        public override void Init(ConfigFile config) {
            CreateItem();
            CreateLang();
            CreateEffect();
            Hooks();
        }


        public override ItemDisplayRuleDict CreateItemDisplayRules() {
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

        public override void Hooks() {
            //On.RoR2.GenericSkill.OnExecute += SkillEcho;
            On.RoR2.CharacterBody.OnInventoryChanged += HandleEchoCtrl;
            On.RoR2.OverlapAttack.Fire += MeleeEcho;
            On.RoR2.BulletAttack.Fire += RangedEcho;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += ProjectileEcho;
        }

        private void SkillEcho(On.RoR2.GenericSkill.orig_OnExecute orig, global::RoR2.GenericSkill self) {
            orig(self);
            var owner = self.characterBody;
            if (owner) {
                var body = owner;
                if (body) {
                    var InventoryCount = GetCount(body);
                    if (InventoryCount > 0) {
                    }
                }
            }
        }

        private void ProjectileEcho(On.RoR2.Projectile.ProjectileManager.orig_FireProjectile_FireProjectileInfo orig, RoR2.Projectile.ProjectileManager self, RoR2.Projectile.FireProjectileInfo fireProjectileInfo) {
            orig(self, fireProjectileInfo);
            var owner = fireProjectileInfo.owner;
            if (owner) {
                var body = owner.GetComponent<RoR2.CharacterBody>();
                if (body) {
                    var InventoryCount = GetCount(body);
                    if (InventoryCount > 0) {
                        var echoCtrl = owner.GetComponent<EchoController>();
                        if (!echoCtrl) {return; }
                        echoCtrl.queueEcho(fireProjectileInfo.damage, InventoryCount);
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

        private static GameObject LoadEffect(string resourceName, string soundName, bool parentToTransform) {

            GameObject newEffect = MainAssets.LoadAsset<GameObject>(resourceName);

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
            EchoEffect = LoadEffect("EchoRing.prefab", "", false);

            if (EchoEffect) { PrefabAPI.RegisterNetworkPrefab(EchoEffect); }
            EffectAPI.AddEffect(EchoEffect);
        }
    }
}
