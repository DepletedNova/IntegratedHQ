using KitchenHQ.Utility.Abstract;
using KitchenData;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class LoadingBayText : FranchiseAppliance
    {
        public override string UniqueNameID => "Loading Bay Text";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Loading Bay Text", "", new(), new()))
        };

        public override void SetupPrefab(GameObject prefab)
        {
            var textPrefab = Object.Instantiate(GetGDO<Appliance>(AssetReference.GarageDecorations).Prefab.GetChild("GameObject/Loading Bay Text"));
            textPrefab.transform.SetParent(prefab.transform, false);
            textPrefab.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0.06f, 0f);
        }
    }
}
