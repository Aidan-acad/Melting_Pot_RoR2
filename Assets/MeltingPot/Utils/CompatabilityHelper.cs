using BepInEx.Bootstrap;

namespace MeltingPot.Utils
{
	internal static class CompatabilityHelper
	{
		public static BepInEx.Logging.ManualLogSource BSModLogger;
		internal static void Init() {
			BSModLogger = MeltingPotPlugin.ModLogger;
			bool flag = Chainloader.PluginInfos.ContainsKey("com.xoxfaby.BetterUI");
			if (flag) {
				try {
					BetterUICompatability.Init();
					CompatabilityHelper.betterUICompatEnabled = true;
					BSModLogger.LogInfo("Better UI Compat Initialised");
				}
				catch {
				}
			}
			bool flag2 = Chainloader.PluginInfos.ContainsKey("dev.ontrigger.itemstats");
			if (flag2) {
				try {
					ItemStatsCompatability.Init();
					CompatabilityHelper.itemStatsCompatEnabled = true;
					BSModLogger.LogInfo("Item Stats Compat Initialised");
				}
				catch {
				}
			}
		}
		internal static bool betterUICompatEnabled;
		internal static bool itemStatsCompatEnabled;

	}
}
