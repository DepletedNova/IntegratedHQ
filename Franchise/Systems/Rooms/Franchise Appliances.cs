using Kitchen;
using KitchenHQ.API;
using System.Collections.Generic;
using System;
using Unity.Entities;
using UnityEngine;
using KitchenHQ.Utility;

namespace KitchenHQ.Franchise
{
    [UpdateInGroup(typeof(ModFranchiseComponentGroup))]
    public class GenerateFranchiseAppliances : FranchiseBuildSystem
    {
        protected override bool OverrideBuild => true;

        protected override void Build()
        {
            LogDebug("[BUILD] Franchise Appliances");

            Dictionary<Vector3, List<(FranchiseAppliance Appliance, Action<Entity, EntityManager> Action)>> set =
                ReplaceHQ ? FranchiseAppliance.ModAppliances : FranchiseAppliance.BaseAppliances;

            var createSwapper = false;

            foreach (var pair in set)
            {
                if (GetOccupant(pair.Key) != Entity.Null)
                {
                    LogDebug($"[BUILD] [APPLIANCES] Skipping occupied spot: {pair.Key}");
                    continue;
                }

                if (!createSwapper && pair.Value.Count > 1)
                    createSwapper = true;

                var container = EntityManager.CreateEntity(typeof(CPosition), typeof(CFranchiseAppliance));
                EntityManager.SetComponentData(container, new CPosition(pair.Key));
                EntityManager.SetComponentData(container, new CFranchiseAppliance() { Total = pair.Value.Count });

                LogDebug($"[BUILD] [APPLIANCES] Created Franchise Appliance with {pair.Value.Count} selectable appliance(s)");
            }

            if (createSwapper)
            {
                Vector3 swapperPos = ReplaceHQ ? new(3.5f, 0, 2) : new(6, 0, 7);
                Create(FranchiseReferences.ApplianceSwapper, swapperPos, Vector3.forward);
            }

            Set<SFranchiseApplianceIndex>();
            Set<SRebuildFranchiseAppliances>();
        }
    }
}
