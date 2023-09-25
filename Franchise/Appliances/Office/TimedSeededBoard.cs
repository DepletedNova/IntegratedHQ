using KitchenHQ.Utility.Abstract;
using KitchenData;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class TimedSeededBoard : FranchiseAppliance
    {
        public override string UniqueNameID => "Timed Seeded Board";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Timed Seeded Board", "", new(), new()))
        };

        public override void SetupPrefab(GameObject prefab)
        {
            var fixedRun = GetGDO<Appliance>(AssetReference.FixedRunVisual).Prefab;
            Object.Instantiate(fixedRun.GetChild("Daily")).transform.SetParent(prefab.transform, false);
            Object.Instantiate(fixedRun.GetChild("Weekly")).transform.SetParent(prefab.transform, false);
            Object.Instantiate(fixedRun.GetChild("FixedRunBoard")).transform.SetParent(prefab.transform, false);
        }
    }
}
