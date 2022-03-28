using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using UnityEngine;
using MeltingPot.Utils;
using UnityEngine.Networking;
using static MeltingPot.MeltingPotPlugin;
using R2API.Networking;
using R2API.Networking.Interfaces;
using static R2API.RecalculateStatsAPI;

namespace MeltingPot.Items
{
    public class MechMosquito : ItemBase<MechMosquito> {
        public static float bleedChance = 0.1f;
        public static float drainGrowth = 0.2f;
        public override string ItemName => "Mechanical Mosquito";
        public override string ItemLangTokenName => "MECHMOSQUITO";

        public override string ItemPickupDesc => $"Drones and turrets gain a chance to bleed, and heal for a portion of bleed damage dealt";

        public override string ItemFullDescription => $"Mechanical Minions gain <style=cDeath>{bleedChance*200}%</style> <style=cStack>(+{bleedChance*100}% per stack)</style> bleed chance, and heal for <style=cIsHealing>{drainGrowth*100}%</style> <style=cStack>(per stack)</style> of bleed damage dealt";

        public override string VoidCounterpart => null;
        public override string ItemLore => "[Stamped onto the base]\n\n" +
            "Official EnderGrimm™ Brand - Mechanical Mosquito:\n\n A sharper shot for a healthier bot";
        public static BepInEx.Logging.ManualLogSource BSModLogger;

        public GameObject ItemModel => Assets.mainAssetBundle.LoadAsset<GameObject>("assets/meltingpot/mpassets/itemprefabs/mechanical_mosquito/mechmosquito.prefab");

        public static BuffDef MosquitoBuff => ContentPackProvider.contentPack.buffDefs.Find("MeltingPot_Mosquito");

        public static GameObject ItemBodyModelPrefab;


        public override void Init(ConfigFile config, bool enabled) {
            NetworkingAPI.RegisterMessageType<SyncMechUpdate>();
            CreateItem("MechMosquito_ItemDef", enabled);
            if (enabled) {
                CreateLang();
                Hooks();
            }
        }
        private void AttachMosquitoCtrl(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, global::RoR2.CharacterBody self) {

            self.AddItemBehavior<MosquitoController>(base.GetCount(self));
            orig(self);
        }

