using Kitchen;
using MessagePack;
using System.Threading;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class ActivateSwitchView : UpdatableObjectView<ActivateSwitchView.ViewData>
    {
        public Animator Animator;

        protected override void UpdateData(ViewData data)
        {
            Animator.ResetTrigger("Play");

            if (data.Pinged)
                Animator.SetTrigger("Play");
        }

        [MessagePackObject]
        public struct ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)]
            public bool Pinged;

            public IUpdatableObject GetRelevantSubview(IObjectView view) => view.GetSubView<ActivateSwitchView>();

            public bool IsChangedFrom(ViewData check) => Pinged != check.Pinged;
        }

        private class UpdateView : IncrementalViewSystemBase<ViewData>
        {
            private EntityQuery Query;
            protected override void Initialise()
            {
                base.Initialise();

                Query = GetEntityQuery(new QueryHelper()
                    .All(typeof(CAppliance), typeof(CLinkedView), typeof(CTakesDuration))
                    .Any(typeof(CModRoomSwapper), typeof(CFranchiseApplianceSwapper), typeof(CChoiceRefresher)));
            }

            protected override void OnUpdate()
            {
                var views = Query.ToComponentDataArray<CLinkedView>(Allocator.Temp);
                var durations = Query.ToComponentDataArray<CTakesDuration>(Allocator.Temp);
                for (int i = 0; i < views.Length; i++)
                {
                    var duration = durations[i];
                    SendUpdate(views[i], new()
                    {
                        Pinged = duration.Remaining <= 0f && duration.Active
                    }, MessageType.SpecificViewUpdate);
                }
            }
        }
    }
}
