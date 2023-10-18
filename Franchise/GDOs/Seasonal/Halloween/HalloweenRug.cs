using KitchenData;
using KitchenHQ.Utility;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class HalloweenRug : StaticFranchiseAppliance
    {
        public override string UniqueNameID => "Halloween Rug";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Halloween Rug", "", new(), new()))
        };

        public override GameObject Prefab => GetPrefab(UniqueNameID);
        public override void SetupPrefab(GameObject prefab)
        {
            prefab.ApplyMaterialToChild("Rug", "Rug - Vine", "Clothing Black");
        }
    }
}
