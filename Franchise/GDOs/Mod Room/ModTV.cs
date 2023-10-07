﻿using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class ModTV : CustomAppliance
    {
        public override string UniqueNameID => "Mod TV";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Mod TV", "", new(), new()))
        };

        public override GameObject Prefab => GetPrefab(UniqueNameID);
        public override void SetupPrefab(GameObject prefab)
        {
            prefab.TryAddComponent<HoldPointContainer>().HoldPoint = prefab.transform.Find("HoldPoint");

            prefab.ApplyMaterialToChild("TV", "Wood", "Wood - Default", "Metal Very Dark", "Metal", "Wood 5 - Grey");
            prefab.ApplyMaterialToChild("Legs", "Metal");

            prefab.ApplyMaterialToChild("Screen", "Flat Image");
        }

    }
}
