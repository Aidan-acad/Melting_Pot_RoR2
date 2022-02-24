using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using MeltingPot.Utils;
using UnityEngine.Networking;

namespace MeltingPot.Items
{
    public class ThornShield : ItemBase<ThornShield>
    {
        private static float ArmourGrowth = 0.25f;
        public override string ItemName => "Shield of Thorns";
        public override string ItemLangTokenName => "THORNSHIELD";
        public override string ItemPickupDesc => $"Reflect melee range damage equal to a portion of armour";
        public override string ItemFullDescription => $"When <style=cIsDamage>hit</style> in melee range, return <style=cStack>5 + (2 per stack)</style> plus <style=cStack>{ArmourGrowth*100}% + ({ArmourGrowth*100}% per stack)</style> of your <style=cIsHealing>armour</style> value as damage";
        public override string ItemLore => "[Carved into the inner rim of the shield]\n\n" +
            "Forgive me. Forgive my friends. We burned the druids in anger, and it has come back to haunt us. The ground rages, they're near";

        
        public static BepInEx.Logging.ManualLogSource BSModLogger;
        public static GameObject ThornEffect;

        public static GameObject ItemModel => Assets.mainAssetBundle.LoadAsset<GameObject>("assets/meltingpot/mpassets/itemprefabs/thorn_shield/ShieldofThorns.prefab");
        public static GameObject ItemBodyModelPrefab;

        public static BuffDef ThornActiveBuff => ContentPackProvider.contentPack.buffDefs.Find("MeltingPot_ThornBuff");

