using Kitchen;
using Unity.Entities;
using static KitchenHQ.Franchise.STapeWriter;

namespace KitchenHQ.Franchise.Systems
{
    public class CleanTapeEditor : GenericSystemBase
    {
        protected override void OnUpdate()
        {
            if (Require(out SHasTapeEditor sHasEditor) && 
                (sHasEditor.Editor == Entity.Null || sHasEditor.Player == Entity.Null 
                || !Has<STapeEditor>(sHasEditor.Editor) || !Has<CPlayer>(sHasEditor.Player)))
            {
                EntityManager.RemoveComponent<SHasTapeEditor>(GetSingletonEntity<SHasTapeEditor>());
            }

            if (Require(out STapeEditor sEditor) && (!Has<SHasTapeEditor>() || sEditor.Appliance == Entity.Null || sEditor.Completed))
            {
                Clear<STapeEditor>();

                if (Has<SHasTapeEditor>())
                    EntityManager.RemoveComponent<SHasTapeEditor>(GetSingletonEntity<SHasTapeEditor>());
            }

            if ((!Has<SHasTapeEditor>() || !Has<STapeEditor>()) && TryGetSingletonEntity<STapeWriter>(out var writer) && Has<CPreventItemTransfer>(writer))
            {
                EntityManager.RemoveComponent<CPreventItemTransfer>(writer);
            }
        }
    }
}
