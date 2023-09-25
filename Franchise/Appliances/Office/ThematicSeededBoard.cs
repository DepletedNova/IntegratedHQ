using KitchenHQ.Utility.Abstract;
using Kitchen;
using KitchenData;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

namespace KitchenHQ.Franchise
{
    public class ThematicSeededBoard : FranchiseAppliance, ISpecialLocalise
    {
        public override string UniqueNameID => "Thematic Seeded Board";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Thematic Seeded Board", "", new(), new()))
        };

        public Dictionary<string, List<(Locale locale, string text)>> Localisations => new()
        {
            { "IHQ:Thematic", new()
                {
                    (Locale.English, "Thematic")
                }
            }
        };

        public override void SetupPrefab(GameObject prefab)
        {
            var fixedRun = GetGDO<Appliance>(AssetReference.SpeedrunBoardVisual).Prefab;
            var board = Object.Instantiate(fixedRun.GetChild("FixedRunBoard"));
            board.transform.SetParent(prefab.transform, false);
            board.transform.localPosition = new Vector3(0f, 0.06f, -0.03f);
            var text = Object.Instantiate(fixedRun.GetChild("Left"));
            text.transform.SetParent(prefab.transform, false);
            text.GetComponent<RectTransform>().localPosition = new Vector3(0.025f, 1.45f, 0.29f);
            text.GetComponent<AutoGlobalLocal>().Text = "IHQ:Thematic";
        }
    }
}
