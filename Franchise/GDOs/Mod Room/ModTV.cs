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
    public class ModTV : CustomAppliance
    {
        public override string UniqueNameID => "Mod TV";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Mod TV", "", new(), new()))
        };

        public override GameObject Prefab => GetPrefab(UniqueNameID);
        public override void SetupPrefab(GameObject prefab)
        {
            prefab.TryAddComponent<HoldPointContainer>().HoldPoint = prefab.transform.Find("HoldPoint");

            prefab.ApplyMaterialToChild("TV", "Wood", "Wood - Default", "Metal Very Dark", "Metal", "Wood 5 - Grey");
            prefab.ApplyMaterialToChild("Legs", "Metal");

            var screen = prefab.GetChild("Screen");

            prefab.ApplyMaterialToChild("Screen", "Flat Image");
            screen.ApplyMaterialToChild("Texture", "Flat Image");

            var label = screen.transform.CreateLabel("Label", new(0, 0.7f, 0.1f), Quaternion.Euler(40, 0, 0),
                MaterialUtils.GetExistingMaterial("Alphakind Atlas Material_1"), FontUtils.GetExistingTMPFont("Floating Text"), 73.07f, 2f,
                "Mod Name!");

            var view = prefab.TryAddComponent<TelevisionView>();
            view.Renderer = screen.GetChild("Texture").GetComponent<MeshRenderer>();
            view.Screen = screen;
            view.Label = label.GetComponent<TextMeshPro>();
        }

    }
}
