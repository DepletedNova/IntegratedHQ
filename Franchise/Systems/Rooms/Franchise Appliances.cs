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
    public class GenerateFranchiseAppliances : FranchiseFirstFrameSystem
    {
        protected override void OnUpdate()
        {
            LogDebug("[BUILD] Franchise Appliances");

            Dictionary<Vector3, List<(FranchiseAppliance Appliance, Action<Entity, EntityManager> Action)>> set =
                PrefManager.Get<bool>("LegacyHQEnabled") ? FranchiseAppliance.BaseAppliances : FranchiseAppliance.ModAppliances;
            foreach (var pair in set)
            {
                if (GetOccupant(pair.Key) != Entity.Null)
                {
                    LogDebug($"[BUILD] [APPLIANCES] Skipping occupied spot: {pair.Key}");
                    continue;
                }

                var container = EntityManager.CreateEntity(typeof(CPosition), typeof(CFranchiseAppliance));
                EntityManager.SetComponentData(container, new CPosition(pair.Key));
                EntityManager.SetComponentData(container, new CFranchiseAppliance() { Total = pair.Value.Count });

                LogDebug($"[BUILD] [APPLIANCES] Created Franchise Appliance with {pair.Value.Count} selectable appliance(s)");
            }

            EntityManager.CreateEntity(typeof(SFranchiseApplianceIndex));
            EntityManager.CreateEntity(typeof(SRebuildFranchiseAppliances));
        }
    }
}
