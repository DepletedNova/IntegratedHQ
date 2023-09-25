using KitchenHQ.Utility;
using Kitchen;
using KitchenData;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class CreateModdedExpGrants : FranchiseBuildSystem<CreateExpGrants>, IModSystem
    {
        private EntityQuery Grants;

        protected override void OnInitialise()
        {
            Grants = GetEntityQuery(new ComponentType[] { typeof(CExpGrant) });
        }

        protected override void Build()
        {
            int grantedExp = 0;
            var expGrants = Grants.ToComponentDataArray<CExpGrant>(Allocator.Temp);
            foreach (var grant in expGrants)
            {
                if (!grant.IsGranted)
                {
                    grantedExp += grant.Amount;
                }
            }
            expGrants.Dispose();

            if (grantedExp > 0)
            {
                LogDebug("[BUILD] XP grant");
                var grantAppliance = Create(AssetReference.ExpGranter, new Vector3(6f, 0f, 8f), Vector3.forward);
                EntityManager.AddComponentData(grantAppliance, new CExpGrantAppliance { Amount = grantedExp });
            }
        }
    }
}
