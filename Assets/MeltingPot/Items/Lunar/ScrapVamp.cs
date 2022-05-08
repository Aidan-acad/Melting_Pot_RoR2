using BepInEx.Configuration;
using HG;
using MeltingPot.Utils;
using R2API;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static R2API.RecalculateStatsAPI;

namespace MeltingPot.Items
{
    public class ScrapVamp : ItemBase<ScrapVamp>
    {
        public static float armourPerStack = 4f;
        public static float damagePerStack = 1f;
        public static float healthPerStack = 15f;

        public static float damageMalusGrowth = 0.25f;
        public static float asGrowth = 0.1f;
        public static float healthGrowth = 0.2f;
        public static float baseRadius = 30f;
        public override string ItemName => "Mark of the Malevolent Machinist";
        public override string ItemLangTokenName => "SCRAPVAMP";
        public override string ItemPickupDesc =>
            $"Defeating mechanical enemies permanently boosts your stats. Repair drones automatically, <style=cDeath>but they are hostile</style>.";
        public override string ItemFullDescription =>
            $"Defeating mechanical enemies permanently increases damage by <style=cIsDamage>{damagePerStack}</style>, max HP by <style=cIsHealing>{healthPerStack}</style>, and armor by <style=cIsUtility>{armourPerStack}</style> <style=cStack>(per stack)</style>. Drones repair automatically within <style=cIsUtility>{baseRadius}m</style> <style=cStack>(+{baseRadius}m per stack)</style>, but are always hostile. Drones gain buffs above 1 stack.";
        public override string ItemLore =>
            "As you stare into its eyes, you hear a voice whisper:\n\n"
            + "More! Bring me more of the cowardly automatons! Feed me, and I will lend you my strength.";

        public override string VoidCounterpart => null;
        public static BepInEx.Logging.ManualLogSource BSModLogger;

        public static GameObject ItemModel;
        public static GameObject ItemBodyModelPrefab;

        public static BuffDef VampArmour =>
            ContentPackProvider.contentPack.buffDefs.Find("MeltingPot_VampArmour");
        public static BuffDef VampHealth =>
            ContentPackProvider.contentPack.buffDefs.Find("MeltingPot_VampHealth");
        public static BuffDef VampDamage =>
            ContentPackProvider.contentPack.buffDefs.Find("MeltingPot_VampDamage");
        public static ItemDef vampArmourTally =>
            ContentPackProvider.contentPack.itemDefs.Find("ScrapVamp_ArmourTally");
        public static ItemDef vampHealthTally =>
            ContentPackProvider.contentPack.itemDefs.Find("ScrapVamp_HealthTally");
        public static ItemDef vampDamageTally =>
            ContentPackProvider.contentPack.itemDefs.Find("ScrapVamp_DamageTally");

        private EntityStates.SerializableEntityStateType droneStateOverride =
            new EntityStates.SerializableEntityStateType(typeof(NoRespawnDrone));

