using Kitchen;
using Unity.Entities;
using static KitchenHQ.Franchise.STapeWriter;

namespace KitchenHQ.Franchise.Systems
{
    public class CleanTapeEditor : GenericSystemBase
    {
        protected override void OnUpdate()
        {
            if (Require(out SHasEditor sHasEditor) && 
                (sHasEditor.Editor == Entity.Null || sHasEditor.Player == Entity.Null 
                || !Has<SEditor>(sHasEditor.Editor) || !Has<CPlayer>(sHasEditor.Player)))
            {
                EntityManager.RemoveComponent<SHasEditor>(GetSingletonEntity<SHasEditor>());
            }

            if (Require(out SEditor sEditor) && (!Has<SHasEditor>() || sEditor.Appliance == Entity.Null || sEditor.Completed))
            {
                Clear<SEditor>();

                if (Has<SHasEditor>())
                    EntityManager.RemoveComponent<SHasEditor>(GetSingletonEntity<SHasEditor>());
            }

            if ((!Has<SHasEditor>() || !Has<SEditor>()) && TryGetSingletonEntity<STapeWriter>(out var writer) && Has<CPreventItemTransfer>(writer))
            {
                EntityManager.RemoveComponent<CPreventItemTransfer>(writer);
            }
        }
    }
}
