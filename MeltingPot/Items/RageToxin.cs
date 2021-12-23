using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using MeltingPot.Utils;
using static MeltingPot.MeltingPotPlugin;
using UnityEngine.Networking;
namespace MeltingPot.Items
{
	class RageToxin: ItemBase<RageToxin>
	{
        public override string ItemName => "Rage Toxin";
        public override string ItemLangTokenName => "Rage_Toxin";

        public override string ItemPickupDesc => $"Hit enemies to taunt them into attacking you";

        public override string ItemFullDescription => $"Hitting enemies forces them to target you for <style=cStack>1s + (1s per stack)</style>. Enraged enemies attack faster, but deal reduced damage and have a chance to fumble attacks";

        public override string ItemLore => "[A warning label on the side of the tube]\n\n" +
            "Take care with application of ShasCo. Rage Toxin. Keep out of reach of children.\n\n" +
            "Symptoms in users under 5 years of age can often be mistaken for demonic possession.\n\n" +
            "If ingested be careful not to make eye contact with any essential persons.";
        public static BepInEx.Logging.ManualLogSource BSModLogger;

        public override ItemTag[] ItemTags => new ItemTag[] { ItemTag.Utility };
        public override ItemTier Tier => ItemTier.Tier3;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("RageToxin.prefab");
        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("rageToxin_Icon.png");

        public GameObject Fumble_eff;// => MainAssets.LoadAsset<GameObject>("Fumble_MSG.prefab");

        public static GameObject ItemBodyModelPrefab;
        public static RoR2.BuffDef RageBuff;


        private void CreateBuff() {

            RageBuff = ScriptableObject.CreateInstance<BuffDef>();
            RageBuff.name = "Melting Pot: Enraged";
            RageBuff.buffColor = Color.white;
            RageBuff.canStack = false;
            RageBuff.isDebuff = false;
            RageBuff.iconSprite = MainAssets.LoadAsset<Sprite>("rageToxin_Aggro.PNG");

            BuffAPI.Add(new CustomBuff(RageBuff));

        }

        private class AggroTracker: MonoBehaviour
		{
            private GameObject Target;
            private float baseDmg;

            public AggroTracker(GameObject tgt) {
                this.Target = tgt;
                this.baseDmg = this.gameObject.GetComponent<CharacterBody>().damage;
			}

            public void UpdateTarget(GameObject tgt) {
                this.Target = tgt;
			}

            public GameObject getTarget() {
                return this.Target;
			}

            public float getBaseDmg() {
                return this.baseDmg;
			}
		}

        public override void Init(ConfigFile config) {
            CreateLang();
            CreateBuff();
            CreateItem();
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
            On.RoR2.GlobalEventManager.OnHitEnemy += Aggro;
            On.RoR2.CharacterAI.BaseAI.FixedUpdate += AggroApply;
            On.RoR2.CharacterBody.OnSkillActivated += AggroSquelch;
            //On.RoR2.CharacterAI.BaseAI.BeginSkillDriver += AggroSquelch
            //On.RoR2.OverlapAttack.PerformDamage
        }

        private void AggroSquelch(On.RoR2.CharacterBody.orig_OnSkillActivated orig, global::RoR2.CharacterBody self, global::RoR2.GenericSkill skill) {

            BSModLogger = ModLogger;
            try {
                var aggroTracker = self.gameObject.GetComponent<AggroTracker>();
                if (self.HasBuff(RageBuff) && aggroTracker) {
                    BSModLogger.LogInfo($"Attack Primed -> {self.name}");
                    var rand = new System.Random();
                    var res = rand.Next(1,11);
                    if (res > 5) {
                        BSModLogger.LogInfo($"Squelching attack of -> {self.name}");
                        EffectData effectData = new EffectData {
                            origin = self.corePosition + Vector3.up*2f,
                        };
                        EffectManager.SpawnEffect(Fumble_eff, effectData, true);
                        RoR2.SetStateOnHurt.SetStunOnObject(self.gameObject, 0.1f);
                        //self.damage = 0f;
                    } else {
                        //self.damage = aggroTracker.getBaseDmg();
                    }
                }
            }
			catch {
                // Lets hope this works
            }
            orig(self, skill);
        }
        private void AggroApply(On.RoR2.CharacterAI.BaseAI.orig_FixedUpdate orig, global::RoR2.CharacterAI.BaseAI self) {
            orig(self);
            try {
                var aggroTracker = self.GetComponent<AggroTracker>();
                if (self.body.HasBuff(RageBuff) && aggroTracker) {
                    self.currentEnemy.gameObject = aggroTracker.getTarget();
                    self.currentEnemy.bestHurtBox = RoR2.Util.FindBodyMainHurtBox(self.currentEnemy.gameObject.GetComponent<CharacterBody>());
                    self.enemyAttention = 1f;
                    self.body.damage = self.body.baseDamage / 4;
                    self.body.attackSpeed = self.body.baseAttackSpeed * 2;
                    self.body.armor = self.body.baseArmor - 40;
                }
			}
			catch {
                // Hope shit dont get funky
                BSModLogger = ModLogger;
                BSModLogger.LogInfo($"Failed Applying aggro to -> {self.name}");
                return;
			}
		}
        private void Aggro(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, RoR2.GlobalEventManager self, RoR2.DamageInfo damageInfo, GameObject victim) {

            BSModLogger = MeltingPotPlugin.ModLogger;
            if (damageInfo.rejected || damageInfo.procCoefficient <= 0) {
                //BSModLogger.LogInfo($"Rejected");
                orig(self, damageInfo, victim);
                return;
            }
            if (victim && damageInfo.attacker) {
                var InventoryCount = GetCount(damageInfo.attacker.GetComponent<CharacterBody>());
                if (InventoryCount > 0) {
                    //BSModLogger.LogInfo($"Victim and attacker exist");
                    try {
                        var vBody = victim.GetComponent<CharacterBody>();
                        var duration = 1f * InventoryCount;
                        if (!vBody.HasBuff(RageBuff)) {
                            vBody.AddTimedBuff(RageBuff, duration);
						} else {
                            vBody.timedBuffs.Find(x => x.buffIndex == RageBuff.buffIndex).timer = duration;
                        }
                        var aggroTracker = vBody.GetComponent<AggroTracker>();
                        if (!aggroTracker) {
                            aggroTracker = vBody.gameObject.AddComponent<AggroTracker>();
                        }
                        aggroTracker.UpdateTarget(damageInfo.attacker);
                    }
                    catch {
                        BSModLogger.LogError("Failure in characterbody assignment, issues with hurtboxes?");
                    }
                }
            }
            orig(self, damageInfo, victim);
            return;
        }

        private static GameObject LoadEffect(string resourceName, string soundName, bool parentToTransform) {

            GameObject newEffect = MainAssets.LoadAsset<GameObject>(resourceName);

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

        public void CreateEffect() {
            Fumble_eff = LoadEffect("Fumble_MSG.prefab", "", false);

            if (Fumble_eff) { PrefabAPI.RegisterNetworkPrefab(Fumble_eff); }
            EffectAPI.AddEffect(Fumble_eff);
        }
    }
}
