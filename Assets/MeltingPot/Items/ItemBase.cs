using BepInEx.Configuration;
using MeltingPot.Utils;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MeltingPot.Items
{
    // The directly below is entirely from TILER2 API (by ThinkInvis) specifically the Item module. Utilized to keep instance checking functionality as I migrate off TILER2.
    // TILER2 API can be found at the following places:
    // https://github.com/ThinkInvis/RoR2-TILER2
    // https://thunderstore.io/package/ThinkInvis/TILER2/

    public abstract class ItemBase<T> : ItemBase where T : ItemBase<T>
    {
        public static T instance { get; private set; }

        public ItemBase()
        {
            if (instance != null)
                throw new InvalidOperationException(
                    "Singleton class \""
                        + typeof(T).Name
                        + "\" inheriting ItemBoilerplate/Item was instantiated twice"
                );
            instance = this as T;
        }
    }

    public abstract class ItemBase
    {
        public abstract string ItemName { get; }
        public abstract string ItemLangTokenName { get; }
        public abstract string ItemPickupDesc { get; }
        public abstract string ItemFullDescription { get; }
        public abstract string ItemLore { get; }

        public ItemDef ItemDef;
        public abstract string VoidCounterpart { get; }

        public abstract void Init(ConfigFile config, bool enabled);

        public ItemDisplayRuleDict ItemDisplayRules { get; set; }

        public virtual bool AIBlacklisted { get; set; } = false;
        public virtual bool Enabled { get; set; } = true;

        public static string ModelPath { get; set; }

        public void PopulateMPath()
        {
            ModelPath = $"assets/meltingpot/mpassets/itemprefabs/{TierConversion(ItemDef.tier)}";
        }

        protected void CreateLang()
        {
            LanguageAPI.Add("MeltingPot_ITEM_" + ItemLangTokenName + "_NAME", ItemName);
            LanguageAPI.Add("MeltingPot_ITEM_" + ItemLangTokenName + "_PICKUP", ItemPickupDesc);
            LanguageAPI.Add("MeltingPot_ITEM_" + ItemLangTokenName + "_DESC", ItemFullDescription);
            LanguageAPI.Add("MeltingPot_ITEM_" + ItemLangTokenName + "_LORE", ItemLore);
        }

        protected string TierConversion(ItemTier inpTier)
        {
            string otp = "";
            switch (inpTier)
            {
                case ItemTier.Tier1:
                    otp = "Tier_1";
                    break;
                case ItemTier.Tier2:
                    otp = "Tier_2";
                    break;
                case ItemTier.Tier3:
                    otp = "Tier_3";
                    break;
                case ItemTier.Lunar:
                    otp = "Lunar";
                    break;
                case ItemTier.Boss:
                    otp = "Boss";
                    break;
                case ItemTier.VoidTier1:
                    otp = "Void_Tier_1";
                    break;
                case ItemTier.VoidTier2:
                    otp = "Void_Tier_2";
                    break;
                case ItemTier.VoidTier3:
                    otp = "Void_Tier_3";
                    break;
                case ItemTier.NoTier:
                    otp = "Hidden";
                    break;
            }
            return otp;
        }

        public abstract ItemDisplayRuleDict CreateItemDisplayRules();

        protected void CreateItem(string name, bool enabled)
        {
            ItemDef = ContentPackProvider.contentPack.itemDefs.Find(name);
            if (enabled)
            {
                PopulateMPath();
            }
            if (AIBlacklisted)
            {
                ItemDef.tags = new List<ItemTag>(ItemDef.tags) { ItemTag.AIBlacklist }.ToArray();
            }
            bool flag = false;
            // Ensure in the content pack you dont put the base item after the void item I didnt make this that robust a check
            if (VoidCounterpart != null)
            {
                MeltingPotPlugin.ItemStatusDictionary.TryGetValue(
                    MeltingPotPlugin.Items.Find(x => x.ItemDef.name == VoidCounterpart),
                    out flag
                );
                if (flag)
                {
                    ItemDef baseItemDef = ContentPackProvider.contentPack.itemDefs.Find(
                        VoidCounterpart
                    );
                    On.RoR2.ItemCatalog.SetItemRelationships += (orig, providers) =>
                    {
                        var isp = ScriptableObject.CreateInstance<ItemRelationshipProvider>();
                        isp.relationshipType = DLC1Content.ItemRelationshipTypes.ContagiousItem;
                        isp.relationships = new[]
                        {
                            new ItemDef.Pair { itemDef1 = baseItemDef, itemDef2 = ItemDef }
                        };
                        orig(providers.Concat(new[] { isp }).ToArray());
                    };
                }
            }
            ItemDisplayRules = CreateItemDisplayRules();
            if (!enabled)
            {
                Enabled = false;
            }
        }

        public abstract void Hooks();

        // The below is entirely from TILER2 API (by ThinkInvis) specifically the Item module. Utilized to keep easy count functionality as I migrate off TILER2.
        // TILER2 API can be found at the following places:
        // https://github.com/ThinkInvis/RoR2-TILER2
        // https://thunderstore.io/package/ThinkInvis/TILER2/

        public int GetCount(CharacterBody body)
        {
            if (!body || !body.inventory)
            {
                return 0;
            }

            return body.inventory.GetItemCount(ItemDef);
        }

        public int GetCount(CharacterMaster master)
        {
            if (!master || !master.inventory)
            {
                return 0;
            }

            return master.inventory.GetItemCount(ItemDef);
        }

        public int GetCountSpecific(CharacterBody body, ItemDef itemDef)
        {
            if (!body || !body.inventory)
            {
                return 0;
            }

            return body.inventory.GetItemCount(itemDef);
        }
    }
}
