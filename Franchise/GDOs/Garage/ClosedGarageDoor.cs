﻿using KitchenData;
using KitchenHQ.Utility;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class ClosedGarageDoor : StaticFranchiseAppliance
    {
        public override string UniqueNameID => "Garage Door";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Closed Garage Door", "", new(), new()))
        };

        public override void SetupPrefab(GameObject prefab)
        {
            prefab.ApplyMaterialToChild("Door Light", "Glowing Outside");
            prefab.ApplyMaterialToChild("Door", "Metal");
            prefab.ApplyMaterialToChild("Frame", "Wall Top");
        }
    }
}
