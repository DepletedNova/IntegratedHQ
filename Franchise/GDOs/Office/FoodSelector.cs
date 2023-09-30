using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class FoodSelector : CustomAppliance, ISpecialLocalise
    {
        public override string UniqueNameID => "Food Selector";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Food Selector", "", new(), new()))
        };

        public Dictionary<string, List<(Locale locale, string text)>> Localisations => new()
        {
            { "IHQ:FoodDefault", new()
                {
                    (Locale.English, "Default")
                }
            },
            { "IHQ:FoodAll", new()
                {
                    (Locale.English, "All")
                }
            },
            { "IHQ:FoodModded", new()
                {
                    (Locale.English, "Modded")
                }
            },
        };

        public override List<IApplianceProperty> Properties => new()
        {
            new CFoodSelector()
        };

        public override GameObject Prefab => GetPrefab(UniqueNameID);
        public override void SetupPrefab(GameObject prefab)
        {
            var basePrefab = GetGDO<Appliance>(AssetReference.SettingSelector).Prefab;
            Object.Instantiate(basePrefab.GetChild("Collider")).transform.SetParent(prefab.transform, false);
            Object.Instantiate(basePrefab.GetChild("NavMesh Obstacle")).transform.SetParent(prefab.transform, false);

            var label = Object.Instantiate(basePrefab.GetChild("Floor Label"));
            label.transform.SetParent(prefab.transform, false);
            var cLabel = label.GetChild("Label");
            cLabel.GetComponent<AutoGlobalLocal>().Text = "IHQ:FoodDefault";

            var container = Object.Instantiate(basePrefab.GetChild("Container"));
            container.transform.SetParent(prefab.transform, false);
            container.transform.position += new Vector3(0f, 0.65f, 0f);

            var selector = prefab.TryAddComponent<FoodSelectionView>();
            selector.Container = container.transform;
            selector.Text = cLabel.GetComponent<TextMeshPro>();
        }

    }
}
