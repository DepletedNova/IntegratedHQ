using KitchenData;
using KitchenHQ.Utility;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class Pumpkin1 : StaticFranchiseAppliance
    {
        public override string UniqueNameID => "Pumpkin 1";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Pumpkin 1", "", new(), new()))
        };

        public override GameObject Prefab => GetPrefab(UniqueNameID);
        public override void SetupPrefab(GameObject prefab)
        {
            prefab.ApplyMaterialToChildren("Pumpkin", "Plastic - Orange", "Wood - Dark");
        }
    }

    public class Pumpkin2 : StaticFranchiseAppliance
    {
        public override string UniqueNameID => "Pumpkin 2";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Pumpkin 2", "", new(), new()))
        };

        public override GameObject Prefab => GetPrefab(UniqueNameID);
        public override void SetupPrefab(GameObject prefab)
        {
            prefab.ApplyMaterialToChildren("Pumpkin", "Plastic - Orange", "Wood - Dark");
        }
    }
}
