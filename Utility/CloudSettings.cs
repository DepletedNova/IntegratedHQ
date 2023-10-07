using KitchenHQ.Franchise;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace KitchenHQ.Utility
{
    public class CloudSettings
    {
        public STape VHS;

        public List<CEvent> Events;

        [JsonConstructor]
        public CloudSettings(STape VHS, CEvent[] Events)
        {
            this.VHS = VHS;

            this.Events = new();
            this.Events.AddRange(Events);
        }

    }
}
