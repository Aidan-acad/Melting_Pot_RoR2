using BepInEx.Configuration;
using MeltingPot.Utils;
using R2API;
using RoR2;
using R2API.Networking;
using R2API.Networking.Interfaces;
using System;
using UnityEngine;
using UnityEngine.Networking;
using RoR2.Hologram;

namespace MeltingPot.Items
{
    public class BloodPrice : ItemBase<BloodPrice>
    {
        public static float MoneyGrowth = 0.5f;
        public static int HealthCostGrowth = 25;

        public override string ItemName => "Blood Price";
        public override string ItemLangTokenName => "BLOODPRICE";
        public override string ItemPickupDesc =>
            $"You convert all gold into <style=cIsUtility>exp</style> every 30 seconds <style=cDeath>BUT all gold prices cost a percentage of your health</style>";
        public override string ItemFullDescription =>
            $"You convert <style=cShrine>100% (+{MoneyGrowth * 100}% per stack) of gold</style> into <style=cIsUtility>exp</style> every 30 seconds <style=cDeath>BUT</style> all gold prices cost (25/30/50/75)% (+{HealthCostGrowth}% per stack - Max 93%) of your health scaling with interactable value";
        public override string ItemLore =>
            "[Haemo Global brand credit card]\n\n"
            + "Skip the middle man, don't waste your blood, sweat and tears working!\n\nHaemo Global is a subsidiary of Sanguisuge Consortium Ltd.";

        public override string VoidCounterpart => null;
        public static BepInEx.Logging.ManualLogSource BSModLogger;

        public static GameObject ItemModel;
        public static GameObject ItemBodyModelPrefab;
        public static GameObject BloodPriceEffect;

        public override void Init(ConfigFile config, bool enabled)
        {
            NetworkingAPI.RegisterMessageType<SyncBPPrice>();
            CreateItem("BloodPrice_ItemDef", enabled);
            if (enabled)
            {
                ItemModel = Assets.mainAssetBundle.LoadAsset<GameObject>(
                    $"{ModelPath}/blood_price/bloodprice.prefab"
                );
                CreateLang();
                CreateEffect();
                Hooks();
            }
        }

        public void CreateEffect()
        {
            BloodPriceEffect = PrefabAPI.InstantiateClone(
                LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/LevelUpEffect"),
                "BloodPriceProc"
            );

            var effectComponent = BloodPriceEffect.GetComponent<EffectComponent>();
            effectComponent.soundName = "Melting_Pot_Blood";
            BloodPriceEffect.AddComponent<DestroyOnTimer>().duration = 1;
            BloodPriceEffect.AddComponent<NetworkIdentity>();

            if (BloodPriceEffect)
            {
                PrefabAPI.RegisterNetworkPrefab(BloodPriceEffect);
            }
            ContentAddition.AddEffect(BloodPriceEffect);
        }

