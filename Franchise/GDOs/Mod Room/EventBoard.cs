using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class EventBoard : CustomAppliance, ISpecialLocalise
    {
        public override string UniqueNameID => "Event Board";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Event Board", "", new(), new()))
        };

        public Dictionary<string, List<(Locale locale, string text)>> Localisations => new()
        {
            { "IHQ:EventBoard", new()
                {
                    (Locale.English, "Events")
                }
            },
        };

        public override GameObject Prefab => GetPrefab(UniqueNameID);
        public override void SetupPrefab(GameObject prefab)
        {
            prefab.ApplyMaterialToChild("Board/Board", "Wood - Corkboard");
            prefab.ApplyMaterialToChild("Board/Frame", "Wood");

            var notes = prefab.GetChild("Notes");
            notes.ApplyMaterialToChildren("Yellow", "Paper - Postit Yellow");
            notes.ApplyMaterialToChildren("Red", "Paper - Postit Red");
            notes.ApplyMaterialToChildren("Blue", "Paper - Postit Blue");

            var events = prefab.GetChild("Events");
            events.GetChild("Pins").ApplyMaterialToChildren("", "Wood 1");
            events.ApplyMaterialToChild("Paper", "Paper - Newspaper");
            events.ApplyMaterialToChild("Poster", "Flat Image - Faded");
        }

    }
}