        public override void Init(ConfigFile config, bool enabled)
        {
            NetworkingAPI.RegisterMessageType<SyncVampStacks>();
            CreateItem("ScrapVamp_ItemDef", enabled);
            if (enabled)
            {
                ItemModel = Assets.mainAssetBundle.LoadAsset<GameObject>(
                    $"{ModelPath}/scrap_vamp/ScrapVamp.prefab"
                );
                CreateLang();
                Hooks();
            }
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>(
                $"{ModelPath}/scrap_vamp/displayscrapvamp.prefab"
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
                        childName = "Base",
                        localPos = new Vector3(-0.49323F, 0.11615F, -0.90168F),
                        localAngles = new Vector3(90F, 180F, 0F),
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
                        childName = "Base",
                        localPos = new Vector3(-0.49323F, 0.11615F, -0.90168F),
                        localAngles = new Vector3(90F, 180F, 0F),
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
                        childName = "Base",
                        localPos = new Vector3(-0.49323F, 0.11615F, -0.90168F),
                        localAngles = new Vector3(90F, 180F, 0F),
                        localScale = new Vector3(0.10402F, 0.12245F, 0.10402F)
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
                        childName = "Base",
                        localPos = new Vector3(-0.49323F, 0.11615F, -0.90168F),
                        localAngles = new Vector3(90F, 180F, 0F),
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
                        childName = "Base",
                        localPos = new Vector3(-0.49323F, 0.11615F, -0.90168F),
                        localAngles = new Vector3(90F, 180F, 0F),
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
                        childName = "Base",
                        localPos = new Vector3(-0.49323F, 0.11615F, -0.90168F),
                        localAngles = new Vector3(90F, 180F, 0F),
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
                        childName = "Base",
                        localPos = new Vector3(0.03075F, 0.11615F, -3.40706F),
                        localAngles = new Vector3(90F, 180F, 0F),
                        localScale = new Vector3(0.25179F, 0.23132F, 0.1965F)
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
                        childName = "Base",
                        localPos = new Vector3(-0.49323F, 0.11615F, -0.90168F),
                        localAngles = new Vector3(90F, 180F, 0F),
                        localScale = new Vector3(0.10402F, 0.12245F, 0.10402F)
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
                        childName = "Neck",
                        localPos = new Vector3(0.00001F, 1.55992F, -1.49533F),
                        localAngles = new Vector3(27.05177F, 0F, 0F),
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
                        childName = "Base",
                        localPos = new Vector3(-0.49323F, 0.11615F, -0.90168F),
                        localAngles = new Vector3(90F, 180F, 0F),
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
                        childName = "Base",
                        localPos = new Vector3(-0.49323F, 0.11615F, -0.90168F),
                        localAngles = new Vector3(90F, 180F, 0F),
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
                        childName = "Base",
                        localPos = new Vector3(-0.49323F, 0.11615F, -0.90168F),
                        localAngles = new Vector3(90F, 180F, 0F),
                        localScale = new Vector3(0.10402F, 0.12245F, 0.10402F)
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
                        childName = "Base",
                        localPos = new Vector3(-0.49323F, 0.11615F, -0.90168F),
                        localAngles = new Vector3(90F, 180F, 0F),
                        localScale = new Vector3(0.10402F, 0.12245F, 0.10402F)
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
                        childName = "Base",
                        localPos = new Vector3(-0.49323F, 0.11615F, -0.90168F),
                        localAngles = new Vector3(90F, 180F, 0F),
                        localScale = new Vector3(0.10402F, 0.12245F, 0.10402F)
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
                        childName = "Base",
                        localPos = new Vector3(-0.49323F, 0.11615F, -0.90168F),
                        localAngles = new Vector3(90F, 180F, 0F),
                        localScale = new Vector3(0.10402F, 0.12245F, 0.10402F)
                    }
                }
            );
            return rules;
        }

        public override void Hooks()
        {
            GetStatCoefficients += GrantStats;
            On.RoR2.CharacterBody.OnInventoryChanged += AttachScrapVampCtrl;
            On.RoR2.GlobalEventManager.OnCharacterDeath += CheckDeath;
        }

        private void CheckDeath(
            On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig,
            RoR2.GlobalEventManager self,
            DamageReport rep
        )
        {
            if (
                rep.victimBody.baseNameToken.Contains("DRONE")
                && !rep.victimBody.baseNameToken.Contains("MEGADRONE")
            )
            {
                rep.victim.GetComponent<CharacterDeathBehavior>().deathState = droneStateOverride;
            }
            if ((((int)rep.victimBody.bodyFlags & 2) > 0) && GetCount(rep.attackerBody) > 0)
            {
                int aStack = 0;
                int hStack = 0;
                int dStack = 0;
                string bName = rep.victimBody.baseNameToken;
                switch (bName)
                {
                    case string a when a.Contains("HEALING"):
                        hStack += 1;
                        break;
                    case string a when a.Contains("EMERGENCY"):
                        hStack += 5;
                        break;
                    case string a when a.Contains("MISSILE"):
                        dStack += 2;
                        break;
                    case string a when a.Contains("GUNNER"):
                        dStack += 1;
                        break;
                    case string a when a.Contains("TURRET"):
                        aStack += 1;
                        break;
                    case string a when a.Contains("STRIKE"):
                        dStack += 1;
                        break;
                    case string a when a.Contains("FLAME"):
                        dStack += 5;
                        break;
                    case string a when a.Contains("COMMANDER"):
                        dStack += 3;
                        aStack += 2;
                        break;
                    case string a when a.Contains("MEGA"):
                        aStack += 10;
                        dStack += 10;
                        break;
                    case string a when a.Contains("EQUIPMENT"):
                        hStack += 2;
                        break;
                    case string a when a.Contains("ROBOBALL"):
                        if (a.Contains("BOSS"))
                        {
                            dStack += 15;
                        }
                        else
                        {
                            aStack += 1;
                            hStack += 1;
                        }
                        break;
                    default:
                        hStack += 1;
                        break;
                }
                NetMessageExtensions.Send(
                    new SyncVampStacks(
                        rep.attackerBody.gameObject.GetComponent<NetworkIdentity>().netId,
                        hStack,
                        aStack,
                        dStack
                    ),
                    (NetworkDestination)(NetworkServer.active ? 1 : 2)
                );
            }
            orig(self, rep);
        }

        private void AttachScrapVampCtrl(
            On.RoR2.CharacterBody.orig_OnInventoryChanged orig,
            global::RoR2.CharacterBody self
        )
        {
            self.AddItemBehavior<ScrapVampController>(GetCount(self));
            orig(self);
        }

        private void GrantStats(CharacterBody sender, StatHookEventArgs args)
        {
            var count = GetCount(sender);
            if (count > 0)
            {
                int aCount = sender.GetBuffCount(VampArmour);
                int hCount = sender.GetBuffCount(VampHealth);
                int dCount = sender.GetBuffCount(VampDamage);
                args.armorAdd += armourPerStack * count * aCount;
                args.baseHealthAdd += healthPerStack * count * hCount;
                args.baseDamageAdd += damagePerStack * count * dCount;
            }
        }

        private class NoRespawnDrone : EntityStates.Drone.DeathState
        {
            public override void OnImpactServer(Vector3 contactPoint)
            {
                MeltingPotPlugin.ModLogger.LogInfo($"No Drone for you");
            }
        }

        private class ScrapVampController : CharacterBody.ItemBehavior
        {
            private float cdTimerForSearch = 0.5f;
            private float radius = 30f;
            private int healthStack = 0;
            private int armourStack = 0;
            private int damageStack = 0;
            private SphereSearch sphereSearch;
            private float timerStopwatch;

            public void adjustPrivateStacks(int modifier, int stat)
            {
                switch (stat)
                {
                    case 0:
                        this.healthStack += modifier;
                        for (int x = 0; x < modifier; x++)
                        {
                            body.inventory.GiveItem(vampHealthTally);
                        }
                        break;
                    case 1:
                        this.armourStack += modifier;
                        for (int x = 0; x < modifier; x++)
                        {
                            body.inventory.GiveItem(vampArmourTally);
                        }
                        break;
                    case 2:
                        this.damageStack += modifier;
                        for (int x = 0; x < modifier; x++)
                        {
                            body.inventory.GiveItem(vampDamageTally);
                        }
                        break;
                }
            }

            public void Awake()
            {
                radius = baseRadius * this.stack;
                this.body = this.gameObject.GetComponent<CharacterBody>();
                createNewSearch(this.body.transform.position);
                this.healthStack = body.inventory.GetItemCount(vampHealthTally);
                this.armourStack = body.inventory.GetItemCount(vampArmourTally);
                this.damageStack = body.inventory.GetItemCount(vampDamageTally);
            }

            private void createNewSearch(Vector3 origin)
            {
                this.sphereSearch = new SphereSearch();
                this.sphereSearch.origin = origin;
                this.sphereSearch.mask = LayerIndex.CommonMasks.interactable;
                this.sphereSearch.queryTriggerInteraction = QueryTriggerInteraction.Collide;
                this.sphereSearch.radius = this.radius;
            }

            private PurchaseInteraction ScanForTargets()
            {
                List<Collider> list = CollectionPool<Collider, List<Collider>>.RentCollection();
                this.sphereSearch.origin = this.body.transform.localPosition;
                this.sphereSearch.radius = baseRadius * this.stack;
                this.sphereSearch.ClearCandidates();
                this.sphereSearch.RefreshCandidates();
                this.sphereSearch.FilterCandidatesByColliderEntities();
                this.sphereSearch.OrderCandidatesByDistance();
                this.sphereSearch.FilterCandidatesByDistinctColliderEntities();
                this.sphereSearch.GetColliders(list);
                PurchaseInteraction result = null;
                int i = 0;
                int count = list.Count;
                while (i < count)
                {
                    PurchaseInteraction component = list[i]
                        .GetComponent<EntityLocator>()
                        .entity.GetComponent<PurchaseInteraction>();
                    if (PurchaseInteractionIsMechanical(component))
                    {
                        result = component;
                        break;
                    }
                    i++;
                }
                CollectionPool<Collider, List<Collider>>.ReturnCollection(list);
                return result;
            }

            private bool PurchaseInteractionIsMechanical(PurchaseInteraction target)
            {
                return target
                    && target.available
                    && (target.costType == CostTypeIndex.Money)
                    && (
                        target.displayNameToken.Contains("DRONE")
                        || target.displayNameToken.Contains("TURRET")
                        || target.displayNameToken.Contains("MEGA")
                    );
            }

            private void UnlockTarget(PurchaseInteraction target)
            {
                //Debug.Log("Unlocking");
                if (target && target.available)
                {
                    target.Networkcost = 0;
                    //Debug.Log("Fetching Owner");
                    GameObject ownerObject = body.gameObject;
                    //Debug.Log("Owner Fetched");
                    if (ownerObject)
                    {
                        Interactor component = ownerObject.GetComponent<Interactor>();
                        if (component)
                        {
                            component.AttemptInteraction(target.gameObject);
                        }
                    }
                }
            }

            public void CollectMinions(
                CharacterMaster ownerMaster,
                Action<CharacterBody> actionToRun
            )
            {
                ownerMaster.DroneHunt(
                    (minionMaster) =>
                    {
                        CharacterBody minionBody = minionMaster.GetBody();
                        if (minionBody)
                        {
                            actionToRun(minionBody);
                        }
                    }
                );
            }

            private void CorruptAlly(CharacterBody minion)
            {
                //MeltingPotPlugin.ModLogger.LogWarning($"Corrupting -- {minion.baseNameToken}");
                if (((int)minion.bodyFlags & 2) > 0 && !minion.baseNameToken.Contains("ENGITURRET"))
                {
                    if (minion.maxHealth != 0)
                    {
                        //MeltingPotPlugin.ModLogger.LogWarning($"Stacks? {this.stack}");
                        minion.damage *= 0.75f + (this.stack - 1) * damageMalusGrowth;
                        minion.maxHealth *= 1 + (this.stack - 1) * healthGrowth;
                        minion.attackSpeed *= 1 + (this.stack - 1) * asGrowth;
                        minion.master.teamIndex = TeamIndex.Lunar;
                        minion.teamComponent.teamIndex = TeamIndex.Lunar;
                        BaseAI component = minion.master.GetComponent<BaseAI>();
                        if (component)
                        {
                            component.enemyAttention = 0f;
                            component.ForceAcquireNearestEnemyIfNoCurrentEnemy();
                        }
                        minion.master.minionOwnership.ownerMaster = null;
                    }
                }
            }

            public void FixedUpdate()
            {
                this.timerStopwatch -= Time.fixedDeltaTime;
                if (this.timerStopwatch <= 0f)
                {
                    this.timerStopwatch = cdTimerForSearch;
                    PurchaseInteraction purchaseInteraction = this.ScanForTargets();
                    if (purchaseInteraction)
                    {
                        //Debug.Log(purchaseInteraction.displayNameToken);
                        UnlockTarget(purchaseInteraction);
                    }
                    // Corrupt allies
                    //Debug.LogError(this.body.baseNameToken);
                    if (this.body.masterObject.GetComponent<CharacterMaster>())
                    {
                        //Debug.Log("Corrupting");
                        CollectMinions(
                            this.body.masterObject.GetComponent<CharacterMaster>(),
                            CorruptAlly
                        );
                    }
                }
                if (NetworkServer.active)
                {
                    int aCount = body.GetBuffCount(VampArmour);
                    if (aCount != armourStack)
                    {
                        body.SetBuffCount(VampArmour.buffIndex, armourStack);
                    }
                    int hCount = body.GetBuffCount(VampHealth);
                    if (hCount != healthStack)
                    {
                        body.SetBuffCount(VampHealth.buffIndex, healthStack);
                    }
                    int dCount = body.GetBuffCount(VampDamage);
                    if (dCount != damageStack)
                    {
                        body.SetBuffCount(VampDamage.buffIndex, damageStack);
                    }
                }
            }
        }

        public class SyncVampStacks : INetMessage, ISerializableObject
        {
            // Token: 0x0600047A RID: 1146 RVA: 0x0002F85A File Offset: 0x0002DA5A
            public SyncVampStacks() { }

            // Token: 0x0600047B RID: 1147 RVA: 0x0002F864 File Offset: 0x0002DA64
            public SyncVampStacks(NetworkInstanceId objID, int hStacks, int aStacks, int dStacks)
            {
                this.objID = objID;
                this.hStacks = hStacks;
                this.aStacks = aStacks;
                this.dStacks = dStacks;
            }

            // Token: 0x0600047C RID: 1148 RVA: 0x0002F87C File Offset: 0x0002DA7C
            public void Deserialize(NetworkReader reader)
            {
                this.objID = reader.ReadNetworkId();
                this.hStacks = reader.ReadInt32();
                this.aStacks = reader.ReadInt32();
                this.dStacks = reader.ReadInt32();
            }

            // Token: 0x0600047D RID: 1149 RVA: 0x0002F898 File Offset: 0x0002DA98
            public void OnReceived()
            {
                GameObject gameObject = Util.FindNetworkObject(this.objID);
                bool flag = gameObject;
                if (flag)
                {
                    ScrapVampController component = gameObject.GetComponent<ScrapVampController>();
                    bool flag2 = component;
                    if (flag2)
                    {
                        component.adjustPrivateStacks(this.hStacks, 0);
                        component.adjustPrivateStacks(this.aStacks, 1);
                        component.adjustPrivateStacks(this.dStacks, 2);
                    }
                }
            }

            // Token: 0x0600047E RID: 1150 RVA: 0x0002F8DC File Offset: 0x0002DADC
            public void Serialize(NetworkWriter writer)
            {
                writer.Write(this.objID);
                writer.Write(this.hStacks);
                writer.Write(this.aStacks);
                writer.Write(this.dStacks);
            }

            // Token: 0x0400042A RID: 1066
            private NetworkInstanceId objID;

            // Token: 0x0400042B RID: 1067
            private int hStacks;
            private int aStacks;
            private int dStacks;
        }
    }
}
