using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class VHSWriter : CustomAppliance
    {
        public override string UniqueNameID => "VHS Writer";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("VHS Writer", "", new(), new()))
        };

        public override List<IApplianceProperty> Properties => new()
        {
            new CItemHolder(),
            new CItemHolderFilter() { Category = TapeItemCategory }
        };

        public override GameObject Prefab => GetPrefab(UniqueNameID);

        public override void SetupPrefab(GameObject prefab)
        {
            prefab.TryAddComponent<HoldPointContainer>().HoldPoint = prefab.transform.Find("HoldPoint");

            prefab.ApplyMaterialToChild("Computer", "Plastic", "Plastic - Black", "Hob Black");
        }

    }
}
