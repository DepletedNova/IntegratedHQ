using Kitchen;
using KitchenHQ.Utility;
using MessagePack;
using Steamworks.Ugc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KitchenHQ.Franchise
{
    public class CreatedModsViewView : UpdatableObjectView<CreatedModsViewView.ViewData>
    {
        private static readonly int Image = Shader.PropertyToID("_Image");

        public MeshRenderer Renderer;
        public TextMeshPro Label;
        public float MaxTimer;

        private ViewData Data;
        private List<Item> Items = new();
        private int ItemIndex;
        private float TimerDelta;

        protected override void UpdateData(ViewData data)
        {
            if (!data.IsChangedFrom(Data))
                return;
            Data = data;

            Items = Task.Run(() => WebUtility.GetItemsFromQuery(Query.Items.WhereUserPublished(data.UserID))).GetAwaiter().GetResult();
            ItemIndex = Random.Range(0, Items.Count);

            // Setup label
            if (Label != null)
                Label.text = string.Format("{0}\n<size=1.5>{1}</size>", Items.Count, Localisation["IHQ:CreatedMods"].Split('/')[Items.Count > 1 ? 0 : 1]);

            // Initial renderer setup
            UpdateRenderer();
        }

        void Update()
        {
            // Cycle
            if (Items.Count > 1)
            {
                TimerDelta -= Time.deltaTime;
                if (TimerDelta <= 0)
                {
                    TimerDelta = MaxTimer; // Reset timer

                    ItemIndex = (ItemIndex + 1) % Items.Count; // Increment index

                    UpdateRenderer(); // Update renderer
                }
            }
        }

        private void UpdateRenderer()
        {
            if (Renderer == null)
                return;

            var item = Items[ItemIndex];
            Texture tex = WebUtility.GetItemIcon(item);
            Renderer.material.SetTexture(Image, tex);
            Renderer.gameObject.SetActive(tex != null);
        }

        [MessagePackObject]
        public struct ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(1)] public ulong UserID;

            public IUpdatableObject GetRelevantSubview(IObjectView view) => view.GetSubView<CreatedModsViewView>();

            public bool IsChangedFrom(ViewData check) => UserID != check.UserID;
        }

        private class UpdateView : IncrementalViewSystemBase<ViewData>
        {
            private EntityQuery Query;
            protected override void Initialise()
            {
                base.Initialise();
                Query = GetEntityQuery(new ComponentType[] { typeof(CAppliance), typeof(CLinkedView), typeof(CCreatedModsView) });
            }

            protected override void OnUpdate()
            {
                using var Views = Query.ToComponentDataArray<CLinkedView>(Allocator.Temp);
                using var Comps = Query.ToComponentDataArray<CCreatedModsView>(Allocator.Temp);
                for (int i = 0; i < Views.Length; i++)
                {
                    var comp = Comps[i];
                    SendUpdate(Views[i], new()
                    {
                        UserID = comp.UserID
                    });
                }
            }
        }
    }
}
