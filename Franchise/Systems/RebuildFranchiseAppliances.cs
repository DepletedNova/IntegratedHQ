using Kitchen;
using KitchenHQ.API;
using KitchenHQ.Utility;
using System.Collections.Generic;
using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQ.Franchise.Systems
{
    [UpdateAfter(typeof(ModFranchiseComponentGroup))]
    public class RebuildFranchiseAppliances : FranchiseSystem
    {
        private EntityQuery FranchiseAppliances;

        protected override void Initialise()
        {
            base.Initialise();

            FranchiseAppliances = GetEntityQuery(new ComponentType[] { typeof(CPosition), typeof(CFranchiseAppliance) });

            RequireSingletonForUpdate<SRebuildFranchiseAppliances>();
            RequireSingletonForUpdate<SFranchiseApplianceIndex>();
            RequireForUpdate(FranchiseAppliances);
        }

        protected override void OnUpdate()
        {
            Clear<SRebuildFranchiseAppliances>();

            var GlobalIndex = GetSingleton<SFranchiseApplianceIndex>().Index;
            Dictionary<Vector3, List<(FranchiseAppliance Appliance, Action<Entity, EntityCommandBuffer> Action)>> set =
                PrefManager.Get<bool>("LegacyHQEnabled") ? FranchiseAppliance.BaseAppliances : FranchiseAppliance.ModAppliances;

            LogDebug("[REBUILD] [APPLIANCES] Rebuilding");

            var ECB = new EntityCommandBuffer(Allocator.Temp);

            var entities = FranchiseAppliances.ToEntityArray(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var FA = EntityManager.GetComponentData<CFranchiseAppliance>(entity);

                if (FA.Total <= GlobalIndex)
                    continue;

                if (FA.PairedAppliance != Entity.Null && FA.Total == 1)
                    continue;

                EntityManager.DestroyEntity(FA.PairedAppliance);
                var pos = EntityManager.GetComponentData<CPosition>(entity).Position;

                LogDebug($"[REBUILD] [APPLIANCES] Rebuilding appliance: {pos}");

                if (!set.ContainsKey(pos))
                {
                    LogError($"[REBUILD] [APPLIANCES] Could not find Appliance key: {pos}");
                    continue;
                }

                var pair = set[pos][GlobalIndex];
                
                var applianceEntity = Create(pair.Appliance.ID, pair.Appliance.Position, pair.Appliance.Rotation);
                if (pair.Action != null)
                    pair.Action.Invoke(applianceEntity, ECB);

                FA.PairedAppliance = applianceEntity;
                Set(entity, FA);
            }

            ECB.Playback(EntityManager);
            ECB.Dispose();

            LogDebug($"[REBUILD] [APPLIANCES] Rebuilt appliances under index: {GlobalIndex}");
        }
    }
}
