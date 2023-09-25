using KitchenHQ.Utility;
using Kitchen;
using KitchenData;
using KitchenMods;
using UnityEngine;

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

            Create(FranchiseReferences.Marker, Vector3.up * 0.5f, Vector3.forward);

            Create(FranchiseReferences.Marker, LobbyPositionAnchors.Workshop, Vector3.forward);
            Create(FranchiseReferences.Marker, LobbyPositionAnchors.Garage, Vector3.forward);
            Create(FranchiseReferences.Marker, LobbyPositionAnchors.StartMarker, Vector3.forward);

            Create(FranchiseReferences.Marker, LobbyPositionAnchors.Kitchen, Vector3.forward);
            Create(FranchiseReferences.Marker, LobbyPositionAnchors.Office, Vector3.forward);
            Create(FranchiseReferences.Marker, LobbyPositionAnchors.Stats, Vector3.forward);
            Create(FranchiseReferences.Marker, LobbyPositionAnchors.Contracts, Vector3.forward);

            foreach (var bed in LobbyPositionAnchors.Bedrooms)
                Create(FranchiseReferences.Marker, bed, Vector3.forward);

            if (ReplaceHQ)
                Create(FranchiseReferences.Marker, ModFranchise.ModRoomAnchor, Vector3.forward);
        }
    }
}
