using KitchenHQ.Utility;
using Kitchen;
using KitchenData;
using KitchenMods;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    [UpdateInGroup(typeof(ModFranchiseGroup))]
    public class CreateModdedKitchen : FranchiseBuildSystem<CreateFranchiseKitchen>, IModSystem
    {
        protected override void Build()
        {
            var anchor = ModdedLobbyPositionAnchors.Kitchen;

            LogDebug("[BUILD] Kitchen");

            // Generic
            Create(AssetReference.Counter, anchor + new Vector3(3f, 0f, 0f), Vector3.forward);
            Create(AssetReference.Counter, anchor + new Vector3(4f, 0f, 0f), Vector3.forward);
            Create(AssetReference.Sink, anchor + new Vector3(5f, 0f, 0f), Vector3.forward);
            Create(AssetReference.InfiniteBin, anchor + new Vector3(6f, 0f, 0f), Vector3.forward);

            Create(AssetReference.Counter, anchor + new Vector3(0f, 0f, -3f), Vector3.left);
            Create(AssetReference.Counter, anchor + new Vector3(0f, 0f, -4f), Vector3.left);
            Create(AssetReference.Counter, anchor + new Vector3(0f, 0f, -5f), Vector3.left);

            // Table
            var tablePos = anchor + new Vector3(2f, 0f, -1f);
            var table = Create(AssetReference.TutorialTable, tablePos, Vector3.right);
            CreateChair(table, tablePos, Vector3.back);
            CreateChair(table, tablePos, Vector3.forward);

            // Kitchen Tutorial
            EntityManager.AddComponent<CModdedKitchenTutorialPrompt>(Create(AssetReference.FranchiseKitchenTutorial, anchor + new Vector3(2f, 0f, -3f), Vector3.forward));

            // Slots
            CreateSlot(anchor + new Vector3(3f, 0f, -2f), Vector3.left);

            CreateSlot(anchor + new Vector3(6f, 0f, -1f), Vector3.right);
            CreateSlot(anchor + new Vector3(6f, 0f, -2f), Vector3.right);
            CreateSlot(anchor + new Vector3(6f, 0f, -3f), Vector3.right);
            CreateSlot(anchor + new Vector3(6f, 0f, -4f), Vector3.right);

            CreateSlot(anchor + new Vector3(6f, 0f, -5f), Vector3.back);
            CreateSlot(anchor + new Vector3(5f, 0f, -5f), Vector3.back);
            CreateSlot(anchor + new Vector3(4f, 0f, -5f), Vector3.back);
            CreateSlot(anchor + new Vector3(3f, 0f, -5f), Vector3.back);
            CreateSlot(anchor + new Vector3(2f, 0f, -5f), Vector3.back);
            CreateSlot(anchor + new Vector3(1f, 0f, -5f), Vector3.back);

            // Update table
            if (!HasSingleton<SPerformTableUpdate>())
                EntityManager.CreateEntity(new ComponentType[] { typeof(SPerformTableUpdate) });

            // Rebuild kitchen
            EntityManager.AddComponentData(EntityManager.CreateEntity(), new RebuildKitchen.CRebuildKitchen { Dish = AssetReference.DishSteak });
        }

        private void CreateChair(Entity table, Vector3 tablePos, Vector3 offset)
        {
            EntityManager.AddComponentData(Create(AssetReference.Chair, tablePos + offset, offset), new CInteractionProxy
            {
                Target = table,
                IsActive = true
            });
        }

        protected void CreateSlot(Vector3 pos, Vector3 dir)
        {
            Entity slot = EntityManager.CreateEntity(new ComponentType[]
            {
                typeof(RebuildKitchen.CFranchiseKitchenSlot)
            });
            EntityManager.AddComponentData(slot, new CPosition(pos, Quaternion.LookRotation(dir, Vector3.up)));
        }
    }
}
