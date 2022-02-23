using RoR2.ContentManagement;
using System.Collections;
using System.Reflection;
using UnityEngine;
using Path = System.IO.Path;

namespace MeltingPot.Utils
{
	public static class Assets
	{
		public static AssetBundle mainAssetBundle = null;
		//the filename of your assetbundle
		internal static string assetBundleName = "meltingpotassets";

		internal static string assemblyDir {
			get {
				return Path.GetDirectoryName(MeltingPotPlugin.pluginInfo.Location);
			}
		}

		public static void PopulateAssets() {
			mainAssetBundle = AssetBundle.LoadFromFile(Path.Combine(assemblyDir, assetBundleName));
			ContentPackProvider.serializedContentPack = mainAssetBundle.LoadAsset<SerializableContentPack>(ContentPackProvider.contentPackName);
		}

		public static string soundBankDirectory => System.IO.Path.Combine(Assets.assemblyDir, "Soundbanks");

		[RoR2.SystemInitializer] //look at putting it in FinalizeAsync
		public static void InitializeSoundbanks() {
			Debug.Log("Initialising Melting Pot : Soundbanks");
			uint akBankID;  // Not used. These banks can be unloaded with their file name.
			AkSoundEngine.AddBasePath(soundBankDirectory);
			AkSoundEngine.LoadBank("MeltingPot_SoundBank.bnk", -1, out akBankID);
		}
	}

	public class ContentPackProvider : IContentPackProvider
	{
		public static BepInEx.Logging.ManualLogSource BSModLogger;
		public static SerializableContentPack serializedContentPack;
		public static ContentPack contentPack;
		//Should be the same names as your SerializableContentPack in the asset bundle
		public static string contentPackName = "meltingpotcontentpack";

		public string identifier {
			get {
				return "Melting Pot";
			}
		}

		internal static void Initialize() {
			contentPack = serializedContentPack.CreateContentPack();
			ContentManager.collectContentPackProviders += AddCustomContent;
		}

		private static void AddCustomContent(ContentManager.AddContentPackProviderDelegate addContentPackProvider) {
			addContentPackProvider(new ContentPackProvider());
		}

		public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args) {
			args.ReportProgress(1f);
			yield break;
		}

		public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args) {
			ContentPack.Copy(contentPack, args.output);
			args.ReportProgress(1f);
			yield break;
		}

		public IEnumerator FinalizeAsync(FinalizeAsyncArgs args) {
			Assets.InitializeSoundbanks();
			args.ReportProgress(1f);
			yield break;
		}
	}
}