using Kitchen;
using Unity.Entities;

namespace KitchenHQ.Franchise.Systems
{
    /*public class ManageEventIndicators : IndicatorManager
    {
        protected override ViewType ViewType => throw new System.NotImplementedException();

        protected override EntityQuery GetSearchQuery() => GetEntityQuery(new ComponentType[] { typeof(CEventData), typeof(SEventBoard), typeof(CPosition) });

        protected override bool ShouldHaveIndicator(Entity candidate) =>
            Has<SEventBoard>(candidate) && Has<CPosition>(candidate) && Has<CBeingLookedAt>(candidate) && RequireBuffer<CEventData>(candidate, out var buffer) && !buffer.IsEmpty;

        protected override Entity CreateIndicator(Entity source)
        {

            return base.CreateIndicator(source);
        }

        protected override void DestroyIndicator(Entity indicator, Entity source)
        {

            base.DestroyIndicator(indicator, source);
        }
    }*/
}
