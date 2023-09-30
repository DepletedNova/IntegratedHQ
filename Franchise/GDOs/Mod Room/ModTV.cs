using Kitchen;
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

        public override List<IApplianceProperty> Properties => new()
        {
            new CItemHolder(),
            new CItemHolderFilter() { Category = ModRoomReferences.TapeItemCategory }
        };

        public override GameObject Prefab => GetPrefab(UniqueNameID);
        public override void SetupPrefab(GameObject prefab)
        {
            prefab.TryAddComponent<HoldPointContainer>().HoldPoint = prefab.transform.Find("HoldPoint");

            prefab.ApplyMaterialToChild("Tele", "Wood - Default", "Metal Dark", "Plastic - Black", "Hob Black");

            var screen = prefab.GetChild("Screen");
            screen.ApplyMaterialToChild("Lit", "Lamp Beige - On");
            screen.ApplyMaterialToChild("Icon", "Flat Image");
        }

    }
}
