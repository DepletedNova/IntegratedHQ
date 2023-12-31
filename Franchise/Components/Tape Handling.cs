﻿using KitchenData;
using KitchenLib.Utils;
using KitchenMods;
using MessagePack;
using Newtonsoft.Json;
using Unity.Collections;
using Unity.Entities;

namespace KitchenHQ.Franchise
{
    [MessagePackObject(false)]
    public struct STape : IAttachableProperty, IModComponent
    {
        [Key(1)] public int Type;
        [Key(2)] public FixedString512 Tags;
        [Key(3)] public FixedString128 User;
        [Key(4)] public FixedString512 Search;

        [JsonConstructor]
        public STape(int Type, string Tags, string User, string Search)
        {
            this.Type = Type;
            this.Tags = Tags.IsNullOrEmpty() ? new() : (FixedString512)Tags;
            this.User = User.IsNullOrEmpty() ? new() : (FixedString128)User;
            this.Search = Search.IsNullOrEmpty() ? new() : (FixedString512)Search;
        }
    }

    public struct STelevision : IAttachableProperty, IModComponent
    {
        public struct STriggerInteraction : IAttachableProperty, IModComponent
        {
            public int PlayerID;
        }
    }

    public struct STapePlayer : IAttachableProperty, IModComponent
    {
        public STape Tape;
        public bool Holding;
    }

    public struct STapeWriter : IAttachableProperty, IModComponent
    {
        public struct STriggerTapeEditor : IAttachableProperty, IModComponent
        {
            public Entity Interactor;
        }

        public struct SHasTapeEditor : IAttachableProperty, IModComponent
        {
            public Entity Editor;
            public Entity Player;
        }

        public struct STapeEditor : IModComponent
        {
            public Entity Appliance;
            public int PlayerID;

            public bool Completed;
        }
    }
}
