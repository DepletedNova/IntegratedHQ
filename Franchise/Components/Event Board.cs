using KitchenData;
using KitchenMods;
using Newtonsoft.Json;
using Unity.Collections;
using Unity.Entities;

namespace KitchenHQ.Franchise
{
    public struct SEventBoard : IAttachableProperty, IModComponent
    {
        public int Index;

        public struct SInteracted : IAttachableProperty, IModComponent
        {
            public Entity Player;
        }
    }

    [InternalBufferCapacity(4)]
    public struct CEvent : IBufferElementData
    {
        public FixedString128 Title;
        public FixedString512 LinkUrl;
        public FixedString512 Desc;
        public FixedString512 IconUrl;

        [JsonConstructor]
        public CEvent(string Title, string LinkUrl, string Desc, string IconUrl)
        {
            this.Title = Title;
            this.LinkUrl = LinkUrl;
            this.Desc = Desc;
            this.IconUrl = IconUrl;
        }
    }

}
