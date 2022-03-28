using System;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using ItemStats;
using ItemStats.Stat;
using ItemStats.ValueFormatters;
using MeltingPot.Items;

namespace MeltingPot.Utils
{
	internal static class ItemStatsCompatability
	{
		internal static void Init() {
			RoR2Application.onLoad = (Action)Delegate.Combine(RoR2Application.onLoad, new Action(ItemStatsCompatability.RegisterItemStatDefs));
		}

		private static void RegisterItemStatDefs() {
			try {
				ItemIndex itemIndex = ContentPackProvider.contentPack.itemDefs.Find("BurningSoul_ItemDef").itemIndex;
				ItemStatDef itemStatDef = new ItemStatDef();
				List<ItemStat> list = new List<ItemStat>();
				list.Add(new ItemStat((float itemCount, StatContext ctx) => ctx.Master ? ctx.Master.GetComponent<HealthComponent>().fullCombinedHealth * BurningSoul.self_burn_percent + BurningSoul.self_burn_percent_growth * (itemCount - 1f) : BurningSoul.self_burn_percent + BurningSoul.self_burn_percent_growth * (itemCount - 1f), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_SelfBurn", new object[]
				  {
					Extensions.FormatPercentage(value, 1, 100f, float.MaxValue, true, "\"red\"")
				  })));
				list.Add(new ItemStat((float itemCount, StatContext ctx) => ctx.Master ? ctx.Master.GetComponent<BurningSoul.BurningSoulController>().getMaxHealth() / 4 * (BurningSoul.enemy_burn_percent + BurningSoul.enemy_burn_percent_growth * (itemCount - 1f)) : 0.25f * (BurningSoul.enemy_burn_percent + BurningSoul.enemy_burn_percent_growth * (itemCount - 1f)), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_EnemyBurn", new object[]
					  {
					Extensions.FormatPercentage(value, 1, 100f, float.MaxValue, true, "\"green\"")
					  })));
				itemStatDef.Stats = list;
				ItemStatsMod.AddCustomItemStatDef(itemIndex, itemStatDef);
			} catch {}
			try {
				ItemIndex itemIndex2 = ContentPackProvider.contentPack.itemDefs.Find("ReverbRing_ItemDef").itemIndex;
				ItemStatDef itemStatDef2 = new ItemStatDef();
				List<ItemStat> list2 = new List<ItemStat>();
				list2.Add(new ItemStat((float itemCount, StatContext ctx) => EchoBang.scaling * (itemCount), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_TotalDamage", new object[]
				{
				Extensions.FormatPercentage(value, 1, 100f, float.MaxValue, true, "\"green\"")
				})));
				list2.Add(new ItemStat((float itemCount, StatContext ctx) => EchoBang.baseRadius + EchoBang.radiusGrowth * (itemCount - 1f), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_Radius", new object[]
				{
				Extensions.FormatInt(value, Language.GetString("ITEMSTATS_MeltingPot_Meter_PF"), 1, false, "\"green\"")
				})));
				itemStatDef2.Stats = list2;
				ItemStatsMod.AddCustomItemStatDef(itemIndex2, itemStatDef2);
			}
			catch { }
			try { 
				ItemIndex itemIndex3 = ContentPackProvider.contentPack.itemDefs.Find("LeadFetters_ItemDef").itemIndex;
				ItemStatDef itemStatDef3 = new ItemStatDef();
				List<ItemStat> list3 = new List<ItemStat>();
				list3.Add(new ItemStat((float itemCount, StatContext ctx) => (float)LeadFetters.armourGrowth * (itemCount), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_Armour", new object[]
				{
					Extensions.FormatInt(value, "", 0, false, "\"green\"")
				})));
				list3.Add(new ItemStat((float itemCount, StatContext ctx) => 1f - Mathf.Clamp(1 / (1 + (itemCount * LeadFetters.knockbackGrowth)), 0, 1), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_KnockBackReduction", new object[]
				{
					Extensions.FormatPercentage(value, 1, 100f, float.MaxValue, true, "\"green\"")
				})));
				itemStatDef3.Stats = list3;
				ItemStatsMod.AddCustomItemStatDef(itemIndex3, itemStatDef3);
			}
			catch { }
			try {
				ItemIndex itemIndex4 = ContentPackProvider.contentPack.itemDefs.Find("MidasCosh_ItemDef").itemIndex;
				ItemStatDef itemStatDef4 = new ItemStatDef();
				List<ItemStat> list4 = new List<ItemStat>();
				list4.Add(new ItemStat((float itemCount, StatContext ctx) => MidasCosh.GoldDuration + MidasCosh.GoldDurationGrowth * (itemCount - 1f), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_DebuffDuration", new object[]
				{
					Extensions.FormatInt(value, Language.GetString("ITEMSTATS_MeltingPot_Seconds_PF"), 0, true, "\"green\"")
				})));
				list4.Add(new ItemStat((float itemCount, StatContext ctx) => Mathf.Clamp(1 - 1 / (1 + MidasCosh.baseProcChance * itemCount), 0, 1), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_MidasChance", new object[]
				{
					Extensions.FormatPercentage(value, 1, 100f, float.MaxValue, true, "\"yellow\"")
				})));
				itemStatDef4.Stats = list4;
				ItemStatsMod.AddCustomItemStatDef(itemIndex4, itemStatDef4);
			}
			catch { }
			try {
				ItemIndex itemIndex5 = ContentPackProvider.contentPack.itemDefs.Find("RageToxin_ItemDef").itemIndex;
				ItemStatDef itemStatDef5 = new ItemStatDef();
				List<ItemStat> list5 = new List<ItemStat>();
				list5.Add(new ItemStat((float itemCount, StatContext ctx) => RageToxin.baseDuration + RageToxin.durationGrowth * (itemCount - 1f), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_DebuffDuration", new object[]
				{
					Extensions.FormatInt(value, Language.GetString("ITEMSTATS_MeltingPot_Seconds_PF"), 0, true, "\"green\"")
				})));
				list5.Add(new ItemStat((float itemCount, StatContext ctx) => Mathf.Clamp(1 - 1 / (1 + RageToxin.fumbleChance + (RageToxin.fumbleGrowth * itemCount - 1)), 0, 1), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_FumbleChance", new object[]
				{
					Extensions.FormatPercentage(value, 1, 100f, float.MaxValue, false, "\"green\"")
				})));
				itemStatDef5.Stats = list5;
				ItemStatsMod.AddCustomItemStatDef(itemIndex5, itemStatDef5);
			}
			catch { }
			try {
				ItemIndex itemIndex6 = ContentPackProvider.contentPack.itemDefs.Find("ReactiveArmour_ItemDef").itemIndex;
				ItemStatDef itemStatDef6 = new ItemStatDef();
				List<ItemStat> list7 = new List<ItemStat>();
				list7.Add(new ItemStat((float itemCount, StatContext ctx) => ReactiveArmour.absorbGrowth * (itemCount), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_Absorb", new object[]
				{
					Extensions.FormatPercentage(value, 1, 100f, float.MaxValue, true, "\"blue\"")
				})));
				itemStatDef6.Stats = list7;
				ItemStatsMod.AddCustomItemStatDef(itemIndex6, itemStatDef6);
			}
			catch { }
			try {
				ItemIndex itemIndex7 = ContentPackProvider.contentPack.itemDefs.Find("ScrapVamp_ItemDef").itemIndex;
				ItemStatDef itemStatDef7 = new ItemStatDef();
				List<ItemStat> list8 = new List<ItemStat>();
				list8.Add(new ItemStat((float itemCount, StatContext ctx) => ScrapVamp.armourPerStack * (itemCount), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_ArmourPerStack", new object[]
				{
					Extensions.FormatInt(value, "", 1, false, "\"blue\"")
				})));
				list8.Add(new ItemStat((float itemCount, StatContext ctx) => ScrapVamp.healthPerStack * (itemCount), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_HealthPerStack", new object[]
				{
					Extensions.FormatInt(value, "", 1, false, "\"green\"")
				})));
				list8.Add(new ItemStat((float itemCount, StatContext ctx) => ScrapVamp.damagePerStack * (itemCount), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_DamagePerStack", new object[]
				{
					Extensions.FormatInt(value, "", 1, false, "\"yellow\"")
				})));
				list8.Add(new ItemStat((float itemCount, StatContext ctx) => MidasCosh.GoldDuration + MidasCosh.GoldDurationGrowth * (itemCount - 1f), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_DebuffDuration", new object[]
				{
					Extensions.FormatInt(value, Language.GetString("ITEMSTATS_MeltingPot_Seconds_PF"), 0, true, "\"green\"")
				})));
				list8.Add(new ItemStat((float itemCount, StatContext ctx) => (float) 0.75f + ScrapVamp.damageMalusGrowth*(itemCount-1), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_DroneDamageGrowth", new object[]
				{
					Extensions.FormatPercentage(value, 0, 100f, float.MaxValue, true, "\"red\"")
				})));
				list8.Add(new ItemStat((float itemCount, StatContext ctx) => (float)1f + ScrapVamp.healthGrowth * (itemCount - 1), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_DroneHealthGrowth", new object[]
				{
					Extensions.FormatPercentage(value, 0, 100f, float.MaxValue, true, "\"red\"")
				})));
				list8.Add(new ItemStat((float itemCount, StatContext ctx) => (float)1f + ScrapVamp.asGrowth * (itemCount - 1), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_DroneASGrowth", new object[]
				{
					Extensions.FormatPercentage(value, 0, 100f, float.MaxValue, true, "\"red\"")
				})));
				itemStatDef7.Stats = list8;
				ItemStatsMod.AddCustomItemStatDef(itemIndex7, itemStatDef7);
			}
			catch { }
			try {
				ItemIndex itemIndex8 = ContentPackProvider.contentPack.itemDefs.Find("ThornShield_ItemDef").itemIndex;
				ItemStatDef itemStatDef8 = new ItemStatDef();
				List<ItemStat> list9 = new List<ItemStat>();
				list9.Add(new ItemStat((float itemCount, StatContext ctx) => ThornShield.flatArmour * itemCount, (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_Armour", new object[]
				{
					Extensions.FormatInt(value, "", 0, true, "\"green\"")
				})));
				list9.Add(new ItemStat((float itemCount, StatContext ctx) => (ctx.Master.GetComponent<CharacterBody>() ? ThornShield.flatDamage + ThornShield.flatStack * (itemCount - 1) + ThornShield.armourGrowth* itemCount*ctx.Master.GetComponent<CharacterBody>().armor : ThornShield.flatDamage + ThornShield.flatStack * (itemCount - 1) + ThornShield.armourGrowth*itemCount), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_Retaliation", new object[]
				{
					Extensions.FormatInt(value, "", 0, true, "\"green\"")
				})));
				itemStatDef8.Stats = list9;
				ItemStatsMod.AddCustomItemStatDef(itemIndex8, itemStatDef8);
			}
			catch { }
			try {
				ItemIndex itemIndex9 = ContentPackProvider.contentPack.itemDefs.Find("MechMosquito_ItemDef").itemIndex;
				ItemStatDef itemStatDef9 = new ItemStatDef();
				List<ItemStat> list10 = new List<ItemStat>();
				list10.Add(new ItemStat((float itemCount, StatContext ctx) => MechMosquito.bleedChance * (itemCount + 1), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_BleedChance", new object[]
				{
					Extensions.FormatPercentage(value, 0, 100f, float.MaxValue, true, "\"red\"")
				})));
				list10.Add(new ItemStat((float itemCount, StatContext ctx) => MechMosquito.drainGrowth *(itemCount), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_HealPercent", new object[]
				{
					Extensions.FormatPercentage(value, 0, 100f, float.MaxValue, true, "\"green\"")
				})));
				itemStatDef9.Stats = list10;
				ItemStatsMod.AddCustomItemStatDef(itemIndex9, itemStatDef9);
			}
			catch { }
			try {
				ItemIndex itemIndex10 = ContentPackProvider.contentPack.itemDefs.Find("PenitFang_ItemDef").itemIndex;
				ItemStatDef itemStatDef10 = new ItemStatDef();
				List<ItemStat> list11 = new List<ItemStat>();
				list11.Add(new ItemStat((float itemCount, StatContext ctx) =>  (1 - Mathf.Clamp(1 / (1 + (itemCount * PenitentsFang.poisonChance)), 0, 1)), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_PoisonChance", new object[]
				{
					Extensions.FormatPercentage(value, 0, 100f, float.MaxValue, true, "\"green\"")
				})));
				itemStatDef10.Stats = list11;
				ItemStatsMod.AddCustomItemStatDef(itemIndex10, itemStatDef10);
			}
			catch { }
			try {
				ItemIndex itemIndex11 = ContentPackProvider.contentPack.itemDefs.Find("FesterFang_ItemDef").itemIndex;
				ItemStatDef itemStatDef11 = new ItemStatDef();
				List<ItemStat> list12 = new List<ItemStat>();
				list12.Add(new ItemStat((float itemCount, StatContext ctx) => FesteringFang.blightChance * itemCount, (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_BlightChance", new object[]
			   {
					Extensions.FormatPercentage(value, 0, 100f, float.MaxValue, true, "\"green\"")
			   })));
				itemStatDef11.Stats = list12;
				ItemStatsMod.AddCustomItemStatDef(itemIndex11, itemStatDef11);
			}
			catch { }
			try {
				ItemIndex itemIndex12 = ContentPackProvider.contentPack.itemDefs.Find("SappingBloom_ItemDef").itemIndex;
				ItemStatDef itemStatDef12 = new ItemStatDef();
				List<ItemStat> list13 = new List<ItemStat>();
				list13.Add(new ItemStat((float itemCount, StatContext ctx) => (1 - Mathf.Clamp(1 / (1 + (itemCount * SappingBloom.weakenChance)), 0, 1)), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_WeakenChance", new object[]
			   {
					Extensions.FormatPercentage(value, 0, 100f, float.MaxValue, true, "\"green\"")
			   })));
				itemStatDef12.Stats = list13;
				ItemStatsMod.AddCustomItemStatDef(itemIndex12, itemStatDef12);
			}
			catch { }
			try {
				ItemIndex itemIndex13 = ContentPackProvider.contentPack.itemDefs.Find("SerratedPellets_ItemDef").itemIndex;
				ItemStatDef itemStatDef13 = new ItemStatDef();
				List<ItemStat> list14 = new List<ItemStat>();
				list14.Add(new ItemStat((float itemCount, StatContext ctx) => Math.Max(SerratedPellets.bleedStackBase - (itemCount - 1) * SerratedPellets.stackDegen, 0), (float value, StatContext ctx) => Language.GetStringFormatted("ITEMSTATS_MeltingPot_BleedStacks", new object[]
			   {
				Extensions.FormatInt(value, "", 0, true, "\"green\"")
			   })));
				itemStatDef13.Stats = list14;
				ItemStatsMod.AddCustomItemStatDef(itemIndex13, itemStatDef13);
			}
			catch { }
		}
	}
}
