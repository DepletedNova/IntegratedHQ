using KitchenData;
using KitchenMods;
using Newtonsoft.Json;
using Unity.Entities;

namespace KitchenHQ.Franchise
{
    [InternalBufferCapacity(4)]
    public struct CEvent : IBufferElementData, IModComponent
    {
        public string Title;
        public string LinkUrl;
        public string Desc;
        public string IconUrl;

        [JsonConstructor]
        public CEvent(string Title, string LinkUrl, string Desc, string IconUrl)
        {
            this.Title = Title;
            this.LinkUrl = LinkUrl;
            this.Desc = Desc;
            this.IconUrl = IconUrl;
        }

        public CEvent() { }
    }

    public struct CEventBoard : IAttachableProperty, IModComponent
    {
        public int Index;

        public struct Interacted : IAttachableProperty, IModComponent
        {
            public Entity Player;
        }
    }

}