        private int MoneyToHealthConv(int cost)
        {
            int hCost = 25;
            switch (cost)
            {
                case int c when c <= 22:
                    // white chest
                    break;
                case int c when c <= 40:
                    // Spec chest/ cheap drones
                    hCost = 30;
                    break;
                case int c when c <= 54:
                    // Green chest/ Missile drone etc.
                    hCost = 50;
                    break;
                default:
                    // Everything more expensive than that
                    hCost = 75;
                    break;
            }
            return hCost;
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>(
                $"{ModelPath}/blood_price/displaybloodprice.prefab"
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
                        childName = "ThighL",
                        localPos = new Vector3(0.12408F, -0.09422F, 0.04995F),
                        localAngles = new Vector3(31.40705F, 47.53595F, 124.626F),
                        localScale = new Vector3(0.10402F, 0.12245F, 0.10402F)
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
                        childName = "BowBase",
                        localPos = new Vector3(-0.1696F, 0.00026F, -0.0385F),
                        localAngles = new Vector3(345.9257F, 340.6141F, 345.5604F),
                        localScale = new Vector3(0.10402F, 0.12245F, 0.10402F)
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
                        localPos = new Vector3(2.41448F, 1.88166F, 2.44416F),
                        localAngles = new Vector3(331.5673F, 69.29052F, 346.1078F),
                        localScale = new Vector3(1.08263F, 1.27444F, 1.08263F)
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
                        localPos = new Vector3(-0.06995F, 0.0709F, 0.23949F),
                        localAngles = new Vector3(350.6499F, 343.0152F, 337.6042F),
                        localScale = new Vector3(0.10402F, 0.12245F, 0.10402F)
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
                        childName = "Pelvis",
                        localPos = new Vector3(0.08564F, -0.06714F, -0.1518F),
                        localAngles = new Vector3(30.96777F, 4.98328F, 197.8541F),
                        localScale = new Vector3(0.10402F, 0.12245F, 0.10402F)
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
                        childName = "HandL",
                        localPos = new Vector3(-0.00132F, 0.11242F, 0.05488F),
                        localAngles = new Vector3(31.80595F, 20.9214F, 156.823F),
                        localScale = new Vector3(0.10402F, 0.12245F, 0.10402F)
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
                        localPos = new Vector3(0.13264F, 0.00613F, 0.2124F),
                        localAngles = new Vector3(7.9107F, 70.21392F, 66.40268F),
                        localScale = new Vector3(0.33251F, 0.30547F, 0.25949F)
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
                        localPos = new Vector3(-0.00153F, 0.00337F, 0.23311F),
                        localAngles = new Vector3(349.326F, 342.4679F, 339.4268F),
                        localScale = new Vector3(0.25589F, 0.30123F, 0.25589F)
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
                        childName = "Chest",
                        localPos = new Vector3(0.71319F, 0.61441F, -2.27547F),
                        localAngles = new Vector3(341.6003F, 341.184F, 340.5392F),
                        localScale = new Vector3(2.20014F, 2.10666F, 2.07156F)
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
                        childName = "MuzzleGun",
                        localPos = new Vector3(0.00397F, 0.02395F, 0.03695F),
                        localAngles = new Vector3(307.8961F, 212.2206F, 125.8659F),
                        localScale = new Vector3(0.10402F, 0.12245F, 0.10402F)
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
                        childName = "Hat",
                        localPos = new Vector3(0.07714F, 0.0843F, 0.07642F),
                        localAngles = new Vector3(0F, 0F, 0F),
                        localScale = new Vector3(0.10402F, 0.12245F, 0.10402F)
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
                        childName = "LegBar2",
                        localPos = new Vector3(-0.17988F, 0.25211F, 0.11095F),
                        localAngles = new Vector3(17.48137F, 68.31F, 64.29781F),
                        localScale = new Vector3(0.24188F, 0.28473F, 0.24188F)
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
                        childName = "Backpack",
                        localPos = new Vector3(-7.42417F, 12.2573F, 0.61768F),
                        localAngles = new Vector3(1.53418F, 115.9251F, 34.57936F),
                        localScale = new Vector3(3.31941F, 3.90754F, 3.31941F)
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
                        childName = "Backpack",
                        localPos = new Vector3(-0.01631F, 0.42072F, 0.03442F),
                        localAngles = new Vector3(26.29276F, 63.29855F, 63.94849F),
                        localScale = new Vector3(0.21628F, 0.2546F, 0.21628F)
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
                        childName = "Head",
                        localPos = new Vector3(-0.15546F, 0.01265F, -0.12889F),
                        localAngles = new Vector3(300.0789F, 240.5217F, 46.9622F),
                        localScale = new Vector3(0.13342F, 0.15706F, 0.13342F)
                    }
                }
            );
            return rules;
        }

        public override void Hooks()
        {
            On.RoR2.Interactor.AttemptInteraction += SiphonForItem;
            On.RoR2.InteractionDriver.OnPreRenderOutlineHighlight += UpdateUI;
            On.RoR2.CharacterBody.OnInventoryChanged += AttachBloodPriceCtrl;
        }

        private void InteriorCheck(global::RoR2.OutlineHighlight outlineHighlight)
        {
            if (!outlineHighlight.sceneCamera)
            {
                return;
            }
            if (!outlineHighlight.sceneCamera.cameraRigController)
            {
                return;
            }
            GameObject target = outlineHighlight.sceneCamera.cameraRigController.target;
            if (!target)
            {
                return;
            }
            InteractionDriver component = target.GetComponent<InteractionDriver>();
            if (!component)
            {
                return;
            }
            if (GetCount(component.characterBody) < 1)
            {
                return;
            }
            GameObject gameObject = component.FindBestInteractableObject();
            if (!gameObject)
            {
                return;
            }
            PurchaseInteraction purchaseI = gameObject.GetComponent<PurchaseInteraction>();
            if (!purchaseI || purchaseI.costType != CostTypeIndex.Money)
            {
                return;
            }
            HologramProjector costHolo = gameObject.GetComponent<HologramProjector>();
            if (!costHolo)
            {
                return;
            }
			try {

                int descaledCost = (int)(
                    (float)purchaseI.cost
                    / Mathf.Pow(Run.instance.compensatedDifficultyCoefficient, 1.25f)
                );
                int hCost = Math.Min(
                    MoneyToHealthConv(descaledCost)
                        + (HealthCostGrowth * (GetCount(component.characterBody) - 1)),
                    93
                );
                GameObject HologramContentInstance = costHolo.hologramContentInstance;
                CostHologramContent costComponent =
                    HologramContentInstance.GetComponent<CostHologramContent>();
                if (costComponent) {
                    costComponent.displayValue = hCost;
                    costComponent.costType = CostTypeIndex.PercentHealth;
                }
            }
			catch {

			}
        }

        private void UpdateUI(
            On.RoR2.InteractionDriver.orig_OnPreRenderOutlineHighlight orig,
            global::RoR2.OutlineHighlight outlineHighlight
        )
        {
            InteriorCheck(outlineHighlight);
            orig(outlineHighlight);
        }

        private void AttachBloodPriceCtrl(
            On.RoR2.CharacterBody.orig_OnInventoryChanged orig,
            global::RoR2.CharacterBody self
        )
        {
            self.AddItemBehavior<BloodPriceController>(GetCount(self));
            orig(self);
        }

        private void SiphonForItem(
            On.RoR2.Interactor.orig_AttemptInteraction orig,
            global::RoR2.Interactor self,
            GameObject interactableObject
        )
        {
            PurchaseInteraction target = interactableObject.GetComponent<PurchaseInteraction>();
            CharacterBody instigator = self.gameObject.GetComponent<CharacterBody>();
            if (
                target
                && target.costType == CostTypeIndex.Money
                && instigator
                && GetCount(instigator) > 0
            )
            {
                int heldCost = target.cost;
                int descaledCost = (int)(
                    (float)target.cost
                    / Mathf.Pow(Run.instance.compensatedDifficultyCoefficient, 1.25f)
                );
                int newCost = Math.Min(
                    MoneyToHealthConv(descaledCost)
                        + (HealthCostGrowth * (GetCount(instigator) - 1)),
                    93
                );
                NetMessageExtensions.Send(
                    new SyncBPPrice(
                        self.gameObject.GetComponent<NetworkIdentity>().netId,
                        newCost,
                        CostTypeIndex.PercentHealth,
                        target
                    ),
                    (NetworkDestination)(NetworkServer.active ? 1 : 2)
                );
                orig(self, interactableObject);
                if (target.isShrine)
                {
                    NetMessageExtensions.Send(
                        new SyncBPPrice(
                            self.gameObject.GetComponent<NetworkIdentity>().netId,
                            heldCost,
                            CostTypeIndex.Money,
                            target
                        ),
                        (NetworkDestination)(NetworkServer.active ? 1 : 2)
                    );
                }
            }
            else
            {
                orig(self, interactableObject);
            }
        }

        private class BloodPriceController : CharacterBody.ItemBehavior
        {
            private class BloodPriceExchanger : RoR2.ConvertPlayerMoneyToExperience
            {
                public GameObject AttachedMaster { get; set; }

                public void Awake()
                {
                    if (!AttachedMaster)
                    {
                        AttachedMaster = gameObject;
                    }
                }

                public new void FixedUpdate()
                {
                    this.burstTimer -= Time.fixedDeltaTime;
                    if (this.burstTimer <= 0f)
                    {
                        bool flag = false;
                        GameObject gameObject = AttachedMaster;
                        CharacterMaster component = gameObject.GetComponent<CharacterMaster>();
                        uint num;
                        if (!this.burstSizes.TryGetValue(gameObject, out num))
                        {
                            num = (uint)Mathf.CeilToInt(component.money / (float)this.burstCount);
                            this.burstSizes[gameObject] = num;
                        }
                        if (num > component.money)
                        {
                            num = component.money;
                        }
                        component.money -= num;
                        GameObject bodyObject = component.GetBodyObject();
                        ulong num2 = (ulong)(num / 2f / (float)1);
                        if (num > 0U)
                        {
                            flag = true;
                        }
                        if (bodyObject)
                        {
                            ExperienceManager.instance.AwardExperience(
                                base.transform.position,
                                bodyObject.GetComponent<CharacterBody>(),
                                num2
                            );
                        }
                        else
                        {
                            TeamManager.instance.GiveTeamExperience(component.teamIndex, num2);
                        }
                        if (flag)
                        {
                            this.burstTimer = this.burstInterval;
                            return;
                        }
                        if (this.burstTimer < -2.5f)
                        {
                            EffectManager.SpawnEffect(
                                BloodPriceEffect,
                                new EffectData
                                {
                                    origin = bodyObject.transform.position,
                                    scale = 2f,
                                },
                                true
                            );
                            UnityEngine.Object.Destroy(this);
                        }
                    }
                }
            }

            private float cdTimer;
            private BloodPriceExchanger bpExchange;

            public void Awake()
            {
                cdTimer = 30f;
            }

            public void FixedUpdate()
            {
                if (!NetworkServer.active)
                {
                    return;
                }
                cdTimer -= Time.fixedDeltaTime;
                if (cdTimer <= 0)
                {
                    cdTimer = 30f;
                    body.master.money = System.Convert.ToUInt32(
                        body.master.money * (1 + (stack - 1) * MoneyGrowth)
                    );
                    bpExchange = body.masterObject.AddComponent<BloodPriceExchanger>();
                }
            }
        }

        public class SyncBPPrice : INetMessage, ISerializableObject
        {
            // Token: 0x0600047A RID: 1146 RVA: 0x0002F85A File Offset: 0x0002DA5A
            public SyncBPPrice() { }

            // Token: 0x0600047B RID: 1147 RVA: 0x0002F864 File Offset: 0x0002DA64
            public SyncBPPrice(
                NetworkInstanceId objID,
                int cost,
                CostTypeIndex tgtType,
                PurchaseInteraction purchaseInt
            )
            {
                this.objID = objID;
                this.cost = cost;
                this.costType = tgtType;
                this.interaction = purchaseInt;
            }

            // Token: 0x0600047C RID: 1148 RVA: 0x0002F87C File Offset: 0x0002DA7C
            public void Deserialize(NetworkReader reader)
            {
                this.objID = reader.ReadNetworkId();
                this.cost = reader.ReadInt32();
                this.costType = (CostTypeIndex)reader.ReadInt32();
                this.interaction = reader.ReadGameObject().GetComponent<PurchaseInteraction>();
            }

            // Token: 0x0600047D RID: 1149 RVA: 0x0002F898 File Offset: 0x0002DA98
            public void OnReceived()
            {
                interaction.Networkcost = cost;
                interaction.costType = costType;
            }

            // Token: 0x0600047E RID: 1150 RVA: 0x0002F8DC File Offset: 0x0002DADC
            public void Serialize(NetworkWriter writer)
            {
                writer.Write(this.objID);
                writer.Write(this.cost);
                writer.Write((Int32)this.costType);
                writer.Write(this.interaction.gameObject);
            }

            // Token: 0x0400042A RID: 1066
            private NetworkInstanceId objID;

            // Token: 0x0400042B RID: 1067
            private int cost;
            private CostTypeIndex costType;
            private PurchaseInteraction interaction;
        }
    }
}