        public override void Init(ConfigFile config)
        {

            CreateItem("ThornShield_ItemDef");
            CreateLang();
            CreateEffect();
            Hooks();
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>("assets/meltingpot/mpassets/itemprefabs/thorn_shield/displayThornShield.prefab");
            Vector3 generalScale = new Vector3(1f, 1f, 1f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
            rules.Add("mdlCommandoDualies", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "UpperArmL",
                    localPos = new Vector3(0.08158F, 0.364F, 0.04397F),
                    localAngles = new Vector3(357.7899F, 260.6965F, 270.743F),
                    localScale = new Vector3(0.16226F, 0.16226F, 0.16226F)
                }
            });
            rules.Add("mdlHuntress", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(0F, -0.27727F, 0.15052F),
                    localAngles = new Vector3(0F, 180F, 112.5631F),
                    localScale = new Vector3(0.18139F, 0.18139F, 0.18139F)
                }
            });
            rules.Add("mdlToolbot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Hip",
                    localPos = new Vector3(2.07188F, 0.56545F, 0.00017F),
                    localAngles = new Vector3(0F, 270F, 107.0169F),
                    localScale = new Vector3(1F, 1F, 1F)
                }
            });
            rules.Add("mdlEngi", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0.002F, -0.04605F, -0.30533F),
                    localAngles = new Vector3(354.4472F, 0.53189F, 292.1049F),
                    localScale = new Vector3(0.13121F, 0.13121F, 0.13121F)
                }
            });
            rules.Add("mdlMage", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "LowerArmR",
                    localPos = new Vector3(-0.07468F, 0.17895F, -0.0002F),
                    localAngles = new Vector3(0F, 133.4465F, 0F),
                    localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            });
            rules.Add("mdlMerc", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.00099F, 0.23105F, 0.01854F),
                    localAngles = new Vector3(86.19852F, 3.67883F, 3.4493F),
                    localScale = new Vector3(0.35F, 0.35F, 0.25F)
                }
            });
            rules.Add("mdlTreebot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "FlowerBase",
                    localPos = new Vector3(0.77055F, 1.84111F, -0.34754F),
                    localAngles = new Vector3(0F, -0.00001F, 270F),
                    localScale = new Vector3(0.2F, 0.2F, 0.2F)
                }
            });
            rules.Add("mdlLoader", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Stomach",
                    localPos = new Vector3(0.0013F, 0.3853F, 0.16613F),
                    localAngles = new Vector3(22.26286F, 175.4879F, 262.9642F),
                    localScale = new Vector3(0.2F, 0.2F, 0.2F)
                }
            });
            rules.Add("mdlCroco", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MouthMuzzle",
                    localPos = new Vector3(-0.05746F, 2.66866F, 4.02146F),
                    localAngles = new Vector3(348.6305F, 180.5293F, 178.9295F),
                    localScale = new Vector3(1F, 1F, 1F)
                }
            });
            rules.Add("mdlCaptain", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.00569F, 0.25728F, 0.14752F),
                    localAngles = new Vector3(352.3789F, 180.6956F, 269.482F),
                    localScale = new Vector3(0.04502F, 0.04502F, 0.04502F)
                }
            });
            rules.Add("mdlBandit2", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Hat",
                    localPos = new Vector3(-0.01844F, 0.10087F, 0.07106F),
                    localAngles = new Vector3(31.12656F, 161.9059F, 279.5356F),
                    localScale = new Vector3(0.02676F, 0.02676F, 0.02676F)
                }
            });
            rules.Add("mdlEngiTurret", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "LegBar1",
                    localPos = new Vector3(0F, 0.07968F, 0.18019F),
                    localAngles = new Vector3(359.0982F, 180.6359F, 111.653F),
                    localScale = new Vector3(0.66596F, 0.66596F, 0.66596F)
                }
            });
            rules.Add("mdlScav", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "LowerArmL",
                    localPos = new Vector3(-0.35742F, -0.02077F, 0.62581F),
                    localAngles = new Vector3(355.7607F, 152.8153F, 64.15575F),
                    localScale = new Vector3(4.51801F, 4.51801F, 4.51801F)
                }
            });
            return rules;
        }

        public override void Hooks()
        {
            On.RoR2.SetStateOnHurt.OnTakeDamageServer += Thorns;
            On.RoR2.CharacterBody.FixedUpdate += BoostRegen;
        }

        private static GameObject LoadEffect(string soundName, bool parentToTransform) {

            GameObject newEffect = Assets.mainAssetBundle.LoadAsset<GameObject>("assets/meltingpot/mpassets/effects/Thorns.prefab");

            newEffect.AddComponent<DestroyOnTimer>().duration = 1;
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
            ThornEffect = LoadEffect("", false);

            if (ThornEffect) { PrefabAPI.RegisterNetworkPrefab(ThornEffect); }
            EffectAPI.AddEffect(ThornEffect);
        }
        private void Thorns(On.RoR2.SetStateOnHurt.orig_OnTakeDamageServer orig, SetStateOnHurt self, DamageReport damageReport)
        {
            var body = damageReport.attackerBody;
            var victimBody = damageReport.victimBody;
            if (body && victimBody)
            {
                var InventoryCount = GetCount(victimBody);
                if (InventoryCount > 0) {
                    float distance = Vector3.Distance(body.corePosition, victimBody.corePosition);
                    if (distance < 5.0f) {
                        victimBody.AddTimedBuff(ThornActiveBuff, 0.5f);
                        EffectData effectData = new EffectData {
                            origin = victimBody.corePosition,
                            scale = 0.5f,
                        };
                        effectData.SetHurtBoxReference(victimBody.mainHurtBox);
                        GameObject effectPrefab = ThornEffect;
                        EffectManager.SpawnEffect(effectPrefab, effectData, true);
                        DamageInfo reflect = new DamageInfo {
                            damage = 5 + victimBody.armor * (ArmourGrowth * InventoryCount),
                            crit = false,
                            attacker = victimBody.gameObject,
                            inflictor = victimBody.gameObject,
                            position = victimBody.corePosition,
                            force = new Vector3(0, 0, 0),
                            rejected = false,
                            procCoefficient = 0f,
                            damageType = DamageType.BypassArmor,
                            damageColorIndex = DamageColorIndex.Nearby
                        };
                        body.healthComponent.TakeDamage(reflect);
                    }
                }
            }
            orig(self, damageReport);
            return;
        }

        private void BoostRegen(On.RoR2.CharacterBody.orig_FixedUpdate orig, RoR2.CharacterBody self) {
            orig(self);
            var InventoryCount = GetCount(self);
            if (InventoryCount > 0 && self.HasBuff(ThornActiveBuff.buffIndex)) {
                self.regen += InventoryCount * (0.2f);
			}
		}

    }
}