using Kitchen;
using MessagePack;

namespace KitchenHQ.Franchise
{
    public class TelevisionView : UpdatableObjectView<TelevisionView.ViewData>
    {
        protected override void UpdateData(ViewData data)
        {

        }

        [MessagePackObject]
        public struct ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)] public bool Cycle;
            [Key(1)] public STape TapeData;

            public IUpdatableObject GetRelevantSubview(IObjectView view) => view.GetSubView<TelevisionView>();

            public bool IsChangedFrom(ViewData check) => Cycle != check.Cycle;
        }
    }
}
