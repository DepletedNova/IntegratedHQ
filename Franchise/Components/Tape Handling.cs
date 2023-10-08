using KitchenData;
using KitchenMods;
using Newtonsoft.Json;
using Unity.Collections;
using Unity.Entities;

namespace KitchenHQ.Franchise
{
    public struct STape : IAttachableProperty, IModComponent
    {
        public TapeTypes Type;
        public FixedString512 Tag;
        public FixedString128 User;
        public FixedString512 Search;

        [JsonConstructor]
        public STape(int Type, string Tag, string User, string Search)
        {
            this.Type = (TapeTypes)Type;
            this.Tag = Tag;
            this.User = User;
            this.Search = Search;
        }
    }

    public struct STelevision : IAttachableProperty, IModComponent { }
    public struct STapePlayer : IAttachableProperty, IModComponent { }

    public struct STapeWriter : IAttachableProperty, IModComponent
    {
        public struct STriggerEditor : IAttachableProperty, IModComponent
        {
            public Entity Interactor;
        }

        public struct SHasEditor : IAttachableProperty, IModComponent
        {
            public Entity Editor;
            public Entity Player;
        }

        public struct SEditor : IModComponent
        {
            public Entity Appliance;
            public int PlayerID;

            public bool Completed;
        }
    }
}
