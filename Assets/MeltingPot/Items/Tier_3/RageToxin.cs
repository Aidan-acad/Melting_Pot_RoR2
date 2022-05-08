using BepInEx.Configuration;
using MeltingPot.Utils;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static MeltingPot.MeltingPotPlugin;

namespace MeltingPot.Items
{
    class RageToxin : ItemBase<RageToxin>
    {
        public override string ItemName => "Rage Toxin";
        public override string ItemLangTokenName => "RAGETOXIN";

        public static float baseDuration = 2.0f;
        public static float durationGrowth = 1.0f;
        public static float fumbleChance = 0.2f;
        public static float fumbleGrowth = 0.1f;
        public override string ItemPickupDesc => $"Hit enemies to <style=cIsUtility>taunt them</style> into attacking you.";

        public override string ItemFullDescription =>
            $"Hitting enemies forces them to target you for {baseDuration} seconds <style=cStack>(+{durationGrowth}s per stack)</style>. Enraged enemies <style=cIsDamage>attack faster</style>, but <style=cIsDamage>deal reduced damage</style> and have a {fumbleChance * 100}% <style=cStack>(+{fumbleGrowth * 100}% per stack)</style> chance to <style=cIsUtility>fumble attacks</style>.";

        public override string ItemLore =>
            "Take care with application of ShasCo. Rage Toxin. Keep out of reach of children.\n\n"
            + "Symptoms in users under 5 years of age can often be mistaken for demonic possession.\n\n"
            + "If ingested, be careful not to make eye contact with any essential persons.";
        public static BepInEx.Logging.ManualLogSource BSModLogger;

        public override string VoidCounterpart => null;
        public static GameObject ItemModel;

        public GameObject Fumble_eff; // => MainAssets.LoadAsset<GameObject>("Fumble_MSG.prefab");

        public static GameObject ItemBodyModelPrefab;
        public static BuffDef RageBuff =>
            ContentPackProvider.contentPack.buffDefs.Find("MeltingPot_Enraged");

        private class AggroTracker : MonoBehaviour
        {
            private GameObject Target;
            private float baseDmg;
            public int severity;

            public AggroTracker(GameObject tgt, int stacks)
            {
                this.Target = tgt;
                this.baseDmg = this.gameObject.GetComponent<CharacterBody>().damage;
                this.severity = stacks;
            }

            public void UpdateTarget(GameObject tgt)
            {
                this.Target = tgt;
            }

            public GameObject getTarget()
            {
                return this.Target;
            }

            public float getBaseDmg()
            {
                return this.baseDmg;
            }
        }

