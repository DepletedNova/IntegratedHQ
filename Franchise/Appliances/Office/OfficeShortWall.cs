using KitchenHQ.Utility.Abstract;
using KitchenData;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class OfficeShortWall : FranchiseAppliance
    {
        public override string UniqueNameID => "Short Wall Section";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Office Short Wall", "", new(), new()))
        };

        public override void SetupPrefab(GameObject prefab)
        {
            prefab.ApplyMaterialToChild("Lower", "Wall Main", "BaseDefault", "Wall Main");
            prefab.ApplyMaterialToChild("Middle", "Wall Main", "Wall Main");
            prefab.ApplyMaterialToChild("Top", "Wall Top");
        }
    }
}
