using BepInEx.Configuration;
using R2API;
using RoR2;
using System.Collections.Generic;
using MeltingPot.Utils;
using System;

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
            if (instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting ItemBoilerplate/Item was instantiated twice");
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

        public abstract void Init(ConfigFile config);

        public ItemDisplayRuleDict ItemDisplayRules { get; set; }

        public virtual bool AIBlacklisted { get; set; } = false;

        protected void CreateLang()
        {
            LanguageAPI.Add("Shasocais_ITEM_" + ItemLangTokenName + "_NAME", ItemName);
            LanguageAPI.Add("Shasocais_ITEM_" + ItemLangTokenName + "_PICKUP", ItemPickupDesc);
            LanguageAPI.Add("Shasocais_ITEM_" + ItemLangTokenName + "_DESC", ItemFullDescription);
            LanguageAPI.Add("Shasocais_ITEM_" + ItemLangTokenName + "_LORE", ItemLore);
        }

        public abstract ItemDisplayRuleDict CreateItemDisplayRules();

        protected void CreateItem(string name)
        {
            ItemDef = ContentPackProvider.contentPack.itemDefs.Find(name);
            if (AIBlacklisted) {
                ItemDef.tags = new List<ItemTag>(ItemDef.tags) { ItemTag.AIBlacklist }.ToArray();
            }
            ItemDisplayRules = CreateItemDisplayRules();      
        }

        public abstract void Hooks();

        // The below is entirely from TILER2 API (by ThinkInvis) specifically the Item module. Utilized to keep easy count functionality as I migrate off TILER2.
        // TILER2 API can be found at the following places:
        // https://github.com/ThinkInvis/RoR2-TILER2
        // https://thunderstore.io/package/ThinkInvis/TILER2/

        public int GetCount(CharacterBody body)
        {
            if (!body || !body.inventory) { return 0; }

            return body.inventory.GetItemCount(ItemDef);
        }

        public int GetCount(CharacterMaster master)
        {
            if (!master || !master.inventory) { return 0; }

            return master.inventory.GetItemCount(ItemDef);
        }

        public int GetCountSpecific(CharacterBody body, ItemDef itemDef)
        {
            if (!body || !body.inventory) { return 0; }

            return body.inventory.GetItemCount(itemDef);
        }
    }
}