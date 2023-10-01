using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using static KitchenHQ.Layout.ModRoomReferences;

namespace KitchenHQ.Utility
{
    public class CloudSettings
    {
        public Tape VHS;

        public Event[] Events;

        [JsonConstructor]
        public CloudSettings(Tape VHS, Event[] Events)
        {
            this.VHS = VHS;
            this.Events = Events;
        }

        public struct Tape
        {
            [JsonConstructor]
            public Tape(int Type, string Tag, string User, string Search)
            {
                this.Type = (TapeType)Type;
                this.Tag = Tag;
                this.User = User;
                this.Search = Search;
            }

            public TapeType Type;
            public string Tag;
            public string User;
            public string Search;
        }

        public struct Event
        {
            [JsonConstructor]
            public Event(string Title, string LinkUrl, string Desc, string IconUrl)
            {
                this.Title = Title;
                this.LinkUrl = LinkUrl;
                this.Desc = Desc;
                if (IconUrl.Length > 1)
                    Icon = WebUtility.GetIcon(WebUtility.ImageType.Image, Title, IconUrl);
            }

            public string Title;
            public string LinkUrl;
            public string Desc;
            public Texture2D Icon;
        }
    }
}
