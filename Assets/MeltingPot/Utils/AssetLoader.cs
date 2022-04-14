using MeltingPot.Items;
using RoR2;
using RoR2.ContentManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Path = System.IO.Path;

namespace MeltingPot.Utils
{
    public static class Assets
    {
        public static AssetBundle mainAssetBundle = null;

        //the filename of your assetbundle
        internal static string assetBundleName = "meltingpotassets";

        internal static string assemblyDir
        {
            get { return Path.GetDirectoryName(MeltingPotPlugin.pluginInfo.Location); }
        }

        public static void PopulateAssets()
        {
            mainAssetBundle = AssetBundle.LoadFromFile(Path.Combine(assemblyDir, assetBundleName));
            ContentPackProvider.serializedContentPack =
                mainAssetBundle.LoadAsset<SerializableContentPack>(
                    ContentPackProvider.contentPackName
                );
        }

        public static string soundBankDirectory =>
            System.IO.File.Exists(
                System.IO.Path.Combine(Assets.assemblyDir, "MeltingPot_SoundBank.bnk")
            )
              ? System.IO.Path.Combine(Assets.assemblyDir, "MeltingPot_SoundBank.bnk")
              : System.IO.Path.Combine(Assets.assemblyDir, "Soundbanks/MeltingPot_SoundBank.bnk");

        [RoR2.SystemInitializer] //look at putting it in FinalizeAsync
        public static void InitializeSoundbanks()
        {
            Debug.Log("Initialising Melting Pot : Soundbanks");
            R2API.SoundAPI.SoundBanks.Add(File.ReadAllBytes(soundBankDirectory));
        }
    }

    public class ContentPackProvider : IContentPackProvider
    {
        public static BepInEx.Logging.ManualLogSource BSModLogger;
        public static SerializableContentPack serializedContentPack;
        public static ContentPack contentPack;

        //Should be the same names as your SerializableContentPack in the asset bundle
        public static string contentPackName = "meltingpotcontentpack";

        public string identifier
        {
            get { return "Melting Pot"; }
        }

        internal static void Initialize()
        {
            contentPack = serializedContentPack.CreateContentPack();
        }

        internal static void Re_Initilize(List<ItemBase> Items)
        {
            serializedContentPack.itemDefs = Items
                .Where(x => x.Enabled)
                .Select(C => C.ItemDef)
                .ToArray()
                .Concat(
                    serializedContentPack.itemDefs
                        .Where(x => x.tier.Equals(ItemTier.NoTier))
                        .ToArray()
                )
                .ToArray();
            contentPack = serializedContentPack.CreateContentPack();
            ContentManager.collectContentPackProviders += AddCustomContent;
        }

        private static void AddCustomContent(
            ContentManager.AddContentPackProviderDelegate addContentPackProvider
        )
        {
            addContentPackProvider(new ContentPackProvider());
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            Assets.InitializeSoundbanks();
            args.ReportProgress(1f);
            yield break;
        }
    }
}
