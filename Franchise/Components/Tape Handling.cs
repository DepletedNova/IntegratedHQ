using KitchenData;
using KitchenMods;
using MessagePack;
using Newtonsoft.Json;
using Unity.Collections;
using Unity.Entities;

namespace KitchenHQ.Franchise
{
    [MessagePackObject]
    public struct STape : IAttachableProperty, IModComponent
    {
        [Key(0)] public int Type;
        [Key(1)] public FixedString512 Tags;
        [Key(2)] public FixedString128 User;
        [Key(3)] public FixedString512 Search;

        [JsonConstructor]
        public STape(int Type, string Tags, string User, string Search)
        {
            this.Type = Type;
            this.Tags = Tags;
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
