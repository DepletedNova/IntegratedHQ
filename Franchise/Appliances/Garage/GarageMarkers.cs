﻿using KitchenHQ.Utility.Abstract;
using KitchenData;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class GarageMarkers : FranchiseAppliance
    {
        public override string UniqueNameID => "Garage Markers";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Garage Markers", "", new(), new()))
        };

        public override void SetupPrefab(GameObject prefab)
        {
            prefab.ApplyMaterialToChildren("Marks", "Road Bright Yellow Markings 1");
        }
    }
}
