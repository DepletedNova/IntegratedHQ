using KitchenHQ.Utility;
using Kitchen;
using KitchenData;
using KitchenMods;
using MessagePack.Resolvers;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    [UpdateInGroup(typeof(ModFranchiseGroup))]
    public class CreateModdedLayoutSlots : FranchiseBuildSystem<CreateLayoutSlots>, IModSystem
    {
        private EntityQuery ExtraLayouts;

        protected override void OnInitialise()
        {
            ExtraLayouts = GetEntityQuery(new ComponentType[] { typeof(CUpgradeExtraLayout) });
        }

        protected override void Build()
        {
            var office = LobbyPositionAnchors.Office;

            // Slot count
            int slots = ExtraLayouts.CalculateEntityCount();
            if (Require(out SPlayerLevel sLevel))
            {
                LogDebug("[BUILD] [UNLOCK] Adding extra layout slots...");
                if (sLevel.Level >= 10)
                {
                    slots += 2;
                    LogDebug("[BUILD] [UNLOCK] Level 10: +2");
                }

                if (sLevel.Level >= 15)
                {
                    slots += 3;
                    LogDebug("[BUILD] [UNLOCK] Level 15: +3");
                }
            }

            // Place slots
            List<Vector3> layoutPositions = new()
            {
                new Vector3(-2f, 0f, -3f),
                new Vector3(-2f, 0f, -4f),
                new Vector3(-3f, 0f, -3f),
                new Vector3(-3f, 0f, -4f),
                new Vector3(-4f, 0f, -3f),
                new Vector3(-4f, 0f, -4f),

                new Vector3(-2f, 0f, -6f),
                new Vector3(-3f, 0f, -6f),
                new Vector3(-4f, 0f, -6f),
            };

            int totalSlots = Mathf.Min(layoutPositions.Count, 1 + slots);
            LogDebug($"[BUILD] Layout slots ({totalSlots})");
            for (int i = 0; i < totalSlots; i++)
            {
                CreateMapSource(office + layoutPositions[i]);
            }
        }

        private void CreateMapSource(Vector3 location)
        {
            var pedestal = Create(AssetReference.LayoutPedestal, location, Vector3.forward);
            EntityManager.AddComponent<CItemHolder>(pedestal);
            EntityManager.AddComponent<CreateLayoutSlots.CLayoutSlot>(pedestal);
        }
    }
}
