using KitchenHQ.Utility.Abstract;
using KitchenData;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class DebugMarker : FranchiseAppliance
    {
        public override string UniqueNameID => "Marker";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Debug Marker", "", new(), new()))
        };

        public override void SetupPrefab(GameObject prefab)
        {
            prefab.ApplyMaterialToChild("ping", "Ping");
        }
    }
}
