using KitchenData;
using KitchenHQ.Utility;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class HalloweenGarland : StaticFranchiseAppliance
    {
        public override string UniqueNameID => "Halloween Garland";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Halloween Garland", "", new(), new()))
        };

        public override GameObject Prefab => GetPrefab(UniqueNameID);
        public override void SetupPrefab(GameObject prefab)
        {
            prefab.ApplyMaterialToChild("Garland", "Clothing Black", "Plastic - Black", "Plastic - Orange");
        }
    }
}
