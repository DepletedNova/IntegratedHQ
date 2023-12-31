﻿global using static KitchenHQ.Franchise.ModRoomReferences;
using Kitchen;
using Kitchen.Layouts;
using Kitchen.Layouts.Modules;
using KitchenData;
using KitchenHQ.Utility;
using KitchenLib.Utils;
using KitchenLib.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using XNode;
using WebUtility = KitchenHQ.Utility.WebUtility;

namespace KitchenHQ.Franchise
{
    public static class ModdedLobbyPositionAnchors
    {
        public static readonly Vector3 ModRoomAnchor = new(-1f, 0f, 6f);

        public static readonly Vector3 Office = new(-1f, 0f, -3f);

        public static readonly Vector3 Kitchen = new(6f, 0f, 2f);

        public static readonly Vector3 Contracts = new(-7f, 0f, 3f);

        public static readonly Vector3 Garage = new(-13f, 0f, -9f);

        public static readonly Vector3 Workshop = new(-13f, 0, -3f);

        public static readonly Vector3 StartMarker = new(-10.5f, 0f, -8.5f);

        public static readonly Vector3 Stats = new(9f, 0f, -4f);

        public static readonly List<Vector3> Bedrooms = new List<Vector3>
        {
            new(10.5f, 0f, 7f),
            new(3.5f, 0f, 7f),
            new(10.5f, 0f, 4f),
            new(3.5f, 0f, 4f)
        };
    }

    // Headquarter modifications
    public static class ModFranchise
    {
        #region Setup Layout
        // Setup Anchors & Layout
        internal static void SetupLayout(GameData gameData)
        {
            if (PrefManager.Get<bool>("LegacyHQEnabled"))
                return;

            // Update rooms and features in the Franchise layout
            var franchiseLayout = gameData.ReferableObjects.FranchiseLayout;
            foreach (var node in franchiseLayout.nodes)
            {
                node.TrySetupHQFeatures();
                node.TrySetupHQRooms();
            }

            // Update layout anchors
            LobbyPositionAnchors.Office = new(-1f, 0f, -3f);
            LobbyPositionAnchors.Kitchen = new(6f, 0f, -1f);
            LobbyPositionAnchors.Contracts = new(-5f, 0f, 6f);
            LobbyPositionAnchors.Garage = new(-11f, 0f, -2f);
            LobbyPositionAnchors.Workshop = new(-8f, 0, 5f);
            LobbyPositionAnchors.StartMarker = new(-10.5f, 0f, -8.5f);
            LobbyPositionAnchors.Stats = new(9f, 0f, -4f);

            LobbyPositionAnchors.Bedrooms[0] = new(12f, 0f, 7f);
            LobbyPositionAnchors.Bedrooms[1] = new(5f, 0f, 7f);
            LobbyPositionAnchors.Bedrooms[2] = new(12f, 0f, 4f);
            LobbyPositionAnchors.Bedrooms[3] = new(5f, 0f, 4f);
        }

        // Update room tex & colour maps
        private static void TrySetupHQRooms(this Node node)
        {
            if (!(node is NewFromTexture PUNode))
                return;

            var roomTex = GetAsset<Texture2D>("RoomsTex");
            PUNode.SourceTexture = roomTex;
            PUNode.Map.Add(new()
            {
                Color = MaterialUtils.ColorFromHex(0xFF417D),
                Room = new()
                {
                    ID = VariousUtils.GetID("HQ-Workshop"),
                    Type = RoomType.Office
                }
            });
        }

        // Update feature tex & colour maps
        private static void TrySetupHQFeatures(this Node node)
        {
            if (!(node is FeaturesFromTexture PUNode))
                return;

            var featureTex = GetAsset<Texture2D>("FeaturesTex");
            PUNode.SourceTexture = featureTex;
            PUNode.Map.Add(new()
            {
                Color = MaterialUtils.ColorFromHex(0xF5A15D),
                Feature = FeatureType.MissingDoor
            });
        }
        #endregion

        #region Modify Base Prefabs
        internal static void ModifyBasePrefabs(GameData gameData)
        {
            if (PrefManager.Get<bool>("LegacyHQEnabled"))
                return;

            ModifyLockedContracts(gameData.Get<Appliance>(AssetReference.ContractLock).Prefab);
        }

        private static void ModifyLockedContracts(GameObject prefab)
        {
            var flooring = prefab.GetChild("Cube (1)");
            flooring.transform.localScale = new(4f, 0f, 5f);
            flooring.transform.localPosition = new(0f, 0.1f, 0f);

            prefab.transform.Find("Cube (2)").localPosition -= Vector3.forward * 0.5f;
            prefab.transform.Find("Crate").localPosition -= Vector3.forward * 0.5f;
            prefab.transform.Find("Crate (1)").localPosition -= Vector3.forward * 0.5f;
        }
        #endregion

