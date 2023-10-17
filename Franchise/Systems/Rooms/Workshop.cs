using KitchenHQ.Utility;
using Kitchen;
using KitchenData;
using KitchenMods;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    [UpdateInGroup(typeof(ModFranchiseGroup))]
    public class CreateModdedWorkshop : FranchiseBuildSystem<CreateWorkshop>, IModSystem
    {
        private EntityQuery Crates;
        protected override void OnInitialise()
        {
            Crates = GetEntityQuery(new ComponentType[] { typeof(CCrateAppliance) });
        }

        protected override void Build()
        {
            var workshop = LobbyPositionAnchors.Workshop;

            // Decorations

            // it kept flashing :(
            //Create(FranchiseReferences.WorkshopMarker, workshop + new Vector3(-0.55f, 0.1f, 7.6f), Vector3.forward);

            LogDebug("[BUILD] Workshop room");

            Create(AssetReference.WorkshopWall, workshop + new Vector3(0f, 0f, 8.5f), Vector3.forward);
            Create(AssetReference.WorkshopWall, workshop + new Vector3(1f, 0f, 8.5f), Vector3.forward);
            Create(AssetReference.WorkshopWall, workshop + new Vector3(2f, 0f, 8.5f), Vector3.forward);
            Create(AssetReference.WorkshopGate, workshop + new Vector3(3f, 0f, 8.5f), Vector3.forward);
            Create(AssetReference.WorkshopWall, workshop + new Vector3(4f, 0f, 8.5f), Vector3.forward);
            Create(AssetReference.WorkshopWall, workshop + new Vector3(5f, 0f, 8.5f), Vector3.forward);

            // Workshop Machine
            EntityManager.AddComponentData(Create(AssetReference.WorkshopInputSlot, workshop + new Vector3(1f, 0f, 10f), Vector3.forward), new CWorkshopInput { Index = 0 });
            EntityManager.AddComponentData(Create(AssetReference.WorkshopInputSlot, workshop + new Vector3(2f, 0f, 10f), Vector3.forward), new CWorkshopInput { Index = 1 });
            EntityManager.AddComponentData(Create(AssetReference.WorkshopInputSlot, workshop + new Vector3(3f, 0f, 10f), Vector3.forward), new CWorkshopInput { Index = 2 });

            EntityManager.AddComponent<SWorkshopOutput>(Create(AssetReference.WorkshopOutputSlot, workshop + new Vector3(4f, 0f, 10f), Vector3.forward));
            EntityManager.AddComponent<CWorkshopMachine>(Create(AssetReference.WorkshopMachine, workshop + new Vector3(2.5f, 0f, 11f), Vector3.right));
            EntityManager.AddComponent<CWorkshopActivateButton>(Create(AssetReference.WorkshopCraftButton, workshop + new Vector3(1f, 0f, 8f), Vector3.right));

            if (Crates.IsEmpty)
                return;

            LogDebug("[BUILD] Crate podiums");

            // Crates
            for (int i = 0; i < 6; i++)
            {
                if (i != 3)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        Vector3 facing = (j % 2 != 0) ? Vector3.forward : Vector3.back;
                        EntityManager.AddComponentData(Create(AssetReference.GarageShelf, workshop + new Vector3(i, 0f, j), facing), new CPersistentItemStorageLocation
                        {
                            Type = PersistentStorageType.Crate
                        });
                        if (j % 2 == 1)
                        {
                            Create(AssetReference.GarageDivider, workshop + new Vector3(i, 0f, j), Vector3.forward);
                            if (i == 0)
                                Create(FranchiseReferences.GarageMarker, workshop + new Vector3(-0.5f, 0f, j + 0.5f), Vector3.forward);
                        }
                    }
                }
            }
        }
    }
}
