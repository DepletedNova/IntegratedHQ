using KitchenHQ.Utility;
using Kitchen;
using KitchenData;
using KitchenMods;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace KitchenHQ.Franchise
{
    public class CreateDebugMarkers : FranchiseBuildSystem, IModSystem
    {
        protected override bool OverrideBuild => true;

        protected override void Build()
        {
            if (!PrefManager.Get<bool>("ShowRoomAnchors"))
            {
                LogDebug("[BUILD] [SKIP] Debug markers (disabled)");
                return;
            }

            LogDebug("[BUILD] Debug markers");

            CreateMarker(Vector3.up * 0.5f);

            CreateMarker(LobbyPositionAnchors.Workshop);
            CreateMarker(LobbyPositionAnchors.Garage);
            CreateMarker(LobbyPositionAnchors.StartMarker);

            CreateMarker(LobbyPositionAnchors.Kitchen);
            CreateMarker(LobbyPositionAnchors.Office);
            CreateMarker(LobbyPositionAnchors.Stats);
            CreateMarker(LobbyPositionAnchors.Contracts);

            foreach (var bed in LobbyPositionAnchors.Bedrooms)
                CreateMarker(bed);

            if (!ReplaceHQ)
                return;

            CreateMarker(ModdedLobbyPositionAnchors.ModRoomAnchor, true);

            CreateMarker(ModdedLobbyPositionAnchors.Workshop, true);
            CreateMarker(ModdedLobbyPositionAnchors.Garage, true);
            CreateMarker(ModdedLobbyPositionAnchors.StartMarker, true);

            CreateMarker(ModdedLobbyPositionAnchors.Kitchen, true);
            CreateMarker(ModdedLobbyPositionAnchors.Office, true);
            CreateMarker(ModdedLobbyPositionAnchors.Stats, true);
            CreateMarker(ModdedLobbyPositionAnchors.Contracts, true);

            foreach (var bed in ModdedLobbyPositionAnchors.Bedrooms)
                CreateMarker(bed, true);
        }

        private void CreateMarker(Vector3 position, bool NHQ = false)
        {
            Entity entity = EntityManager.CreateEntity(typeof(CCreateAppliance), typeof(CPosition));
            EntityManager.SetComponentData(entity, new CCreateAppliance { ID = NHQ ? FranchiseReferences.ModDebugMarker : FranchiseReferences.DebugMarker });
            EntityManager.SetComponentData(entity, new CPosition(position, quaternion.LookRotation(Vector3.forward, new float3(0f, 1f, 0f))));
            SetOccupant(position, entity, OccupancyLayer.Ceiling);
        }
    }
}
