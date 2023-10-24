global using static KitchenHQ.Main;
global using static KitchenLib.Utils.GDOUtils;
global using static KitchenLib.Utils.LocalisationUtils;
using Kitchen;
using KitchenData;
using KitchenHQ.Franchise;
using KitchenHQ.Utility;
using KitchenLib;
using KitchenLib.Customs;
using KitchenLib.Event;
using KitchenLib.Utils;
using KitchenMods;
using PreferenceSystem;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using static KitchenHQ.Franchise.ModFranchise;

namespace KitchenHQ
{
    public class Main : BaseMod
    {
        public const string NAME = "Integrated HQ";
        public const string GUID = "nova.integrated-hq";
        public const string VERSION = "1.0.3";

        public Main() : base(GUID, NAME, "Zoey Davis", VERSION, ">=1.0.0", Assembly.GetExecutingAssembly()) { }

        private static AssetBundle Bundle;
        public static PreferenceSystemManager PrefManager;

        private void PostActivate()
        {
            SetupMenu();
            SetupSettings();
            EmbedUtility.PrintEmbedResourceNames();
            FileUtility.InitFiles();
            AddViewTypes();
            AddMaterials();
        }

        private void BuildGameData(GameData gameData)
        {
            SetupLayout(gameData);
            ModifyBasePrefabs(gameData);
            AddLocalisations(gameData);
            GDOContainer.SetupDishes(gameData);
        }

        public void AddMaterials()
        {
            Bundle.LoadAllAssets<Texture2D>();
            Bundle.LoadAllAssets<Sprite>();

            #region Rug - Vine
            var tex = GetAsset<Texture2D>("VineTex");
            var mat = MaterialUtils.CreateFlat("Rug - Vine", 0xA86025, overlayScale: 1f);
            mat.SetTexture("_Overlay", tex);
            mat.SetColor("_OverlayColour", MaterialUtils.ColorFromHex(0x333333));
            mat.SetVector("_OverlayTextureScale", new(10, 10, 0, 0));
            mat.SetInt("_HasTextureOverlay", 1);
            mat.EnableKeyword("_HASTEXTUREOVERLAY_ON");
            mat.SetInt("_TextureByUV", 1);
            mat.EnableKeyword("_TEXTUREBYUV_ON");
            mat.SetFloat("_OverlayMax", 1f);
            mat.SetFloat("_OverlayMin", 1f);
            AddMaterial(mat);
            #endregion
        }

