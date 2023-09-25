global using static KitchenHQ.Main;
global using static KitchenLib.Utils.GDOUtils;
global using static KitchenLib.Utils.LocalisationUtils;
using KitchenHQ.Utility;
using KitchenData;
using KitchenLib;
using KitchenLib.Customs;
using KitchenLib.Event;
using KitchenMods;
using PreferenceSystem;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace KitchenHQ
{
    public class Main : BaseMod
    {
        public const string GUID = "nova.integrated-hq";
        public const string VERSION = "1.0.0";

        public Main() : base(GUID, "Integrated HQ", "Zoey Davis", VERSION, ">=1.0.0", Assembly.GetExecutingAssembly()) { }

        private static AssetBundle Bundle;
        public static PreferenceSystemManager PrefManager;

        private void PostActivate()
        {
            SetupMenu();
        }

        private void BuildGameData(GameData gameData)
        {
            ModFranchise.SetupLayout(gameData);
            ModFranchise.ModifyBasePrefabs(gameData);
            AddLocalisations(gameData);
            GDOContainer.SetupDishes(gameData);
        }

        #region Menu
        private void SetupMenu()
        {
            PrefManager = new(GUID, "Integrated HQ");

            PrefManager
                .AddLabel("Integrated HQ")
                .AddInfo("Any changes will require a restart.")
                .AddSubmenu("HQ", "HQMenu")
                    .AddLabel("Use Legacy HQ")
                    .AddOption("LegacyHQEnabled", false, new bool[] { false, true }, new string[] { "Disabled", "Enabled" })
                .SubmenuDone()
                .AddSubmenu("Developer", "DevMenu")
                    .AddLabel("Show developer logs")
                    .AddOption("DeveloperMode", false, new bool[] { false, true }, new string[] { "Disabled", "Enabled" })
                    .AddLabel("Show room anchors")
                    .AddOption("ShowRoomAnchors", false, new bool[] { false, true }, new string[] { "Disabled", "Enabled" })
                .SubmenuDone();

            PrefManager.RegisterMenu(PreferenceSystemManager.MenuType.MainMenu);
            PrefManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);
        }
        #endregion

        #region Localisation
        private static Dictionary<string, List<(Locale locale, string text)>> Localisations = new();

        public static void AddLocalisation(string tag, List<(Locale, string)> localised) => Localisations.Add(tag, localised);

        private static void AddLocalisations(GameData gameData)
        {
            Dictionary<Locale, List<(string, string)>> localisations = new();
            var globalText = gameData.GlobalLocalisation.Text;

            foreach (var cgdoPair in CustomGDO.GDOs)
            {
                if (!(cgdoPair.Value is ISpecialLocalise localiser))
                    continue;

                foreach (var localisationPair in localiser.Localisations)
                {
                    if (Localisations.ContainsKey(localisationPair.Key))
                        continue;

                    Localisations.Add(localisationPair.Key, localisationPair.Value);
                }
            }

            foreach (var localisationPair in Localisations)
            {
                if (!globalText.ContainsKey(localisationPair.Key))
                    globalText.Add(localisationPair.Key, localisationPair.Value[0].text);

                foreach (var localePair in localisationPair.Value)
                {
                    if (!localisations.ContainsKey(localePair.locale))
                        localisations.Add(localePair.locale, new());

                    localisations[localePair.locale].Add((localisationPair.Key, localePair.text));
                }
            }

            var globalInfo = gameData.GlobalLocalisation.Info;
            foreach (var localePair in localisations)
            {
                if (!globalInfo.Has(localePair.Key))
                    globalInfo.Add(localePair.Key, CreateDictionaryInfo(new()));

                var info = globalInfo.Get(localePair.Key);
                foreach (var textPair in localePair.Value)
                {
                    if (info.Text.ContainsKey(textPair.Item1))
                        continue;

                    info.Text.Add(textPair.Item1, textPair.Item2);
                }
            }
        }

        public interface ISpecialLocalise
        {
            public Dictionary<string, List<(Locale locale, string text)>> Localisations { get; }
        }
        #endregion

        #region Registry
        protected override void OnPostActivate(Mod mod)
        {
            // Bundle loading
            Bundle = mod.GetPacks<AssetBundleModPack>().SelectMany(e => e.AssetBundles).First();

            // Preload any textures
            Bundle.LoadAllAssets<Texture2D>();
            Bundle.LoadAllAssets<Sprite>();

            PostActivate(); // Extra actions to be performed pre-BGD

            AddGameData(); // Adds all GDOs in the Assembly

            Events.BuildGameDataEvent += (s, args) =>
            {
                BuildGameData(args.gamedata); // Actions to be performed during BGD
            };
        }

        internal void AddGameData()
        {
            MethodInfo AddGDOMethod = typeof(BaseMod).GetMethod(nameof(BaseMod.AddGameDataObject));
            int counter = 0;
            Log("Registering GameDataObjects.");
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                // Ignore abstract and non-registering types
                if (type.IsAbstract || typeof(IWontRegister).IsAssignableFrom(type))
                    continue;

                // Make sure type is a Custom GDO
                if (!typeof(CustomGameDataObject).IsAssignableFrom(type))
                    continue;

                // Invoke and add
                MethodInfo generic = AddGDOMethod.MakeGenericMethod(type);
                generic.Invoke(this, null);
                counter++;
            }
            Log($"Registered {counter} GameDataObjects.");
        }

        // Generic interface to disallow GDO registering
        public interface IWontRegister { }
        #endregion

        #region Logging
        internal static void LogInfo(string msg) { Debug.Log($"[{GUID}] " + msg); }
        internal static void LogWarning(string msg) { Debug.LogWarning($"[{GUID}] " + msg); }
        internal static void LogError(string msg) { Debug.LogError($"[{GUID}] " + msg); }
        internal static void LogInfo(object msg) { LogInfo(msg.ToString()); }
        internal static void LogWarning(object msg) { LogWarning(msg.ToString()); }
        internal static void LogError(object msg) { LogError(msg.ToString()); }

        internal static void LogDebug(string msg) { if (PrefManager.Get<bool>("DeveloperMode")) Debug.Log($"[{GUID}] [DEBUG] " + msg); }
        internal static void LogDebug(object msg) { LogDebug(msg.ToString()); }
        #endregion

        #region Utility
        public static T GetGDO<T>(int id) where T : GameDataObject => GetExistingGDO(id) as T;

        public static GameObject GetPrefab(string name) => Bundle.LoadAsset<GameObject>(name);
        public static T GetAsset<T>(string name) where T : Object => Bundle.LoadAsset<T>(name);
        #endregion
    }
}
