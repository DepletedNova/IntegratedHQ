using KitchenData;
using KitchenHQ.Utility;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class WorkshopMarkers : StaticFranchiseAppliance
    {
        public override string UniqueNameID => "Workshop Markers";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Workshop Markers", "", new(), new()))
        };

        public override void SetupPrefab(GameObject prefab)
        {
            prefab.ApplyMaterialToChildren("Marks", "Road Bright Yellow Markings 1");
        }
    }
}