        #region Menu
        private void SetupMenu()
        {
            PrefManager = new(GUID, "Integrated HQ");

            PrefManager
                .AddInfo("Any changes will require a restart.")
                .AddSubmenu("HQ", "HQMenu")
                    .AddLabel("Auto-load Tape in TV")
                    .AddOption("VHSInTV", true, new bool[] { false, true }, new string[] { "Disabled", "Enabled" })
                    .AddLabel("Use Legacy HQ")
                    .AddOption("LegacyHQEnabled", false, new bool[] { false, true }, new string[] { "Disabled", "Enabled" })
                .SubmenuDone()
                .AddSubmenu("Developer", "DevMenu")
                    .AddLabel("Show developer logs")
                    .AddOption("DeveloperMode", false, new bool[] { false, true }, new string[] { "Disabled", "Enabled" })
                    .AddLabel("Show room anchors")
                    .AddOption("ShowRoomAnchors", false, new bool[] { false, true }, new string[] { "Disabled", "Enabled" })
                    .AddLabel("Add Example Appliances")
                    .AddOption("ShowExampleAppRoom", false, new bool[] { false, true }, new string[] { "Disabled", "Enabled" })
                .SubmenuDone()
                .AddSubmenu("Seasonal Decorations", "SeasonalMenu")
                    .AddLabel("Enable Seasonal Decorations")
                    .AddOption("AllowSeasonalDeco", true, new bool[] { false, true }, new string[] { "Disabled", "Enabled" }, true)
                    .AddLabel($"<size=80%>Current:</size> <color=#878787>{SeasonalToString(CurrentSeasonal())}</color>")
                    .AddConditionalBlocker(() => !PrefManager.Get<bool>("AllowSeasonalDeco") || true) // Revert whenever you release more seasonals
                        .AddLabel("Force Seasonal Decorations")
                        .AddOption("ForceSeasonalDeco", false, new bool[] { false, true }, new string[] { "Disabled", "Enabled" }, true)
                    .ConditionalBlockerDone()
                    .AddConditionalBlocker(() => !PrefManager.Get<bool>("ForceSeasonalDeco") || !PrefManager.Get<bool>("AllowSeasonalDeco"))
                        .AddOption("ForcedSeasonalDecoType", 5, new int[]
                        { 1, 2, 3, 4, 5, 6, 7 }, new string[] 
                        { "Chinese New Year", "Valentines", "Easter", "April Fools", "Halloween", "Thanksgiving", "Christmas" }, true)
                    .ConditionalBlockerDone()
                .SubmenuDone()
                .AddSpacer()
                .AddLabel("Allow Network API Calls")
                .AddInfo("Disabling reduces the amount of data gathered from the internet and  <i>may</i> improve load times.")
                .AddOption("AllowAPI", true, new bool[] { false, true }, new string[] { "Disabled", "Enabled" });

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
        private void AddViewTypes()
        {
            TapeEditorCustomView = AddViewType("Tape Editor", GetPrefab("VHS Writer Indicator").TryAddComponent<TapeEditorView>().gameObject);

            EventboardIndicatorView = AddViewType("Event Indicator", () =>
            {
                var prefab = GetPrefab("Event Board Indicator");
                prefab.GetChild("Container/Body").transform.CreateLabel("Title", new(0f, 0f, 0f), Quaternion.identity,
                MaterialUtils.GetExistingMaterial("Cake n Truffles Atlas Material"), FontUtils.GetExistingTMPFont("Map Label"), 0, 2.25f,
                "Event Title!");
                return prefab;
            });
        }

        protected override void OnInitialise()
        {
            if (PrefManager.Get<bool>("ShowExampleAppRoom"))
                ExampleAPI.ShowExamples();
        }

        protected override void OnPostActivate(Mod mod)
        {
            // Bundle loading
            Bundle = mod.GetPacks<AssetBundleModPack>().SelectMany(e => e.AssetBundles).First();

            Bundle.LoadAllAssets<Texture2D>();
            Bundle.LoadAllAssets<Sprite>();

            PostActivate(); // Extra actions to be performed pre-BGD

            AddGameData(); // Adds all GDOs in the Assembly

            AddIcons(); // Creates the icons ingame

            Events.BuildGameDataEvent += (s, args) => BuildGameData(args.gamedata); // Actions to be performed during BGD
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

        internal void AddIcons()
        {
            Bundle.LoadAllAssets<Texture2D>();
            Bundle.LoadAllAssets<Sprite>();

            var icons = Bundle.LoadAsset<TMP_SpriteAsset>("Icon Asset");
            TMP_Settings.defaultSpriteAsset.fallbackSpriteAssets.Add(icons);
            icons.material = Object.Instantiate(TMP_Settings.defaultSpriteAsset.material);
            icons.material.mainTexture = Bundle.LoadAsset<Texture2D>("Icon Texture");

            Log("Registered icons");
        }

        // Generic interface to disallow GDO registering
        public interface IWontRegister { }
        #endregion

        #region Logging
        internal static void LogInfo(string msg) { Debug.Log($"[{NAME}] " + msg); }
        internal static void LogWarning(string msg) { Debug.LogWarning($"[{NAME}] " + msg); }
        internal static void LogError(string msg) { Debug.LogError($"[{NAME}] " + msg); }
        internal static void LogInfo(object msg) { LogInfo(msg.ToString()); }
        internal static void LogWarning(object msg) { LogWarning(msg.ToString()); }
        internal static void LogError(object msg) { LogError(msg.ToString()); }

        internal static void LogDebug(string msg) { if (PrefManager.Get<bool>("DeveloperMode")) Debug.Log($"[{NAME}] [DEBUG] " + msg); }
        internal static void LogDebug(object msg) { LogDebug(msg.ToString()); }
        #endregion

        #region Utility
        public static T GetGDO<T>(int id) where T : GameDataObject => GetExistingGDO(id) as T;

        public static GameObject GetPrefab(string name) => Bundle.LoadAsset<GameObject>(name);
        public static T GetAsset<T>(string name) where T : Object => Bundle.LoadAsset<T>(name);
        #endregion
    }
}
