using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using MeltingPot.Utils;
using static MeltingPot.MeltingPotPlugin;
using UnityEngine.Networking;

namespace MeltingPot.Items
    {
    public class ReactiveArmour : ItemBase<ReactiveArmour>
    {
        public override string ItemName => "Reactive Armour Plating";
        public override string ItemLangTokenName => "REACTIVE_PLATE";

        public override string ItemPickupDesc => $"After standing still for 1 second absorb <style=cStack>20% (+10% per stack)</style> of incoming damage. Using a skill whilst moving releases the nova.";

        public override string ItemFullDescription => $"After standing still for 1 second absorb <style=cStack>20% (+10% per stack)</style> of incoming damage. Using a skill whilst moving releases a nova that deals <style=cIsDamage>10x the damage absorbed</style>.";

        public override string ItemLore => "[A sticker on the inside of the chestplate reads]\n\n" +
            "Do not wash, do not dry clean. When cleaning this product use only insulated cleaning utensils, and remain grounded at all times.\n\n" +
            "Exposure to detergents may cause unpredictable electrocution.";
        public static BepInEx.Logging.ManualLogSource BSModLogger;

        public override ItemTag[] ItemTags => new ItemTag[] { ItemTag.Utility};
        public override ItemTier Tier => ItemTier.Tier1;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("ReactiveArmour.prefab");
        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("Reactive_Armour_Icon.png");
        private Sprite RAActiveIcon => MainAssets.LoadAsset<Sprite>("RA_Active_Icon.png");
        private Sprite RAChargeIcon => MainAssets.LoadAsset<Sprite>("RA_Charge_Icon.png");

        public static GameObject ItemBodyModelPrefab;

        public static RoR2.BuffDef NovaStationaryBuff;
        public static RoR2.BuffDef NovaChargeBuff;
        public static RoR2.BuffDef NovaOffBuff;
        public static RoR2.BuffDef NovaPreppedBuff;
        public static GameObject ReactiveArmourEffect;
        public static GameObject ReactiveArmourActiveEffect;

        private class ArmourController : CharacterBody.ItemBehavior
        {
            private float cdTimerForDet = 0.5f;

            private bool RAActive = false;
            private float activeStopwatch;

            public void Awake()
            {
                this.body = base.gameObject.GetComponent<CharacterBody>();
            }

            public void removeChargeStacks()
            {
                this.body.SetBuffCount(ReactiveArmour.NovaChargeBuff.buffIndex, 0);
                this.body.RemoveBuff(ReactiveArmour.NovaChargeBuff);
                this.body.statsDirty = true;
            }

            public void fireNova()
            {
                this.body.RemoveBuff(ReactiveArmour.NovaPreppedBuff);
                this.body.AddBuff(ReactiveArmour.NovaOffBuff);
                this.body.statsDirty = true;
            }

            public void FixedUpdate()
            {
                this.activeStopwatch -= Time.fixedDeltaTime;
                bool flag = this.RAActive;
                if (flag)
                {
                    this.activeStopwatch = cdTimerForDet;
                }
                bool flag2 = this.activeStopwatch <= 0f && !this.body.HasBuff(ReactiveArmour.NovaPreppedBuff) && !this.body.HasBuff(ReactiveArmour.NovaOffBuff);
                if (flag2)
                {
                    if (this.body.HasBuff(ReactiveArmour.NovaChargeBuff))
                    {
                        this.body.AddBuff(ReactiveArmour.NovaPreppedBuff);
                        this.body.statsDirty = true;
                    }
                    else
                    {
                        this.body.AddBuff(ReactiveArmour.NovaOffBuff);
                        this.body.statsDirty = true;
                    }
                }
                bool flag3 = this.body.notMovingStopwatch > 1.0f && !this.RAActive;
                if (flag3)
                {
                    if (this.body.HasBuff(ReactiveArmour.NovaOffBuff)){
                        this.body.RemoveBuff(ReactiveArmour.NovaOffBuff);
                    }
                    if (this.body.HasBuff(ReactiveArmour.NovaPreppedBuff))
                    {
                        this.body.RemoveBuff(ReactiveArmour.NovaPreppedBuff);
                    }
                    this.body.AddBuff(ReactiveArmour.NovaStationaryBuff);
                    this.RAActive = true;
                    this.activeStopwatch = cdTimerForDet;
                    this.body.statsDirty = true;
                }
                else
                {
                    bool flag4 = this.body.notMovingStopwatch == 0f && this.RAActive;
                    if (flag4)
                    {
                        this.RAActive = false;
                        this.body.RemoveBuff(ReactiveArmour.NovaStationaryBuff);
                        this.body.statsDirty = true;
                    }
                }
            }

        }



        public override void Init(ConfigFile config)
        {
            CreateLang();
            CreateItem();
            CreateBuff();
            CreateEffect();
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
                    childName = "Chest",
                    localPos = new Vector3(0.00469F, -0.00422F, 0.02647F),
                    localAngles = new Vector3(0F, 90F, 0F),
                    localScale = new Vector3(0.24514F, 0.20937F, 0.15943F)
                }
            });
            rules.Add("mdlHuntress", new RoR2.ItemDisplayRule[]
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
            });
            rules.Add("mdlToolbot", new RoR2.ItemDisplayRule[]
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
            });
            rules.Add("mdlEngi", new RoR2.ItemDisplayRule[]
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
            });
            rules.Add("mdlMage", new RoR2.ItemDisplayRule[]
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
            });
            rules.Add("mdlMerc", new RoR2.ItemDisplayRule[]
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
            });
            rules.Add("mdlTreebot", new RoR2.ItemDisplayRule[]
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
            });
            rules.Add("mdlLoader", new RoR2.ItemDisplayRule[]
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
            });
            rules.Add("mdlCroco", new RoR2.ItemDisplayRule[]
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
            });
            rules.Add("mdlCaptain", new RoR2.ItemDisplayRule[]
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
            });
            rules.Add("mdlBandit2", new RoR2.ItemDisplayRule[]
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
            });
            return rules;
        }

        public override void Hooks()
        {
            On.RoR2.CharacterBody.OnInventoryChanged += HandleArmourCtrl;
            On.RoR2.GenericSkill.OnExecute += BlastNova;
            On.RoR2.HealthComponent.TakeDamage += TakeDamage;
            On.RoR2.CharacterModel.UpdateOverlays += CharacterModel_UpdateOverlays;
        }

        private void CharacterModel_UpdateOverlays(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
        {
            orig(self);

            if (self)
            {
                if (self.body && self.body.HasBuff(NovaStationaryBuff))
                {
                    var overlayController = self.body.GetComponent<OverlayTracker>();
                    if (!overlayController) overlayController = self.body.gameObject.AddComponent<OverlayTracker>();
                    else return;


                    overlayController.Body = self.body;
                    TemporaryOverlay overlay = self.gameObject.AddComponent<TemporaryOverlay>();
                    overlay.duration = float.PositiveInfinity;
                    overlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    overlay.animateShaderAlpha = true;
                    overlay.destroyComponentOnEnd = true;
                    overlay.originalMaterial = Resources.Load<Material>("Materials/matLunarGolemShield");
                    overlay.AddToCharacerModel(self);
                    overlayController.Overlay = overlay;
                    overlayController.Buff = NovaStationaryBuff.buffIndex;
                }
            }
        }

        private void HandleArmourCtrl(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, global::RoR2.CharacterBody self)
        {

            self.AddItemBehavior<ArmourController>(base.GetCount(self));
            orig(self);
        }

        private void CreateBuff()
        {
            NovaStationaryBuff = ScriptableObject.CreateInstance<RoR2.BuffDef>();
            NovaStationaryBuff.name = "Melting Pot : Repulsor Plate Active";
            NovaStationaryBuff.buffColor = Color.green;
            NovaStationaryBuff.canStack = false;
            NovaStationaryBuff.isDebuff = false;
            NovaStationaryBuff.iconSprite = RAActiveIcon;

            BuffAPI.Add(new CustomBuff(NovaStationaryBuff));


            NovaOffBuff = ScriptableObject.CreateInstance<RoR2.BuffDef>();
            NovaOffBuff.name = "Melting Pot : Repulsor Plate Inactive";
            NovaOffBuff.buffColor = Color.red;
            NovaOffBuff.canStack = false;
            NovaOffBuff.isDebuff = false;
            NovaOffBuff.iconSprite = RAActiveIcon;

            BuffAPI.Add(new CustomBuff(NovaOffBuff));

            NovaPreppedBuff = ScriptableObject.CreateInstance<RoR2.BuffDef>();
            NovaPreppedBuff.name = "Melting Pot : Repulsor Plate Primed";
            NovaPreppedBuff.buffColor = Color.yellow;
            NovaPreppedBuff.canStack = false;
            NovaPreppedBuff.isDebuff = false;
            NovaPreppedBuff.iconSprite = RAActiveIcon;

            BuffAPI.Add(new CustomBuff(NovaPreppedBuff));


            NovaChargeBuff = ScriptableObject.CreateInstance<RoR2.BuffDef>();
            NovaChargeBuff.name = "Melting Pot : Repulsor Plate Charge";
            NovaChargeBuff.buffColor = Color.cyan;
            NovaChargeBuff.canStack = true;
            NovaChargeBuff.isDebuff = false;
            NovaChargeBuff.iconSprite = RAChargeIcon;

            BuffAPI.Add(new CustomBuff(NovaChargeBuff));
        }

        public void CreateEffect()
        {
            ReactiveArmourEffect = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/LightningFlash"), "ReactiveArmourProc");

            var mpReactiveSoundDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            mpReactiveSoundDef.eventName = "Melting_Pot_Reactive";
            SoundAPI.AddNetworkedSoundEvent(mpReactiveSoundDef);

            var effectComponent = ReactiveArmourEffect.GetComponent<EffectComponent>();
            effectComponent.soundName = "Melting_Pot_Reactive";

            ReactiveArmourEffect.AddComponent<NetworkIdentity>();

            if (ReactiveArmourEffect) { PrefabAPI.RegisterNetworkPrefab(ReactiveArmourEffect); }
            EffectAPI.AddEffect(ReactiveArmourEffect);
        }

            private void BlastNova(On.RoR2.GenericSkill.orig_OnExecute orig, global::RoR2.GenericSkill self)
            {
                var owner = self.characterBody;
                if (owner)
                {
                    var body = owner;
                    var InventoryCount = GetCount(body);
                    if (InventoryCount > 0)
                    {
                        var armourCtrl = owner.GetComponent<ArmourController>();
                        if (!armourCtrl) { orig(self); return; }
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
                                radius = 2f * body.GetBuffCount(NovaChargeBuff),
                                baseForce = 100f* body.GetBuffCount(NovaChargeBuff),
                                bonusForce = Vector3.up * 2000f,
                                baseDamage = 10*body.GetBuffCount(NovaChargeBuff),
                                falloffModel = BlastAttack.FalloffModel.SweetSpot,
                                crit = Util.CheckRoll(owner.crit, owner.master),
                                damageColorIndex = DamageColorIndex.Item,
                                attackerFiltering = AttackerFiltering.NeverHit
                            };
                            blastAttack.Fire();
                            EffectData effectData = new EffectData
                            {
                                origin = owner.corePosition,
                                scale = 1f * body.GetBuffCount(NovaChargeBuff),
                            };
                            EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/JellyfishNova"), effectData, true);
                            armourCtrl.fireNova();
                            armourCtrl.removeChargeStacks();
                        }
                    }
                }
                orig(self);
            }
            private void TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo damageInfo)
            {
                if (!damageInfo.rejected || damageInfo == null)
                {
                    var inventoryCount = GetCount(self.body);
                    if (self.body.HasBuff(NovaStationaryBuff) && inventoryCount > 0)
                    {
                        var percentage = 0.2f + (0.1f * (inventoryCount - 1));
                        var damage = damageInfo.damage * percentage;
                        damageInfo.damage -= damage;
                        for (int x = 0; x < damage; x++)
                        {
                            self.body.AddBuff(NovaChargeBuff);
                            EffectData effectData = new EffectData
                            {
                                origin = damageInfo.position,
                                scale = 0.5f,
                            };
                            EffectManager.SpawnEffect(ReactiveArmourEffect, effectData, true);
                        }
                    }
                }
                orig(self, damageInfo);
            }
        }
    }