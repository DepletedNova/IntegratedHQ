using Kitchen;
using MessagePack;
using Unity.Entities;

namespace KitchenHQ.Franchise
{
    /*public class TelevisionView : UpdatableObjectView<TelevisionView.ViewData>
    {
        private ViewData Data;

        protected override void UpdateData(ViewData data)
        {
            // Guarantee that the cycle mode can be swapped
            if (Data.Cycle != data.Cycle)
                Data.Cycle = data.Cycle;

            // Do not update the tape if it is unnecessary to do so
            if (Data.Activated == data.Activated)
                return;



            Data = data;
        }

        private void Update()
        {

        }

        [MessagePackObject]
        public struct ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)] public bool Cycle;
            [Key(1)] public bool Activated;

            public IUpdatableObject GetRelevantSubview(IObjectView view) => view.GetSubView<TelevisionView>();

            public bool IsChangedFrom(ViewData check) => Cycle != check.Cycle;
        }

        private class UpdateView : IncrementalViewSystemBase<ViewData>
        {
            protected override void Initialise()
            {
                base.Initialise();
                RequireSingletonForUpdate<STapePlayer>();
                RequireSingletonForUpdate<STelevision>();
            }

            protected override void OnUpdate()
            {

            }
        }
    }*/
}
