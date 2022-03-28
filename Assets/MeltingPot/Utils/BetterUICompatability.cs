using BetterUI;
using System;
using RoR2;

namespace MeltingPot.Utils
{
	internal static class BetterUICompatability
	{
		internal static void Init() {
			RoR2Application.onLoad = (Action)Delegate.Combine(RoR2Application.onLoad, new Action(BetterUICompatability.RegisterBuffInfos));
			// No other changes necessary at present
		}

		private static void RegisterBuffInfos() {
			foreach (BuffDef buffDef in ContentPackProvider.contentPack.buffDefs) {
				bool flag = buffDef != null;
				if (flag) {
					string str = "BUFF_" + buffDef.name + "_";
					Buffs.RegisterBuffInfo(buffDef, str + "NAME", str + "DESC");
				}
			}
		}

	}
}
