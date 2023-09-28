using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class Refresher : CustomAppliance, ISpecialLocalise
    {
        public override string UniqueNameID => "Refresher";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Refresher", "", new(), new()))
        };

        public Dictionary<string, List<(Locale locale, string text)>> Localisations => new()
        {
            { "IHQ:Refresher", new()
                {
                    (Locale.English, "Refresh Choices")
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
                Total = 1f,
                Mode = InteractionMode.Items,
                Manual = true,
                ManualNeedsEmptyHands = true
            },

            new CChoiceRefresher()
        };

        public override GameObject Prefab => GetPrefab(UniqueNameID);
        public override void SetupPrefab(GameObject prefab)
        {
            prefab.ApplyMaterialToChild("Container/Spinner", "Glowing Blue Soft");

            var basePrefab = GetGDO<Appliance>(AssetReference.PracticeModeTrigger).Prefab;

            var floorLabel = Object.Instantiate(basePrefab.GetChild("Floor Label"));
            floorLabel.transform.SetParent(prefab.transform, false);

            var localisation = floorLabel.GetChild("Label").GetComponent<AutoGlobalLocal>();
            localisation.Text = "IHQ:Refresher";

            prefab.TryAddComponent<ActivateSwitchView>().Animator = prefab.GetComponent<Animator>();
        }

    }
}
