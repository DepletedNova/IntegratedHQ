using Kitchen;
using KitchenHQ.Utility;
using KitchenLib.Utils;
using MessagePack;
using Steamworks;
using Steamworks.Ugc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class TelevisionView : UpdatableObjectView<TelevisionView.ViewData>
    {
        private const float MaxCycleDuration = 5f;
        private static readonly int Image = Shader.PropertyToID("_Image");

        public MeshRenderer Renderer;
        public GameObject Screen;
        public TextMeshPro Label;

        private ViewData Data = default;

        private bool HasData = false;
        private List<Item> Items = new();
        private Dictionary<ulong, Texture2D> Textures = new();
        private int Index = 0;

        protected override void UpdateData(ViewData data)
        {
            LogDebug($"[APPLIANCE] [Television] Updating TV (Player: {data.PlayerID}; Interacted: {data.Interacted})");
            bool IsActive = data.Active;
            if (!IsActive)
            {
                CycleDuration = MaxCycleDuration;
                Screen.SetActive(false);
                return;
            }

            if (IsActive && (!data.Tape.Equals(Data.Tape) || Items.IsNullOrEmpty()))
            {
                LogDebug("[APPLIANCE] [Television] Retrieving items");
                Index = 0;
                RetrieveFiles(data);
            }

            Data = data;

            UpdateVisuals();

            if (!Screen.activeSelf || !Data.Interacted || Data.PlayerID == 0)
                return;

            LogDebug("[APPLIANCE] [Television] Calling overlay");
            if (Players.Main.Get(Data.PlayerID).IsLocalUser)
                SteamFriends.OpenWebOverlay(Items[Index].Url, false);
        }

        private float CycleDuration = MaxCycleDuration;
        private void Update()
        {
            if (!Data.Active || Items.Count <= 1 || !Screen.activeSelf)
                return;

            CycleDuration -= Time.deltaTime;
            if (CycleDuration > 0)
                return;

            CycleDuration = MaxCycleDuration;
            Index = (Index + 1) % Items.Count;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (!Data.Active || Items.IsNullOrEmpty() || Textures.IsNullOrEmpty() ||
                Index >= Items.Count || !Textures.TryGetValue(Items[Index].Id, out var tex))
            {
                CycleDuration = MaxCycleDuration;
                Screen.SetActive(false);
                Index = 0;
                return;
            }

            Screen.SetActive(true);
            Renderer.material.SetTexture(Image, tex);
            Label.text = Items[Index].Title;
        }

        private void RetrieveFiles(ViewData data)
        {
            Items.Clear();
            Textures.Clear();

            if (!PrefManager.Get<bool>("AllowAPI"))
                return;

            HasData = WebUtility.AwaitTask(Task.Run(() => WebUtility.GetItemsFromQuery(CreateQuery(data.Tape), 1)), out var potentialItems, 50);
            if (!HasData)
            {
                Items = new();
                return;
            }

            LogDebug("[APPLIANCE] [Television] Collected items");
            
            foreach (var item in potentialItems.GetRange(0, Math.Min(potentialItems.Count, 5)))
            {
                Items.Add(item);

                var tex = WebUtility.GetItemIcon(item);
                if (tex != null && tex != default)
                {
                    Textures.Add(item.Id, tex);
                }
            }
            Index = UnityEngine.Random.Range(0, Items.Count);
        }

        private Query CreateQuery(TapeValues tape)
        {
            var query = Query.Items;
            var types = tape.Type;

            if (types.HasFlag(TapeTypes.Tagged)) // Tags
            {
                query.MatchAnyTag();
                var tags = tape.Tags;
                for (int i = 0; i < tags.Count; i++)
                {
                    query.WithTag(tags[i]);
                }
                LogDebug($"[APPLIANCE] [Television] Sorted by tags ({tags.Count}; {tags})");
            }

            if (types.HasFlag(TapeTypes.Search)) // Search
            {
                var search = tape.Search;
                query.WhereSearchText(search);
                LogDebug($"[APPLIANCE] [Television] Search applied (\"{search}\")");
            }

            if (types.HasFlag(TapeTypes.Newest)) // Newest
            {
                LogDebug("[APPLIANCE] [Television] Sorted by Newest");
                query.RankedByPublicationDate();
            }
            else if (types.HasFlag(TapeTypes.Trending)) // Trending
            {
                LogDebug("[APPLIANCE] [Television] Sorted by Trending");
                query.RankedByTrend();
            }

            return query;
        }

        [MessagePackObject(false)]
        public struct ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(1)] public TapeValues Tape;
            [Key(2)] public bool Active;
            [Key(3)] public bool Interacted;
            [Key(4)] public int PlayerID;

            public IUpdatableObject GetRelevantSubview(IObjectView view) => view.GetSubView<TelevisionView>();

            public bool IsChangedFrom(ViewData check) => !Tape.Equals(check.Tape) || Active != check.Active || PlayerID != check.PlayerID || Interacted;
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
                var television = GetSingletonEntity<STelevision>();
                if (!Require(television, out CLinkedView cView))
                    return;

                var sTapePlayer = GetSingleton<STapePlayer>();
                var data = new ViewData()
                {
                    Tape = (TapeValues)sTapePlayer.Tape,
                    Active = sTapePlayer.Holding
                };

                if (Require(television, out STelevision.STriggerInteraction interaction))
                {
                    EntityManager.RemoveComponent<STelevision.STriggerInteraction>(television);
                    if (interaction.PlayerID != 0)
                    {
                        data.Interacted = true;
                        data.PlayerID = interaction.PlayerID;
                    }
                }

                SendUpdate(cView, data, MessageType.SpecificViewUpdate);
            }
        }
    }
}
