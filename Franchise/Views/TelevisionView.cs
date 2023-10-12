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
                SteamFriends.OpenWebOverlay(Items[Index].Url, true);
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

            var potentialItems = WebUtility.AwaitTask(Task.Run(() => WebUtility.GetItemsFromQuery(CreateQuery(data.Tape), 1)), 50);
            if (potentialItems.IsNullOrEmpty() || potentialItems == default)
                return;

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

        private Query CreateQuery(STape tape)
        {
            var query = Query.Items;
            var types = (TapeTypes)tape.Type;

            // Tags
            if (types.HasFlag(TapeTypes.Tagged))
            {
                query.MatchAnyTag();
                var tags = tape.Tags.ConvertToString().Split(';');
                for (int i = 0; i < tags.Length; i++)
                {
                    query.WithTag(tags[i]);
                }
                LogDebug($"[APPLIANCE] [Tape Player] Sorted by tags ({tags.Length}; {tape.Tags.ConvertToString()})");
            }

            // Search
            if (types.HasFlag(TapeTypes.Search))
            {
                var search = tape.Search.ConvertToString();
                query.WhereSearchText(search);
                LogDebug($"[APPLIANCE] [Tape Player] Search applied (\"{search}\")");
            }

            if (types.HasFlag(TapeTypes.Newest)) // Newest
            {
                LogDebug("[APPLIANCE] [Tape Player] Sorted by Newest");
                query.RankedByPublicationDate();
            }
            else if (types.HasFlag(TapeTypes.Trending)) // Trending
            {
                LogDebug("[APPLIANCE] [Tape Player] Sorted by Trending");
                query.RankedByTrend();
            }

            return query;
        }

        [MessagePackObject]
        public struct ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)] public STape Tape;
            [Key(1)] public bool Active;
            [Key(2)] public bool Interacted;
            [Key(3)] public int PlayerID;

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
                    Tape = sTapePlayer.Tape,
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
