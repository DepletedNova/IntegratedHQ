using Kitchen;
using KitchenHQ.Utility;
using MessagePack;
using Steamworks.Ugc;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class SubscribedModsViewView : UpdatableObjectView<SubscribedModsViewView.ViewData>
    {
        public TextMeshPro Label;

        public GameObject GenericPodium;
        public GameObject PartialPodium;

        public float PartialPercent = 0.6f;

        private ViewData Data;
        private int TotalMods = 0;

        protected override void UpdateData(ViewData data)
        {
            if (!data.IsChangedFrom(Data))
                return;
            Data = data;

            if (Label != null)
            {
                var hasTotalMods = WebUtility.AwaitTask(Task.Run(() => WebUtility.GetItemCountFromQuery(Query.Items)), out TotalMods);
                Label.text = hasTotalMods && TotalMods != 0 ?
                    string.Format("{0}/{1}\n<size=1.5>{2}</size>", Data.Count, TotalMods, Localisation["IHQ:SubbedMods"]) :
                    string.Format("{0}\n<size=1.5>{1}</size>", Data.Count, Localisation["IHQ:SubbedMods"]);
            }

            if (GenericPodium != null && PartialPodium != null)
            {
                if (TotalMods != 0 && Data.Count >= TotalMods * PartialPercent)
                {
                    GenericPodium.SetActive(false);
                    PartialPodium.SetActive(true);
                }
                else
                {
                    GenericPodium.SetActive(true);
                    PartialPodium.SetActive(false);
                }
            }
        }

        [MessagePackObject(false)]
        public struct ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(1)] public int Count;

            public IUpdatableObject GetRelevantSubview(IObjectView view) => view.GetSubView<SubscribedModsViewView>();

            public bool IsChangedFrom(ViewData check) => Count != check.Count;
        }

        private class UpdateView : IncrementalViewSystemBase<ViewData>
        {
            private EntityQuery Query;
            protected override void Initialise()
            {
                base.Initialise();
                Query = GetEntityQuery(new ComponentType[] { typeof(CAppliance), typeof(CLinkedView), typeof(CSubscribedModsView) });
            }

            protected override void OnUpdate()
            {
                using var Views = Query.ToComponentDataArray<CLinkedView>(Allocator.Temp);
                using var Comps = Query.ToComponentDataArray<CSubscribedModsView>(Allocator.Temp);
                for (int i = 0; i < Views.Length; i++)
                {
                    var comp = Comps[i];
                    SendUpdate(Views[i], new()
                    {
                        Count = comp.ModCount
                    }, MessageType.SpecificViewUpdate);
                }
            }
        }
    }
}