        #region Seasonal Decoration
        public enum SeasonalLobby
        {
            None,
            NewYear,
            Valentines,
            Easter,
            AprilFools,
            Halloween,
            Thanksgiving,
            Christmas
        }

        public static string SeasonalToString(SeasonalLobby lobby) => lobby switch
            {
                SeasonalLobby.None => "None!",
                SeasonalLobby.NewYear => "Chinese New Year",
                SeasonalLobby.Valentines => "Valentines",
                SeasonalLobby.Easter => "Easter",
                SeasonalLobby.AprilFools => "April Fools",
                SeasonalLobby.Halloween => "Halloween",
                SeasonalLobby.Thanksgiving => "Thanksgiving",
                SeasonalLobby.Christmas => "Christmas",
                _ => "None!"
            };

        public static SeasonalLobby CurrentSeasonal()
        {
            if (!PrefManager.Get<bool>("AllowSeasonalDeco"))
                return SeasonalLobby.None;
            if (PrefManager.Get<bool>("ForceSeasonalDeco"))
                return (SeasonalLobby)PrefManager.Get<int>("ForcedSeasonalDecoType");

            var date = DateTime.UtcNow;

            if ((date.Month == 1 && date.Day <= 20) || (date.Month == 12 && date.Day >= 26))
                return SeasonalLobby.NewYear;
            if (date.Month == 2 && date.Day > 5 && date.Day < 25)
                return SeasonalLobby.Valentines;
            if (date.Month == 3 && date.Day > 15)
                return SeasonalLobby.Easter;
            if (date.Month == 4 && date.Day == 1)
                return SeasonalLobby.AprilFools;
            if (date.Month == 10)
                return SeasonalLobby.Halloween;
            if (date.Month == 11 && date.Day > 10)
                return SeasonalLobby.Thanksgiving;
            if (date.Month == 12)
                return SeasonalLobby.Christmas;

            return SeasonalLobby.None;
        }
        #endregion
    }

    #region Appliance References
    public static class FranchiseReferences
    {
        // Garage
        public const int OpenGarage = 599275099;
        public const int ClosedGarage = -2093978702;
        public const int LoadingBayText = 1206857296;
        public const int GarageMarker = 2071332010;
        public const int WorkshopMarker = -1145577711;

        // Office
        public const int OfficeShortWall = -1011656387;
        public const int ThematicBoardVisual = 870916113;
        public const int TimedBoardVisual = 1742137083;
        public const int ModdedFoodsText = -511278401;
        public const int FoodSelector = 1488723450;
        public const int Arch = 2037686303;
        public const int Refresher = 658296198;

        // Stats
        public const int ModdedUpgradesView = -597932929;
        public const int SubscribedModsView = 498895759;
        public const int CreatedModsView = 1228220591;

        // Generic
        public const int RoomSwapper = -1366052815;
        public const int ApplianceSwapper = 1637849031;
        public const int DebugMarker = -1909067557;
        public const int ModDebugMarker = -413176971;
    }
    #endregion

    #region Mod Room References
    public static class ModRoomReferences
    {
        public static CustomViewType EventboardIndicatorView { get; internal set; }
        public static CustomViewType TapeEditorCustomView { get; internal set; }
        public static readonly ItemCategory TapeItemCategory = (ItemCategory)32768;

        [Flags]
        public enum TapeTypes
        {
            Newest = 1,
            Trending = 2,
            Tagged = 4,
            Creator = 8,
            Search = 16,
        }

        public static CloudSettings Settings { get; private set; }

        private const string CloudURL = "https://raw.githubusercontent.com/DepletedNova/IntegratedHQ/master/CloudSettings.json";
        internal static void SetupSettings()
        {
            if (Settings != null)
                return;

            if (PrefManager.Get<bool>("AllowAPI"))
            {
                LogDebug("[CLOUD] Gathering dynamic info from the cloud");
                using WebClient wc = new();
                bool hasSettings = WebUtility.AwaitTask(wc.DownloadStringTaskAsync(CloudURL), out string settings);
                if (!hasSettings)
                {
                    LogWarning("[CLOUD] Failed to read data dynamic info. Reverting to defaults.");
                    settings = EmbedUtility.ReadEmbeddedTextFile("DefaultSettings.json");
                }
                else LogDebug("[CLOUD] Successfully read data from the cloud.");
                Settings = JsonConvert.DeserializeObject<CloudSettings>(settings);
                return;
            }
            Settings = JsonConvert.DeserializeObject<CloudSettings>(EmbedUtility.ReadEmbeddedTextFile("DefaultSettings.json"));
        }
    }

    public class CloudSettings
    {
        public STape VHS;

        public List<CEventData> Events;

        [JsonConstructor]
        public CloudSettings(STape VHS, CEventData[] Events)
        {
            this.VHS = VHS;

            this.Events = new();
            this.Events.AddRange(Events);
        }

    }
    #endregion
}