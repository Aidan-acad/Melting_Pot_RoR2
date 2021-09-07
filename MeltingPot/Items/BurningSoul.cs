using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;
using MeltingPot.Utils;
using static R2API.RecalculateStatsAPI;
using BepInEx;

namespace MeltingPot.Items
    {
    public class BurningSoul : ItemBase<BurningSoul>
    {
        public override string ItemName => "Burning Soul";
        
        public override string ItemLangTokenName => "BURNING_SOUL";

        public override string ItemPickupDesc => $"Doubles <style=cIsHealing>Health Regen</style>. Attacks <style=cIsDamage>burn</style> enemies for max health damage at the cost of your own";

        public override string ItemFullDescription => $"Increases <style=cIsHealing>health regen by 100%</style>. Once over <style=cIsHealth>1000 health</style>, burn enemies for <style=cIsDamage>4%</style><style=cStack>(+1.5% per stack)</style> of your <style=cIsHealth>Maximum Health</style> <style=cDeath>BUT</style> all attacks cost <style=cIsHealth>1%</style><style=cStack>(+0.5% per stack)</style> of your current health.";

        public override string ItemLore => "[Attached to this box is a strange note covered in letters cut from various sources.]\n\n" +
            "Hello there!\n\n" +

            "If you're reading this, then the mail service has done their job in sending this parcel to the right person. I just want you to know the following: Screw you! " +
            "Not only did you steal my job, you took almost all my possessions from me before fleeing to some deep sector of space and now I'm giving you what you forgot to take!\n\n" +

            "That's right, open up the package! See that? You probably did shortly before it went off, but now I imagine you're not reading this anymore if my device worked. If you're not the person I sent this to, and " +
            "you're only finding the note next to some poor schmuck covered in nails, they got what was coming to them. I've attached the blueprints on how I built this thing in a secret compartment inside the bottom " +
            "of the box.\n\n" +

            "Bury the body, take the design, and stay quiet about this. It can be our little secret.\n\n" +

            "Sincerely,\n" +
            "Jeb Labinsky";

        public override ItemTag[] ItemTags => new ItemTag[] { ItemTag.Damage };
        public override ItemTier Tier => ItemTier.Tier1;

        public override GameObject ItemModel => Resources.Load<GameObject>("Prefabs/PickupModels/PickupMystery");

        public override Sprite ItemIcon => Resources.Load<Sprite>("Textures/MiscIcons/texMysteryIcon");

        public static GameObject ItemBodyModelPrefab;
        public static RoR2.BuffDef BurningSoulActiveBuff;
        public GameObject SoulInflictor = new GameObject("Burning Soul Damage");
        float heldHealth = 0;
        float healthdamage = 0;


        public override void Init(ConfigFile config)
        {
            CreateLang();
            CreateItem();
            CreateBuff();
            Hooks();
        }


        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = ItemModel;
            ItemBodyModelPrefab.AddComponent<RoR2.ItemDisplay>();
            ItemBodyModelPrefab.GetComponent<RoR2.ItemDisplay>().rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
            rules.Add("mdlCommandoDualies", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(-0.16759F, -0.07591F, 0.06936F),
                    localAngles = new Vector3(343.2889F, 299.2036F, 176.8172F),
                    localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlHuntress", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(-0.14431F, -0.06466F, -0.03696F),
                    localAngles = new Vector3(355.1616F, 81.55997F, 180F),
                    localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlToolbot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(0.08787F, 0.07478F, 1.04472F),
                    localAngles = new Vector3(354.9749F, 182.8028F, 237.0256F),
                    localScale = new Vector3(0.5F, 0.5F, 0.5F)
                }
            });
            rules.Add("mdlEngi", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(-0.20102F, 0.09445F, 0.16025F),
                    localAngles = new Vector3(15.50638F, 144.8099F, 180.4037F),
                    localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlMage", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(-0.17241F, -0.0089F, 0.02642F),
                    localAngles = new Vector3(5.28933F, 111.5028F, 190.532F),
                    localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlMerc", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(0.16832F, 0.04282F, 0.06368F),
                    localAngles = new Vector3(355.8307F, 42.81982F, 185.1587F),
                    localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlTreebot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "FlowerBase",
                    localPos = new Vector3(-0.6845F, -0.60707F, -0.05308F),
                    localAngles = new Vector3(349.4037F, 73.89225F, 346.442F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlLoader", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(-0.2442F, 0.04122F, 0.01506F),
                    localAngles = new Vector3(22.73106F, 289.1799F, 159.5365F),
                    localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlCroco", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Hip",
                    localPos = new Vector3(-2.2536F, 1.10779F, 0.45293F),
                    localAngles = new Vector3(1.77184F, 278.9485F, 190.4101F),
                    localScale = new Vector3(0.5F, 0.5F, 0.5F)
                }
            });
            rules.Add("mdlCaptain", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(-0.21004F, -0.09095F, -0.09165F),
                    localAngles = new Vector3(0F, 60.43688F, 180F),
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
                    localPos = new Vector3(0.17925F, -0.02363F, -0.11047F),
                    localAngles = new Vector3(359.353F, 299.9855F, 169.6378F),
                    localScale = new Vector3(0.05F, 0.05F, 0.05F)
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
            R2API.RecalculateStatsAPI.GetStatCoefficients += GrantBaseRegen;
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
            var InventoryCount = GetCount(self);
            if (InventoryCount > 0)
            {
                if (self.healthComponent.fullHealth > 1000)
                {
                    self.AddBuff(BurningSoulActiveBuff);
                    if (instance.heldHealth != self.healthComponent.fullHealth)
                    {
                        instance.heldHealth = self.healthComponent.fullHealth;
                        instance.healthdamage = (float)(0.04 + (InventoryCount - 1) * 0.01) * instance.heldHealth;
                    }
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
                if (self.HasBuff(BurningSoulActiveBuff))
                {
                    self.RemoveBuff(BurningSoulActiveBuff);
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
        }

        private bool MeleeDrain(On.RoR2.OverlapAttack.orig_Fire orig, RoR2.OverlapAttack self, List<RoR2.HurtBox> hitResults)
            {

            var owner = self.inflictor;
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
                            var soul = body.GetComponent<RoR2.HealthComponent>();
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
                            soul.TakeDamage(di);
                        }
                    }
                }
            }
            return orig(self, hitResults);
        }

        private void RangedDrain(On.RoR2.BulletAttack.orig_Fire orig, RoR2.BulletAttack self)
        {
            var owner = self.owner;
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
                            var soul = body.GetComponent<RoR2.HealthComponent>();
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
                            soul.TakeDamage(di);
                        }
                    }
                }
            }
            orig(self);
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
                    var victimBody = victim.GetComponent<HealthComponent>();
                    if (body && victimBody)
                    {
                        var InventoryCount = GetCount(body);
                        if (InventoryCount > 0)
                        {
                            DamageInfo di = new DamageInfo
                            {
                                position = victimBody.body.corePosition,
                                attacker = attacker,
                                inflictor = attacker,
                                crit = damageInfo.crit,
                                damage = damageInfo.damage/body.damage * instance.healthdamage,
                                damageColorIndex = DamageColorIndex.DeathMark,
                                damageType = DamageType.DoT,
                                force = Vector3.zero,
                                procCoefficient = 0,
                                procChainMask = default(ProcChainMask)
                            };
                            victimBody.TakeDamage(di);
                        }
                    }
                }
                orig(self, damageInfo, victim);
            }
        }
    }