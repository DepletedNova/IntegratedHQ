using KitchenHQ.Utility;
using Kitchen;
using KitchenData;
using KitchenMods;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    [UpdateInGroup(typeof(ModFranchiseGroup))]
    public class CreateModdedGarage : FranchiseBuildSystem<CreateGarage>, IModSystem
    {
        protected override void Build()
        {
            var garage = ModdedLobbyPositionAnchors.Garage;

            LogDebug("[BUILD] Garage");

            // Decorations
            Create(FranchiseReferences.ClosedGarage, garage + new Vector3(-0.5f, 0f, 2f), Vector3.forward);
            Create(FranchiseReferences.OpenGarage, garage + new Vector3(5.5f, 0f, 2f), Vector3.forward);
            Create(FranchiseReferences.OfficeShortWall, garage + new Vector3(5.5f, 0f, 0.15f), Vector3.left);
            Create(FranchiseReferences.LoadingBayText, garage + new Vector3(1f, 0f, 3.5f), Vector3.forward);

            // Pedestals
            EntityManager.AddComponent<CItemPedestal>(Create(AssetReference.LoadoutPedestal, garage + new Vector3(0f, 0f, 4f), Vector3.forward));
            EntityManager.AddComponent<CItemPedestal>(Create(AssetReference.LoadoutPedestal, garage + new Vector3(0f, 0f, 5f), Vector3.forward));
            EntityManager.AddComponent<CItemPedestal>(Create(AssetReference.LoadoutPedestal, garage + new Vector3(2f, 0f, 4f), Vector3.forward));
            EntityManager.AddComponent<CItemPedestal>(Create(AssetReference.LoadoutPedestal, garage + new Vector3(2f, 0f, 5f), Vector3.forward));
        }
    }
}
