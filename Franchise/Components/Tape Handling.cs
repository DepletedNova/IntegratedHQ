using KitchenData;
using KitchenMods;
using Newtonsoft.Json;
using static KitchenHQ.Layout.ModRoomReferences;

namespace KitchenHQ.Franchise
{
    public struct STape : IAttachableProperty, IModComponent
    {
        public TapeType Type;
        public string Tag;
        public string User;
        public string Search;

        [JsonConstructor]
        public STape(int Type, string Tag, string User, string Search)
        {
            this.Type = (TapeType)Type;
            this.Tag = Tag;
            this.User = User;
            this.Search = Search;
        }
    }

    public struct STapePlayer : IAttachableProperty, IModComponent { }

    public struct STelevision : IAttachableProperty, IModComponent { }

    public struct STapeWriter : IAttachableProperty, IModComponent { }
}
