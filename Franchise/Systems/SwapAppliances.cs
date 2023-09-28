using Kitchen;
using Kitchen.Layouts.Modules;
using KitchenHQ.API;
using System.Reflection;
using Unity.Collections;
using Unity.Entities;

namespace KitchenHQ.Franchise
{
    public class SwapAppliances : FranchiseSystem
    {
        private EntityQuery Swappers;
        protected override void Initialise()
        {
            base.Initialise();
            Swappers = GetEntityQuery(new ComponentType[] { typeof(CFranchiseApplianceSwapper), typeof(CTakesDuration) });
            RequireForUpdate(Swappers);
            RequireSingletonForUpdate<SFranchiseApplianceIndex>();
        }

        protected override void OnUpdate()
        {
            var durations = Swappers.ToComponentDataArray<CTakesDuration>(Allocator.Temp);
            for (int i = 0; i < durations.Length; i++)
            {
                var duration = durations[i];
                if (duration.Remaining <= 0 && duration.Active)
                {
                    var franchiseAppliances = GetSingleton<SFranchiseApplianceIndex>();
                    franchiseAppliances.Index = (franchiseAppliances.Index + 1) % 
                        (PrefManager.Get<bool>("LegacyHQEnabled") ? FranchiseAppliance.BaseMax : FranchiseAppliance.ModMax);

                    LogDebug("[CYCLE] Cycling Franchise Appliances to index: " + franchiseAppliances.Index.ToString());

                    Set(franchiseAppliances);
                    Set<SRebuildFranchiseAppliances>();
                }
            }
        }
    }
}