        public override void Init(ConfigFile config, bool enabled)
        {
            CreateItem("RageToxin_ItemDef", enabled);
            if (enabled)
            {
                ItemModel = Assets.mainAssetBundle.LoadAsset<GameObject>(
                    $"{ModelPath}/rage_toxin/RageToxin.prefab"
                );
                CreateLang();
                CreateEffect();
                Hooks();
            }
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>(
                $"{ModelPath}/rage_toxin/displayragetoxin.prefab"
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
                        childName = "Pelvis",
                        localPos = new Vector3(0.07683F, 0.01288F, 0.14806F),
                        localAngles = new Vector3(346.4977F, 181.9675F, 174.1487F),
                        localScale = new Vector3(0.03082F, 0.03082F, 0.03082F)
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
                        childName = "UpperArmR",
                        localPos = new Vector3(0.01489F, 0.23606F, -0.08481F),
                        localAngles = new Vector3(8.21882F, 96.5443F, 188.1066F),
                        localScale = new Vector3(0.02374F, 0.02374F, 0.02374F)
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
                        childName = "LowerArmL",
                        localPos = new Vector3(0.2543F, 1.98972F, 0.61059F),
                        localAngles = new Vector3(86.41363F, 131.8821F, 124.6506F),
                        localScale = new Vector3(0.37236F, 0.37236F, 0.37236F)
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
                        childName = "MuzzleRight",
                        localPos = new Vector3(-0.20996F, -0.2132F, -0.14212F),
                        localAngles = new Vector3(4.97284F, 178.6752F, 43.88905F),
                        localScale = new Vector3(0.04209F, 0.03512F, 0.04537F)
                    },
                    new RoR2.ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemBodyModelPrefab,
                        childName = "MuzzleLeft",
                        localPos = new Vector3(0.21646F, -0.21331F, -0.13851F),
                        localAngles = new Vector3(357.656F, 175.7056F, 315.5557F),
                        localScale = new Vector3(0.04209F, 0.03512F, 0.04537F)
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
                        localPos = new Vector3(-0.11061F, 0.014F, -0.25917F),
                        localAngles = new Vector3(7.73172F, 0.49687F, 359.7209F),
                        localScale = new Vector3(0.04639F, 0.04419F, 0.05004F)
                    },
                    new RoR2.ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemBodyModelPrefab,
                        childName = "Chest",
                        localPos = new Vector3(0.11884F, 0.01085F, -0.25877F),
                        localAngles = new Vector3(7.73172F, 0.49687F, 359.7209F),
                        localScale = new Vector3(0.04639F, 0.04419F, 0.05004F)
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
                        childName = "Head",
                        localPos = new Vector3(0.19021F, 0.07139F, 0.05686F),
                        localAngles = new Vector3(0F, 0.00001F, 90F),
                        localScale = new Vector3(0.01414F, 0.01517F, 0.01713F)
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
                        childName = "WeaponPlatform",
                        localPos = new Vector3(0.00421F, -0.7153F, 0.42418F),
                        localAngles = new Vector3(0F, 0F, 0F),
                        localScale = new Vector3(0.12F, 0.08973F, 0.12F)
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
                        localPos = new Vector3(0.15324F, 0.43758F, -0.31688F),
                        localAngles = new Vector3(351.5648F, 0.00351F, 89.94431F),
                        localScale = new Vector3(0.07161F, 0.0333F, 0.08068F)
                    },
                    new RoR2.ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemBodyModelPrefab,
                        childName = "Chest",
                        localPos = new Vector3(-0.16197F, 0.4376F, -0.31893F),
                        localAngles = new Vector3(335.5402F, 179.2124F, 90.17084F),
                        localScale = new Vector3(0.07161F, 0.0333F, 0.08068F)
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
                        childName = "Head",
                        localPos = new Vector3(1.38078F, 0.0539F, 2.76478F),
                        localAngles = new Vector3(290.6895F, 59.03379F, 325.3782F),
                        localScale = new Vector3(0.56991F, 0.49455F, 0.5366F)
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
                        childName = "Head",
                        localPos = new Vector3(-0.22376F, 0.18947F, -0.10258F),
                        localAngles = new Vector3(41.81681F, 190.8585F, 144.7464F),
                        localScale = new Vector3(0.01959F, 0.02774F, 0.01939F)
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
                        childName = "Chest",
                        localPos = new Vector3(0.13592F, 0.28842F, -0.2076F),
                        localAngles = new Vector3(351.8562F, 77.64908F, 293.4286F),
                        localScale = new Vector3(0.02111F, 0.02527F, 0.02334F)
                    },
                    new RoR2.ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemBodyModelPrefab,
                        childName = "Chest",
                        localPos = new Vector3(-0.13432F, 0.28295F, -0.1966F),
                        localAngles = new Vector3(341.4966F, 106.154F, 291.0656F),
                        localScale = new Vector3(0.02111F, 0.02527F, 0.02334F)
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
                        localPos = new Vector3(-0.72032F, -0.22506F, -1.74434F),
                        localAngles = new Vector3(0F, 0F, 0F),
                        localScale = new Vector3(0.07053F, 0.08439F, 0.07793F)
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
                        childName = "Weapon",
                        localPos = new Vector3(0.00038F, 24.18241F, -0.07813F),
                        localAngles = new Vector3(-0.00001F, 180F, 180F),
                        localScale = new Vector3(2.36714F, 3.15618F, 2.36714F)
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
                        childName = "GunRoot",
                        localPos = new Vector3(0.30922F, -0.14979F, -0.03557F),
                        localAngles = new Vector3(0F, 0F, 90F),
                        localScale = new Vector3(0.02646F, 0.02646F, 0.02646F)
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
                        childName = "ThighL",
                        localPos = new Vector3(0.04174F, 0.36401F, -0.20286F),
                        localAngles = new Vector3(357.7899F, 260.6965F, 270.743F),
                        localScale = new Vector3(0.04292F, 0.04292F, 0.04292F)
                    }
                }
            );
            return rules;
        }

        public override void Hooks()
        {
            On.RoR2.GlobalEventManager.OnHitEnemy += Aggro;
            On.RoR2.CharacterAI.BaseAI.FixedUpdate += AggroApply;
            On.RoR2.CharacterBody.OnSkillActivated += AggroSquelch;
        }

        private void AggroSquelch(
            On.RoR2.CharacterBody.orig_OnSkillActivated orig,
            global::RoR2.CharacterBody self,
            global::RoR2.GenericSkill skill
        )
        {
            //BSModLogger = ModLogger;
            try
            {
                var aggroTracker = self.gameObject.GetComponent<AggroTracker>();
                if (self.HasBuff(RageBuff) && aggroTracker)
                {
                    //BSModLogger.LogInfo($"Attack Primed -> {self.name}");
                    var rand = new System.Random();
                    var res = rand.Next(1, 11);

                    if (
                        Util.CheckRoll(
                            1 - 1 / (1 + fumbleChance + fumbleGrowth * (aggroTracker.severity - 1))
                        )
                    )
                    {
                        //BSModLogger.LogInfo($"Squelching attack of -> {self.name}");
                        EffectData effectData = new EffectData
                        {
                            origin = self.corePosition + Vector3.up * 2f,
                        };
                        EffectManager.SpawnEffect(Fumble_eff, effectData, true);
                        RoR2.SetStateOnHurt.SetStunOnObject(self.gameObject, 0.1f);
                        //self.damage = 0f;
                    }
                    else
                    {
                        //self.damage = aggroTracker.getBaseDmg();
                    }
                }
            }
            catch
            {
                // Lets hope this works
            }
            orig(self, skill);
        }

        private void AggroApply(
            On.RoR2.CharacterAI.BaseAI.orig_FixedUpdate orig,
            global::RoR2.CharacterAI.BaseAI self
        )
        {
            orig(self);
            try
            {
                if (self.gameObject && self.body)
                {
                    var aggroTracker = self.gameObject.GetComponent<AggroTracker>();
                    if (self.body.HasBuff(RageBuff) && aggroTracker)
                    {
                        self.currentEnemy.gameObject = aggroTracker.getTarget();
                        self.currentEnemy.bestHurtBox = RoR2.Util.FindBodyMainHurtBox(
                            self.currentEnemy.gameObject.GetComponent<CharacterBody>()
                        );
                        self.enemyAttention = 1f;
                        self.body.damage = self.body.baseDamage / 4;
                        self.body.attackSpeed = self.body.baseAttackSpeed * 2;
                        self.body.armor = self.body.baseArmor - 40;
                    }
                }
            }
            catch
            {
                // Hope shit dont get funky
                BSModLogger = ModLogger;
                BSModLogger.LogInfo($"Failed Applying aggro to -> {self.name}");
                return;
            }
        }

        private void Aggro(
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
                var InventoryCount = GetCount(damageInfo.attacker.GetComponent<CharacterBody>());
                if (InventoryCount > 0)
                {
                    //BSModLogger.LogInfo($"Victim and attacker exist");
                    try
                    {
                        var vBody = victim.GetComponent<CharacterBody>();
                        var duration = baseDuration + durationGrowth * (InventoryCount - 1);
                        vBody.AddTimedBuff(RageBuff, duration);
                        var aggroTracker = vBody.GetComponent<AggroTracker>();
                        if (!aggroTracker)
                        {
                            aggroTracker = vBody.gameObject.AddComponent<AggroTracker>();
                        }
                        aggroTracker.UpdateTarget(damageInfo.attacker);
                    }
                    catch
                    {
                        BSModLogger.LogError(
                            "Failure in characterbody assignment, issues with hurtboxes?"
                        );
                    }
                }
            }
            orig(self, damageInfo, victim);
        }

        private static GameObject LoadEffect(
            string resourceName,
            string soundName,
            bool parentToTransform
        )
        {
            GameObject newEffect = Assets.mainAssetBundle.LoadAsset<GameObject>(resourceName);

            newEffect.AddComponent<DestroyOnTimer>().duration = 0.5f;
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

        public void CreateEffect()
        {
            Fumble_eff = LoadEffect(
                "assets/meltingpot/mpassets/Effects/Fumble_MSG.prefab",
                "",
                false
            );

            if (Fumble_eff)
            {
                PrefabAPI.RegisterNetworkPrefab(Fumble_eff);
            }
            ContentAddition.AddEffect(Fumble_eff);
        }
    }
}
