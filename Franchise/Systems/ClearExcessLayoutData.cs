using Kitchen;
using KitchenMods;
using System.Linq;
using System.Reflection;
using Unity.Collections;
using Unity.Entities;

namespace KitchenHQ.Franchise
{
    [UpdateBefore(typeof(HandleLayoutRequests))]
    public class ClearExcessLayoutData : FranchiseSystem, IModSystem
    {
        private EntityQuery MapItems;
        protected override void Initialise()
        {
            base.Initialise();
            MapItems = GetEntityQuery(new ComponentType[] { 
                typeof(CItemLayoutMap), 
                typeof(HandleLayoutRequests).GetNestedType("CClearOnLayoutRequest", BindingFlags.NonPublic) 
            });
        }

        protected override void OnUpdate()
        {
            if (!Require(out HandleLayoutRequests.SLayoutRequest sRequest) || sRequest.HasBeenCreated)
                return;

            var layoutMaps = MapItems.ToComponentDataArray<CItemLayoutMap>(Allocator.Temp);
            var layoutEntities = from map in layoutMaps select map.Layout;

            LogDebug($"[MAPS] Removing excess layout data ({layoutEntities.Count()})");
            
            foreach (var entity in layoutEntities)
                EntityManager.DestroyEntity(entity);

            layoutMaps.Dispose();
        }
    }
}
