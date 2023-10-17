using Kitchen;
using KitchenData;
using KitchenHQ.Utility;
using KitchenLib.Customs;
using KitchenLib.Utils;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class SubscribedModsView : CustomAppliance, ISpecialLocalise
    {
        public override string UniqueNameID => "Subscribed Mods View";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Subscribed Mods View", "", new(), new()))
        };

        public override GameObject Prefab => GetPrefab(UniqueNameID);

        public Dictionary<string, List<(Locale locale, string text)>> Localisations => new()
        {
            { "IHQ:SubbedMods", new()
                {
                    (Locale.English, "Mods Installed")
                }
            }
        };

        public override void SetupPrefab(GameObject prefab)
        {
            var table = prefab.GetChild("Table");
            table.ApplyMaterialToChild("Top", "Wood - Default");
            table.ApplyMaterialToChild("Cloth", "Cloth - Cheap");
            table.ApplyMaterialToChildren("Leg", "Wood - Default");

            var view = prefab.TryAddComponent<SubscribedModsViewView>();

            view.Label = prefab.transform.CreateLabel("Label", new(0, 0.7f, -0.75f), Quaternion.Euler(40, 0, 0),
                MaterialUtils.GetExistingMaterial("Alphakind Atlas Material_1"), FontUtils.GetExistingTMPFont("Floating Text"), 73.07f, 2.85f,
                "10/10\n<size=1.5>Mods Installed</size>").GetComponent<TextMeshPro>();

            view.GenericPodium = prefab.ApplyMaterialToChild("Podium", "Metal");
            view.PartialPodium = prefab.ApplyMaterialToChild("Partial", "Plastic - Shiny Gold", "Paint - Gold");
        }
    }
}
