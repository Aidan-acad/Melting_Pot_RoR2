using BepInEx.Configuration;
using MeltingPot.Utils;
using R2API;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace MeltingPot.Items
{
    public class ReactiveArmour : ItemBase<ReactiveArmour>
    {
        private int timeout = 1;
        public static float absorbGrowth = 0.1f;
        private int damageMult = 10;
        private float rangeMult = 0.5f;
        public override string ItemName => "Reactive Armor Plating";
        public override string ItemLangTokenName => "REACTIVEARMOUR";

        public override string ItemPickupDesc =>
            $"<style=cIsHealing>Absorb damage</style> while standing still. Use a skill while moving to <style=cIsDamage>explode</style>.";

        public override string ItemFullDescription =>
            $"After standing still for {timeout} seconds, <style=cIsHealing>absorb {2 * absorbGrowth}%</style> <style=cStack>(+{absorbGrowth}% per stack)</style> of incoming damage. Using a skill while moving releases a nova that deals <style=cIsDamage>{damageMult}x</style> the damage absorbed, in a {rangeMult}*damage radius.";

        public override string ItemLore =>
            "Do not wash, do not dry clean. When cleaning this product, use only insulated cleaning utensils, and remain grounded at all times.\n\n"
            + "Exposure to detergents may cause unpredictable electrocution.";
        public static BepInEx.Logging.ManualLogSource BSModLogger;

        public override string VoidCounterpart => null;
        public GameObject ItemModel;

        public static GameObject ItemBodyModelPrefab;

        public static BuffDef NovaStationaryBuff =>
            ContentPackProvider.contentPack.buffDefs.Find("MeltingPot_NovaStationary");
        public static BuffDef NovaChargeBuff =>
            ContentPackProvider.contentPack.buffDefs.Find("MeltingPot_NovaCharge");
        public static BuffDef NovaOffBuff =>
            ContentPackProvider.contentPack.buffDefs.Find("MeltingPot_NovaOff");
        public static BuffDef NovaPreppedBuff =>
            ContentPackProvider.contentPack.buffDefs.Find("MeltingPot_NovaPrepped");
        public static GameObject ReactiveArmourEffect;
        public static GameObject ReactiveArmourActiveEffect;

        private GameObject jellyNova =>
            LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/JellyfishNova");

        private class ArmourController : CharacterBody.ItemBehavior
        {
            private float cdTimerForDet = 0.5f;

            private bool RAActive = false;
            private float activeStopwatch;
            public int storedDamage = 0;
            public bool execNova = false;
            public bool clearCharge = false;

            public void Awake()
            {
                var body = this.gameObject.GetComponent<CharacterBody>();
            }

            public void removeChargeStacks()
            {
                if (!NetworkServer.active)
                {
                    return;
                }
                body.SetBuffCount(ReactiveArmour.NovaChargeBuff.buffIndex, 0);
                //Debug.LogError(body.GetBuffCount(NovaChargeBuff.buffIndex));
                body.RemoveBuff(ReactiveArmour.NovaChargeBuff);
                clearCharge = false;
                //this.body.statsDirty = true;
            }

            public void addChargeStacks(Vector3 position)
            {
                if (!NetworkServer.active)
                {
                    return;
                }
                if (!body.HasBuff(NovaChargeBuff))
                {
                    body.AddBuff(NovaChargeBuff);
                    this.storedDamage -= 1;
                }
                body.SetBuffCount(
                    NovaChargeBuff.buffIndex,
                    body.GetBuffCount(NovaChargeBuff) + this.storedDamage
                );
                EffectData effectData = new EffectData { origin = position, scale = 0.5f, };
                EffectManager.SpawnEffect(ReactiveArmourEffect, effectData, true);
                this.storedDamage = 0;
            }

            public void fireNova()
            {
                if (!NetworkServer.active)
                {
                    return;
                }
                body.RemoveBuff(ReactiveArmour.NovaPreppedBuff);
                body.AddBuff(ReactiveArmour.NovaOffBuff);
                execNova = false;
                //this.body.statsDirty = true;
            }

            public void FixedUpdate()
            {
                if (!NetworkServer.active)
                {
                    return;
                }
                this.activeStopwatch -= Time.fixedDeltaTime;
                if (execNova)
                {
                    //Debug.Log("Fire nova");
                    fireNova();
                }
                if (clearCharge)
                {
                    //Debug.Log("Remove stacks");
                    removeChargeStacks();
                }
                bool flag = this.RAActive;
                if (flag)
                {
                    this.activeStopwatch = cdTimerForDet;
                }
                bool flag2 =
                    this.activeStopwatch <= 0f
                    && !body.HasBuff(ReactiveArmour.NovaPreppedBuff)
                    && !body.HasBuff(ReactiveArmour.NovaOffBuff);
                if (flag2)
                {
                    if (body.HasBuff(ReactiveArmour.NovaChargeBuff))
                    {
                        body.AddBuff(ReactiveArmour.NovaPreppedBuff);
                        //this.body.statsDirty = true;
                    }
                    else
                    {
                        body.AddBuff(ReactiveArmour.NovaOffBuff);
                        //this.body.statsDirty = true;
                    }
                }
                bool flag3 = body.notMovingStopwatch > 1.0f && !this.RAActive;
                if (flag3)
                {
                    if (body.HasBuff(ReactiveArmour.NovaOffBuff))
                    {
                        body.RemoveBuff(ReactiveArmour.NovaOffBuff);
                    }
                    if (body.HasBuff(ReactiveArmour.NovaPreppedBuff))
                    {
                        body.RemoveBuff(ReactiveArmour.NovaPreppedBuff);
                    }
                    body.AddBuff(ReactiveArmour.NovaStationaryBuff);
                    this.RAActive = true;
                    this.activeStopwatch = cdTimerForDet;
                    //this.body.statsDirty = true;
                }
                else
                {
                    bool flag4 = body.notMovingStopwatch == 0f && this.RAActive;
                    if (flag4)
                    {
                        this.RAActive = false;
                        body.RemoveBuff(ReactiveArmour.NovaStationaryBuff);
                        //this.body.statsDirty = true;
                    }
                }
            }
        }

        public override void Init(ConfigFile config, bool enabled)
        {
            NetworkingAPI.RegisterMessageType<SyncRAState>();
            CreateItem("ReactiveArmour_ItemDef", enabled);
            if (enabled)
            {
                ItemModel = Assets.mainAssetBundle.LoadAsset<GameObject>(
                    $"{ModelPath}/reactive_armour/reactivearmour.prefab"
                );
                CreateLang();
                CreateEffect();
                Hooks();
            }
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>(
                $"{ModelPath}/reactive_armour/displayreactivearmour.prefab"
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
                        childName = "Chest",
                        localPos = new Vector3(0.00473F, -0.03606F, 0.02104F),
                        localAngles = new Vector3(0F, 90F, 0F),
                        localScale = new Vector3(0.26753F, 0.22849F, 0.17399F)
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
                        childName = "Chest",
                        localPos = new Vector3(-0.02273F, -0.04551F, 0.03308F),
                        localAngles = new Vector3(0F, 90F, 0.00001F),
                        localScale = new Vector3(0.22323F, 0.17449F, 0.11072F)
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
                        childName = "Chest",
                        localPos = new Vector3(-0.18924F, 0.23135F, 0.55907F),
                        localAngles = new Vector3(0F, 90F, 0F),
                        localScale = new Vector3(3.90961F, 1.3F, 1.95139F)
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
                        childName = "Chest",
                        localPos = new Vector3(0.02968F, -0.02068F, 0.04089F),
                        localAngles = new Vector3(0F, 90F, 6.64848F),
                        localScale = new Vector3(0.26495F, 0.2F, 0.2F)
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
                        childName = "LowerArmR",
                        localPos = new Vector3(0.01381F, 0.16665F, -0.02592F),
                        localAngles = new Vector3(0F, 0F, 0F),
                        localScale = new Vector3(0.12498F, 0.08348F, 0.08348F)
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
                        childName = "Chest",
                        localPos = new Vector3(0.00092F, -0.12525F, 0.01279F),
                        localAngles = new Vector3(0F, 90F, 0F),
                        localScale = new Vector3(0.23017F, 0.2F, 0.20399F)
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
                        childName = "FlowerBase",
                        localPos = new Vector3(0.21472F, 1.54277F, -0.11269F),
                        localAngles = new Vector3(0F, 90F, 0F),
                        localScale = new Vector3(0.2378F, 0.2378F, 0.2378F)
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
                        localPos = new Vector3(0.001F, -0.08771F, 0.03861F),
                        localAngles = new Vector3(0F, 90F, 0F),
                        localScale = new Vector3(0.2F, 0.2F, 0.2F)
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
                        childName = "MouthMuzzle",
                        localPos = new Vector3(-0.107F, -0.57462F, -3.1195F),
                        localAngles = new Vector3(0F, 90F, 71.97762F),
                        localScale = new Vector3(2.8524F, 1.92976F, 1.04712F)
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
                        childName = "Chest",
                        localPos = new Vector3(0.00248F, -0.04984F, 0.07887F),
                        localAngles = new Vector3(0F, 90F, 347.0011F),
                        localScale = new Vector3(0.2F, 0.2F, 0.2F)
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
                        localPos = new Vector3(-0.00002F, -0.00353F, -0.02429F),
                        localAngles = new Vector3(0F, 90F, 0F),
                        localScale = new Vector3(0.2F, 0.17922F, 0.16009F)
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
                        childName = "Base",
                        localPos = new Vector3(0F, 0.87132F, 0F),
                        localAngles = new Vector3(0F, 90F, 0F),
                        localScale = new Vector3(0.58567F, 0.70078F, 0.64713F)
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
                        childName = "Chest",
                        localPos = new Vector3(6.69883F, 7.15236F, -1.99524F),
                        localAngles = new Vector3(318.1998F, 283.3813F, 149.0648F),
                        localScale = new Vector3(0.96234F, 1.28312F, 0.96234F)
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
                        childName = "Chest",
                        localPos = new Vector3(-0.00001F, -0.1017F, 0.00001F),
                        localAngles = new Vector3(0F, 85.34407F, 0F),
                        localScale = new Vector3(0.16226F, 0.16226F, 0.16226F)
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
                        childName = "Chest",
                        localPos = new Vector3(0F, -0.20444F, 0.00441F),
                        localAngles = new Vector3(0F, 90F, 0F),
                        localScale = new Vector3(0.21555F, 0.21555F, 0.21555F)
                    }
                }
            );
            return rules;
        }

        public override void Hooks()
        {
            On.RoR2.CharacterBody.OnInventoryChanged += HandleArmourCtrl;
            On.RoR2.GenericSkill.OnExecute += BlastNova;
            On.RoR2.HealthComponent.TakeDamage += TakeDamage;
            On.RoR2.CharacterModel.UpdateOverlays += CharacterModel_UpdateOverlays;
        }

        private void CharacterModel_UpdateOverlays(
            On.RoR2.CharacterModel.orig_UpdateOverlays orig,
            CharacterModel self
        )
        {
            orig(self);

            if (self)
            {
                if (self.body && self.body.HasBuff(NovaStationaryBuff))
                {
                    var overlayController = self.body.GetComponent<OverlayTracker>();
                    if (!overlayController)
                        overlayController = self.body.gameObject.AddComponent<OverlayTracker>();
                    else
                        return;

                    overlayController.Body = self.body;
                    TemporaryOverlay overlay = self.gameObject.AddComponent<TemporaryOverlay>();
                    overlay.duration = float.PositiveInfinity;
                    overlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    overlay.animateShaderAlpha = true;
                    overlay.destroyComponentOnEnd = true;
                    overlay.originalMaterial = LegacyResourcesAPI.Load<Material>(
                        "Materials/matLunarGolemShield"
                    );
                    overlay.AddToCharacerModel(self);
                    overlayController.Overlay = overlay;
                    overlayController.Buff = NovaStationaryBuff.buffIndex;
                }
            }
        }

        private void HandleArmourCtrl(
            On.RoR2.CharacterBody.orig_OnInventoryChanged orig,
            global::RoR2.CharacterBody self
        )
        {
            self.AddItemBehavior<ArmourController>(GetCount(self));
            orig(self);
        }

        public void CreateEffect()
        {
            ReactiveArmourEffect = PrefabAPI.InstantiateClone(
                LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/LightningFlash"),
                "ReactiveArmourProc"
            );

            var effectComponent = ReactiveArmourEffect.GetComponent<EffectComponent>();
            effectComponent.soundName = "Melting_Pot_Reactive";

            ReactiveArmourEffect.AddComponent<NetworkIdentity>();

            if (ReactiveArmourEffect)
            {
                PrefabAPI.RegisterNetworkPrefab(ReactiveArmourEffect);
            }
            ContentAddition.AddEffect(ReactiveArmourEffect);
        }

        private void BlastNova(
            On.RoR2.GenericSkill.orig_OnExecute orig,
            global::RoR2.GenericSkill self
        )
        {
            var owner = self.characterBody;
            if (owner)
            {
                var body = owner;
                var InventoryCount = GetCount(body);
                if (InventoryCount > 0)
                {
                    var armourCtrl = owner.GetComponent<ArmourController>();
                    if (!armourCtrl)
                    {
                        orig(self);
                        return;
                    }
                    if (body.HasBuff(NovaChargeBuff) && body.HasBuff(NovaPreppedBuff))
                    {
                        // Do explosion
                        BlastAttack blastAttack = new BlastAttack
                        {
                            attacker = owner.gameObject,
                            inflictor = owner.gameObject,
                            teamIndex = TeamComponent.GetObjectTeam(owner.gameObject),
                            position = owner.corePosition,
                            procCoefficient = 0f,
                            radius = 0.5f * body.GetBuffCount(NovaChargeBuff),
                            baseForce = 100f * body.GetBuffCount(NovaChargeBuff),
                            bonusForce = Vector3.up * 2000f,
                            baseDamage = 10 * body.GetBuffCount(NovaChargeBuff),
                            falloffModel = BlastAttack.FalloffModel.SweetSpot,
                            crit = Util.CheckRoll(owner.crit, owner.master),
                            damageColorIndex = DamageColorIndex.Item,
                            attackerFiltering = AttackerFiltering.NeverHitSelf
                        };
                        blastAttack.Fire();
                        EffectData effectData = new EffectData
                        {
                            origin = owner.corePosition,
                            scale = 0.5f * body.GetBuffCount(NovaChargeBuff),
                        };
                        EffectManager.SpawnEffect(jellyNova, effectData, true);

                        armourCtrl.execNova = true;
                        armourCtrl.clearCharge = true;
                        NetMessageExtensions.Send(
                            new SyncRAState(
                                body.gameObject.GetComponent<NetworkIdentity>().netId,
                                true,
                                true
                            ),
                            (NetworkDestination)(NetworkServer.active ? 1 : 2)
                        );
                    }
                }
            }
            orig(self);
        }

        private void TakeDamage(
            On.RoR2.HealthComponent.orig_TakeDamage orig,
            RoR2.HealthComponent self,
            RoR2.DamageInfo damageInfo
        )
        {
            if (!NetworkServer.active)
            {
                orig(self, damageInfo);
                return;
            }
            if (!damageInfo.rejected || damageInfo == null)
            {
                var inventoryCount = GetCount(self.body);
                if (self.body.HasBuff(NovaStationaryBuff) && inventoryCount > 0)
                {
                    var percentage = 0.2f + (0.1f * (inventoryCount - 1));
                    var damage = damageInfo.damage * percentage;
                    damageInfo.damage -= damage;
                    var armourCtrl = self.body.GetComponent<ArmourController>();
                    if (!armourCtrl)
                    {
                        orig(self, damageInfo);
                        return;
                    }
                    armourCtrl.storedDamage = (int)damage;
                    armourCtrl.addChargeStacks(damageInfo.position);
                }
            }
            orig(self, damageInfo);
        }

        public class SyncRAState : INetMessage, ISerializableObject
        {
            // Token: 0x0600047A RID: 1146 RVA: 0x0002F85A File Offset: 0x0002DA5A
            public SyncRAState() { }

            // Token: 0x0600047B RID: 1147 RVA: 0x0002F864 File Offset: 0x0002DA64
            public SyncRAState(NetworkInstanceId objID, bool execNova, bool clearCharge)
            {
                this.objID = objID;
                this.execNova = execNova;
                this.clearCharge = clearCharge;
            }

            // Token: 0x0600047C RID: 1148 RVA: 0x0002F87C File Offset: 0x0002DA7C
            public void Deserialize(NetworkReader reader)
            {
                this.objID = reader.ReadNetworkId();
                this.execNova = reader.ReadBoolean();
                this.clearCharge = reader.ReadBoolean();
            }

            // Token: 0x0600047D RID: 1149 RVA: 0x0002F898 File Offset: 0x0002DA98
            public void OnReceived()
            {
                GameObject gameObject = Util.FindNetworkObject(this.objID);
                bool flag = gameObject;
                if (flag)
                {
                    ArmourController component = gameObject.GetComponent<ArmourController>();
                    bool flag2 = component;
                    if (flag2)
                    {
                        component.execNova = this.execNova;
                        component.clearCharge = this.clearCharge;
                    }
                }
            }

            // Token: 0x0600047E RID: 1150 RVA: 0x0002F8DC File Offset: 0x0002DADC
            public void Serialize(NetworkWriter writer)
            {
                writer.Write(this.objID);
                writer.Write(this.execNova);
                writer.Write(this.clearCharge);
            }

            // Token: 0x0400042A RID: 1066
            private NetworkInstanceId objID;

            // Token: 0x0400042B RID: 1067
            private bool execNova;
            private bool clearCharge;
        }
    }
}
