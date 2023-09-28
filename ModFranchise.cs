global using KitchenHQ.Layout;
using Kitchen;
using Kitchen.Layouts;
using Kitchen.Layouts.Modules;
using KitchenData;
using KitchenLib.Utils;
using UnityEngine;
using XNode;

namespace KitchenHQ.Layout
{
    // Headquarter modifications
    public static class ModFranchise
    {
        public static readonly Vector3 ModRoomAnchor = new(-1f, 0f, 6f);

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
            LobbyPositionAnchors.Workshop = new Vector3(-13f, 0, -3f);
            LobbyPositionAnchors.Garage = new Vector3(-13f, 0f, -9f);
            LobbyPositionAnchors.StartMarker = new Vector3(-10.5f, 0f, -8.5f);

            LobbyPositionAnchors.Kitchen = new Vector3(6f, 0f, 2f);
            LobbyPositionAnchors.Office = new Vector3(-1f, 0f, -3f);
            LobbyPositionAnchors.Stats = new Vector3(9f, 0f, -4f);
            LobbyPositionAnchors.Contracts = new Vector3(-7f, 0f, 3f);

            LobbyPositionAnchors.Bedrooms[0] = new Vector3(10.5f, 0f, 7f);
            LobbyPositionAnchors.Bedrooms[1] = new Vector3(3.5f, 0f, 7f);
            LobbyPositionAnchors.Bedrooms[2] = new Vector3(10.5f, 0f, 4f);
            LobbyPositionAnchors.Bedrooms[3] = new Vector3(3.5f, 0f, 4f);
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
    }
    #endregion
}