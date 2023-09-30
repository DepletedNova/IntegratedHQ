using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class ApplianceSwapper : CustomAppliance, ISpecialLocalise
    {
        public override string UniqueNameID => "Appliance Swapper";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Appliance Swapper", "", new(), new()))
        };

        public Dictionary<string, List<(Locale locale, string text)>> Localisations => new()
        {
            { "IHQ:ApplianceSwapper", new()
                {
                    (Locale.English, "Cycle\nHQ")
                }
            },
        };

        public override List<IApplianceProperty> Properties => new()
        {
            new CItemHolder(),
            new CDisplayDuration
            {
                Process = GetCustomGameDataObject<SwitchProcess>().ID
            },
            new CTakesDuration
            {
                Total = 2f,
                Mode = InteractionMode.Items,
                Manual = true,
                ManualNeedsEmptyHands = true
            },

            new CFranchiseApplianceSwapper()
        };

        public override GameObject Prefab => GetPrefab(UniqueNameID);
        public override void SetupPrefab(GameObject prefab)
        {
            prefab.ApplyMaterialToChild("Container/Spinner", "Glowing Blue Soft");

            var basePrefab = GetGDO<Appliance>(AssetReference.PracticeModeTrigger).Prefab;

            var floorLabel = Object.Instantiate(basePrefab.GetChild("Floor Label"));
            floorLabel.transform.SetParent(prefab.transform, false);

            var localisation = floorLabel.GetChild("Label").GetComponent<AutoGlobalLocal>();
            localisation.Text = "IHQ:ApplianceSwapper";

            prefab.TryAddComponent<ActivateSwitchView>().Animator = prefab.GetComponent<Animator>();
        }

    }
}
