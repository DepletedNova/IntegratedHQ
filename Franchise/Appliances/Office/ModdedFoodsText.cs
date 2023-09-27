using Kitchen;
using KitchenData;
using KitchenHQ.Utility;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class ModdedFoodsText : StaticFranchiseAppliance, ISpecialLocalise
    {
        public override string UniqueNameID => "Modded Foods Text";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Modded Foods Text", "", new(), new()))
        };

        public Dictionary<string, List<(Locale locale, string text)>> Localisations => new()
        {
            { "IHQ:Modded", new()
                {
                    (Locale.English, "Modded")
                }
            }
        };

        public override void SetupPrefab(GameObject prefab)
        {
            prefab.ApplyMaterialToChild("Rug", "Clothing Red", "Clothing - Yellow");

            var textPrefab = Object.Instantiate(GetGDO<Appliance>(AssetReference.GarageDecorations).Prefab.GetChild("GameObject/Loading Bay Text"));
            textPrefab.transform.SetParent(prefab.transform, false);
            textPrefab.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0.07f, 0f);
            textPrefab.GetComponent<AutoGlobalLocal>().Text = "IHQ:Modded";
        }
    }
}
