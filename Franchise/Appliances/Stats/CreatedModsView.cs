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
    public class CreatedModsView : CustomAppliance, ISpecialLocalise
    {
        public override string UniqueNameID => "Created Mods View";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Created Mods View", "", new(), new()))
        };

        public override GameObject Prefab => GetPrefab(UniqueNameID);

        public Dictionary<string, List<(Locale locale, string text)>> Localisations => new()
        {
            { "IHQ:CreatedMods", new()
                {
                    (Locale.English, "Mods Created/Mod Created")
                }
            },
        };

        public override void SetupPrefab(GameObject prefab)
        {
            var table = prefab.GetChild("Table");
            table.ApplyMaterialToChild("Top", "Wood - Default");
            table.ApplyMaterialToChild("Cloth", "Cloth - Cheap");
            table.ApplyMaterialToChildren("Leg", "Wood - Default");

            prefab.ApplyMaterialToChild("Icon", "Flat Image - Faded");
            prefab.ApplyMaterialToChild("Icon/Outline", "Paper - Menu Header");

            var label = prefab.transform.CreateLabel("Label", new(0, 0.7f, -0.75f), Quaternion.Euler(40, 0, 0),
                MaterialUtils.GetExistingMaterial("Alphakind Atlas Material_1"), FontUtils.GetExistingTMPFont("Floating Text"), 73.07f, 2.85f,
                "20\n<size=1.5>Mods Created</size>").GetComponent<TextMeshPro>();

            var cmvv = prefab.TryAddComponent<CreatedModsViewView>();
            cmvv.MaxTimer = 10f;
            cmvv.Renderer = prefab.GetChild("Icon").GetComponent<MeshRenderer>();
            cmvv.Label = label;
        }
    }
}
