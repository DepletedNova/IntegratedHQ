using Kitchen;
using KitchenData;
using KitchenHQ.Utility;
using KitchenLib.Customs;
using KitchenLib.Utils;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class ModRoomTable : StaticFranchiseAppliance
    {
        public override string UniqueNameID => "Mod Room Table";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Mod Room Table", "", new(), new()))
        };

        public override GameObject Prefab => GetPrefab(UniqueNameID);
        public override void SetupPrefab(GameObject prefab)
        {
            prefab.ApplyMaterialToChild("Table", "Wood - Default", "Metal");
        }

    }
}
