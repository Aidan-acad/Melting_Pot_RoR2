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
using static MeltingPot.MeltingPotPlugin;

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
        public override ItemTier Tier => ItemTier.Lunar;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("BurningSoulRedux.prefab");
        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("BurningSoul_Icon.png");

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
                    childName = "HeadCenter",
                    localPos = new Vector3(0F, 0.5F,0F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = generalScale
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
                    localPos = new Vector3(0F, 0.5F,0F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = generalScale
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