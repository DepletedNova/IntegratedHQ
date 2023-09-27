using KitchenData;
using KitchenHQ.Utility;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class OfficeArchway : StaticFranchiseAppliance
    {
        public override string UniqueNameID => "Office Archway";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Office Archway", "", new(), new()))
        };

        public override void SetupPrefab(GameObject prefab)
        {
            prefab.ApplyMaterialToChild("arch", "Wall Top");
        }
    }
}
