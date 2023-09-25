using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class ModdedProgress : CustomAppliance, ISpecialLocalise
    {
        public override string UniqueNameID => "Modded Progress View";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Modded Progress View", "", new(), new()))
        };

        public override GameObject Prefab => GetPrefab(UniqueNameID);

        public Dictionary<string, List<(Locale locale, string text)>> Localisations => new()
        {
            { "IHQ:ProgressDishPage", new()
                {
                    (Locale.English, "Modded Dishes")
                }
            },
            { "IHQ:ProgressSettingsPage", new()
                {
                    (Locale.English, "Modded Settings")
                }
            },
            { "IHQ:ProgressEmptyPage", new()
                {
                    (Locale.English, "No Modded Settings/Dishes")
                }
            },
        };

        public override void SetupPrefab(GameObject prefab)
        {
            // Table
            prefab.ApplyMaterialToChild("Table/Top", "Wood - Default", "Felt");
            prefab.ApplyMaterialToChild("Table/Legs", "Wood 1 - Dim");

            var TMP = CreateLabel("Label", prefab.transform, new(0, 0.7f, -0.75f), Quaternion.Euler(40, 0, 0),
                MaterialUtils.GetExistingMaterial("Alphakind Atlas Material_1"), FontUtils.GetExistingTMPFont("Floating Text"), 73.07f, 2.85f,
                "0\n<size=1.5>Modded Dishes</size>").GetComponent<TextMeshPro>();

            // Dish
            var dish = prefab.GetChild("Dish");
            dish.ApplyMaterialToChild("Page", "Paper - Menu Body", "Paper - Menu Header");
            dish.ApplyMaterialToChild("Dish", "Flat Image - Faded");

            var dishLabel = CreateLabel("Name", dish.transform, new(0, 0.02f, -0.275f), Quaternion.Euler(90, 180, 0),
                MaterialUtils.GetExistingMaterial("Cake n Truffles Atlas Material_3"), FontUtils.GetExistingTMPFont("Map Label"), 0f, 145.35f,
                "Dish");
            var dishTMP = dishLabel.GetComponent<TextMeshPro>();
            dishTMP.enableAutoSizing = true;
            dishTMP.fontSizeMin = 18;
            dishTMP.fontSizeMax = 160;
            dishTMP.enableWordWrapping = true;
            dishTMP.wordWrappingRatios = 0.4f;
            var dishRect = dishLabel.GetComponent<RectTransform>();
            dishRect.sizeDelta = new(50, 25f);
            dishRect.localScale = Vector3.one * 0.01f;

            // Setting
            var setting = prefab.GetChild("Setting");

            var settingLabel = CreateLabel("Name", setting.transform, new(0, 0.02f, 0.3f), Quaternion.Euler(90, 180, 0),
                MaterialUtils.GetExistingMaterial("Cake n Truffles Atlas Material_3"), FontUtils.GetExistingTMPFont("Map Label"), 0f, 145.35f,
                "Setting");
            var settingTMP = settingLabel.GetComponent<TextMeshPro>();
            settingTMP.enableAutoSizing = true;
            settingTMP.fontSizeMin = 18;
            settingTMP.fontSizeMax = 160;
            settingTMP.enableWordWrapping = true;
            settingTMP.wordWrappingRatios = 0.4f;
            var settingRect = settingLabel.GetComponent<RectTransform>();
            settingRect.sizeDelta = new(55, 30f);
            settingRect.localScale = Vector3.one * 0.01f;

            // View
            var view = prefab.TryAddComponent<ModdedProgressView>();
            view.BaseDish = dish;
            view.BaseSetting = setting;
            view.Label = TMP;
        }

        private GameObject CreateLabel(string name, Transform parent, Vector3 position, Quaternion rotation, Material material, TMP_FontAsset font, float spacing, float size, string text)
        {
            var label = new GameObject(name, new[] { typeof(RectTransform), typeof(MeshRenderer), typeof(TextMeshPro) });

            var labelTransform = label.TryAddComponent<RectTransform>();
            labelTransform.SetParent(parent, false);
            labelTransform.localPosition = position;
            labelTransform.localRotation = rotation;

            label.TryAddComponent<MeshRenderer>().material = material;

            var TMP = label.TryAddComponent<TextMeshPro>();
            TMP.font = font;
            TMP.lineSpacing = spacing;
            TMP.fontSize = size;
            TMP.alignment = TextAlignmentOptions.Center;
            TMP.text = text;

            return label;
        }

    }
}
