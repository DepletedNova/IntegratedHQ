using KitchenData;
using KitchenHQ.Utility;
using KitchenMods;
using MessagePack;
using Newtonsoft.Json;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public struct SEventBoard : IAttachableProperty, IModComponent
    {
        public int Index;
        public float Timer;

        public struct SInteractedEvents : IAttachableProperty, IModComponent
        {
            public Entity Player;
        }

        public struct SEventPopup : IModComponent
        {
            public CEventData Data;
        }
    }

    [InternalBufferCapacity(4)]
    [MessagePackObject]
    public struct CEventData : IBufferElementData
    {
        [Key(0)] public FixedString128 Title;
        [Key(1)] public FixedString512 Url;
        [Key(2)] public FixedString512 Desc;
        [Key(3)] public FixedString512 IconUrl;

        [JsonConstructor]
        public CEventData(string Title, string Desc, string Url, string IconUrl)
        {
            this.Title = Title;
            this.Url = Url;
            this.Desc = Desc;
            this.IconUrl = IconUrl;
        }
    }

}