        private void ApplySiphon(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo damageInfo) {
            if (damageInfo.damageType == DamageType.BleedOnHit) {
                if (damageInfo.attacker.GetComponent<CharacterMaster>().minionOwnership.ownerMaster) {
                    var InventoryCount = GetCount(damageInfo.attacker.GetComponent<CharacterMaster>().minionOwnership.ownerMaster.GetBody());

                    // Apply heal
                    damageInfo.attacker.GetComponent<HealthComponent>().Heal(InventoryCount * 0.2f * damageInfo.damage, damageInfo.procChainMask);
                }
            }
            orig(self, damageInfo);
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules() {
            ItemBodyModelPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>("assets/meltingpot/mpassets/itemprefabs/mechanical_mosquito/displaymechmosquito.prefab");
            Vector3 generalScale = new Vector3(1f, 1f, 1f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
            rules.Add("mdlCommandoDualies", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.00637F, 0.38645F, -0.00434F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.03109F, 0.0366F, 0.03109F)
                }
            });
            rules.Add("mdlHuntress", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.00628F, 0.30692F, -0.06469F),
                    localAngles = new Vector3(346.3873F, 359.9307F, 0.04274F),
                    localScale = new Vector3(0.0442F, 0.04967F, 0.04712F)
                }
            });
            rules.Add("mdlToolbot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(1.94045F, 2.53389F, -1.30613F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.43093F, 0.43093F, 0.43093F)
                }
            });
            rules.Add("mdlEngi", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HeadCenter",
                    localPos = new Vector3(-0.0002F, 0.16138F, 0.03043F),
                    localAngles = new Vector3(12.08325F, 0.0086F, 0.08055F),
                    localScale = new Vector3(0.02757F, 0.02873F, 0.02972F)
                }
            });
            rules.Add("mdlMage", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.00083F, 0.16511F, 0.00132F),
                    localAngles = new Vector3(15.17724F, 0.04178F, 0.08851F),
                    localScale = new Vector3(0.05801F, 0.05526F, 0.06258F)
                }
            });
            rules.Add("mdlMerc", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.00111F, 0.26027F, 0.02932F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.01931F, 0.02318F, 0.02669F)
                }
            });
            rules.Add("mdlTreebot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ShoulderL",
                    localPos = new Vector3(-0.00039F, -0.12919F, 0.00794F),
                    localAngles = new Vector3(2.69396F, 287.1411F, 179.4548F),
                    localScale = new Vector3(0.07135F, 0.07135F, 0.07135F)
                }
            });
            rules.Add("mdlLoader", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(-0.20586F, 0.49159F, 0.17499F),
                    localAngles = new Vector3(7.44334F, 0.00317F, 0.04899F),
                    localScale = new Vector3(0.02658F, 0.0269F, 0.02156F)
                }
            });
            rules.Add("mdlCroco", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(2.91017F, 1.8545F, 1.89574F),
                    localAngles = new Vector3(354.2337F, 154.6552F, 69.96472F),
                    localScale = new Vector3(0.30798F, 0.31683F, 0.28998F)
                }
            });
            rules.Add("mdlCaptain", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0.05122F, 0.23185F, -0.01225F),
                    localAngles = new Vector3(335.3733F, 341.37F, 358.3186F),
                    localScale = new Vector3(0.04729F, 0.04729F, 0.04729F)
                }
            });
            rules.Add("mdlBandit2", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Hat",
                    localPos = new Vector3(0.00884F, 0.12571F, -0.02546F),
                    localAngles = new Vector3(331.0096F, 353.1977F, 2.92943F),
                    localScale = new Vector3(0.02096F, 0.02508F, 0.02316F)
                }
            });
            rules.Add("mdlEngiTurret", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0F, 1.43745F, -0.56229F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.38537F, 0.46111F, 0.42581F)
                }
            });
            rules.Add("mdlScav", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0.74289F, 6.68591F, -1.76427F),
                    localAngles = new Vector3(346.5088F, 184.5537F, 359.5352F),
                    localScale = new Vector3(1.35F, 1.8F, 1.35F)
                }
            });
            rules.Add("mdlRailGunner", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Backpack",
                    localPos = new Vector3(0.05857F, 0.40009F, -0.06997F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.03669F, 0.03669F, 0.03669F)
                }
            });
            rules.Add("mdlVoidSurvivor", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.05085F, 0.12299F, -0.10779F),
                    localAngles = new Vector3(310.3616F, 329.4937F, 37.09079F),
                    localScale = new Vector3(0.07043F, 0.07043F, 0.07043F)
                }
            });
            return rules;
        }

        public override void Hooks() {
            On.RoR2.CharacterBody.OnInventoryChanged += AttachMosquitoCtrl;
            On.RoR2.HealthComponent.TakeDamage += ApplySiphon;
            On.RoR2.MinionOwnership.MinionGroup.AddMinion += BuffOnPurchase;
        }
        private static void BuffOnPurchase(On.RoR2.MinionOwnership.MinionGroup.orig_AddMinion orig, NetworkInstanceId ownerId, global::RoR2.MinionOwnership minion) {
            orig(ownerId, minion);
            GameObject spawned_minion = minion.gameObject;
            CharacterMaster minion_owner =minion.ownerMaster;
            //MeltingPotPlugin.ModLogger.LogError($"Checking spawned minion -- {spawned_minion.name}");
            if (minion_owner) {
                //MeltingPotPlugin.ModLogger.LogError($"Owner -- {minion_owner.name}");
                if (minion_owner.inventory.itemStacks[(int)ContentPackProvider.contentPack.itemDefs.Find("MechMosquito_ItemDef").itemIndex] > 0) {
                    //MeltingPotPlugin.ModLogger.LogError($"n_mosq : {minion_owner.inventory.itemStacks[(int)ContentPackProvider.contentPack.itemDefs.Find("MechMosquito_ItemDef").itemIndex]}");
                    MosquitoController owner_mosquito_ctrl = minion_owner.GetBody().gameObject.GetComponent<MosquitoController>();
                    if (!owner_mosquito_ctrl) {
                        //MeltingPotPlugin.ModLogger.LogError($"No controller attached");
                        return;
					}
                    NetMessageExtensions.Send(new SyncMechUpdate(minion_owner.GetBody().gameObject.GetComponent<NetworkIdentity>().netId), (NetworkDestination)(NetworkServer.active ? 1 : 2));
                    /*if (NetworkServer.active) {
                        owner_mosquito_ctrl.DoUpdate = true;
					}*/
                }
            }
		}

        private class MosquitoController : CharacterBody.ItemBehavior
        {
            CharacterMaster heldMaster;
            public bool DoUpdate = false;
            public float CheckCd = 0f;
			public void Awake() {
                this.body = this.gameObject.GetComponent<CharacterBody>();
                heldMaster = body.masterObject.GetComponent<CharacterMaster>();
                UpdateMinions();
            }

			public void FixedUpdate() {
                CheckCd -= Time.fixedDeltaTime;
                if (DoUpdate && CheckCd < 0) {
                    UpdateMinions();
                    DoUpdate = false;
				}
			}

			public void UpdateMinions() {
                if (heldMaster) {
                    CollectMinions(heldMaster, GrantMinionBleed);
                }
            }

			private void GrantMinionBleed(CharacterBody minion) {
                if (NetworkServer.active) {
                    if (((int)minion.bodyFlags & 2) > 0 && !minion.baseNameToken.Contains("ENGITURRET")) {
                        if (minion.maxHealth != 0) {
                            //MeltingPotPlugin.ModLogger.LogError($"Found minion {minion.name}");
                            var inv_check = minion.inventory.itemStacks[(int)RoR2Content.Items.BleedOnHit.itemIndex];
                            //MeltingPotPlugin.ModLogger.LogError($"Minion has {inv_check} bleed daggers");
                            //MeltingPotPlugin.ModLogger.LogError($"{this.stack}");
                            if (inv_check != this.stack) {
                                minion.inventory.GiveItem(RoR2.RoR2Content.Items.BleedOnHit.itemIndex, (this.stack + 1) - inv_check);
                            }
                            //MeltingPotPlugin.ModLogger.LogError($"Minion now has {minion.inventory.itemStacks[(int)RoR2Content.Items.BleedOnHit.itemIndex]} bleed daggers");
                            if (!minion.HasBuff(MosquitoBuff)) {
                                minion.AddBuff(MosquitoBuff);
                                //MeltingPotPlugin.ModLogger.LogError($"Gave Minion Buff");
                            }
                        }
                    }
                }
            }

            public void CollectMinions(CharacterMaster ownerMaster, Action<CharacterBody> actionToRun) {
                ownerMaster.DroneHunt((minionMaster) =>
                {
                    CharacterBody minionBody = minionMaster.GetBody();
                    if (minionBody) {
                        actionToRun(minionBody);
                    }
                });
            }
        }
        public class SyncMechUpdate : INetMessage, ISerializableObject
        {
            // Token: 0x0600047A RID: 1146 RVA: 0x0002F85A File Offset: 0x0002DA5A
            public SyncMechUpdate() {
            }

            // Token: 0x0600047B RID: 1147 RVA: 0x0002F864 File Offset: 0x0002DA64
            public SyncMechUpdate(NetworkInstanceId objID) {
                this.objID = objID;
            }

            // Token: 0x0600047C RID: 1148 RVA: 0x0002F87C File Offset: 0x0002DA7C
            public void Deserialize(NetworkReader reader) {
                this.objID = reader.ReadNetworkId();
            }

            // Token: 0x0600047D RID: 1149 RVA: 0x0002F898 File Offset: 0x0002DA98
            public void OnReceived() {
                GameObject gameObject = Util.FindNetworkObject(this.objID);
                bool flag = gameObject;
                if (flag) {
                    MosquitoController component = gameObject.GetComponent<MosquitoController>();
                    bool flag2 = component;
                    if (flag2) {
                        component.DoUpdate = true;
                        component.CheckCd = 0.25f;
                    }
                }
            }

            // Token: 0x0600047E RID: 1150 RVA: 0x0002F8DC File Offset: 0x0002DADC
            public void Serialize(NetworkWriter writer) {
                writer.Write(this.objID);
            }

            // Token: 0x0400042A RID: 1066
            private NetworkInstanceId objID;
        }
    }
}