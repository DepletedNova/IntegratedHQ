using Kitchen;
using Unity.Entities;
using static KitchenHQ.Franchise.STapeWriter;

namespace KitchenHQ.Franchise.Systems
{
    public class ActivateTapeEditor : ItemInteractionSystem
    {
        protected override bool RequirePress => true;

        protected override bool IsPossible(ref InteractionData data) =>
            !Has<SEditor>() && !Has<SHasEditor>() && !Has<STriggerEditor>() &&
            Has<STapeWriter>(data.Target) && Require(data.Target, out CItemHolder holder) && holder.HeldItem != Entity.Null;

        protected override void Perform(ref InteractionData data)
        {
            Set(data.Target, new STriggerEditor { Interactor = data.Interactor });
            Set<CPreventItemTransfer>(data.Target);
        }
    }
}
