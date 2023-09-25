using Kitchen;
using KitchenHQ.Utility;
using KitchenLib.Utils;
using MessagePack;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class ModdedProgressView : UpdatableObjectView<ModdedProgressView.ViewData>
    {
        private static readonly int Image = Shader.PropertyToID("_Image");

        private ViewData Data;

        public TextMeshPro Label;

        public GameObject BaseDish;
        private List<GameObject> DishPages = new();

        public GameObject BaseSetting;
        private List<GameObject> SettingPages = new();

        protected override void UpdateData(ViewData data)
        {
            Data = data;

            if (!IsInitialized)
            {
                IsInitialized = true;
                Setup();
            }

            // Disable all
            foreach (var setting in SettingPages)
                setting.SetActive(false);
            foreach (var dish in DishPages)
                dish.SetActive(false);

            if (GDOContainer.ModdedDishes.IsNullOrEmpty() && GDOContainer.ModdedSettings.IsNullOrEmpty())
            {
                UpdateText(string.Format("<size=1.5>{0}</size>", Localisation["IHQ:ProgressEmptyPage"]));
                return;
            }

            List<GameObject> pages = Data.IsDish ? DishPages : SettingPages;
            pages[Data.Page].SetActive(true);
            UpdateText(string.Format("{0}\n<size=1.5>{1}</size>",
                Data.IsDish ? GDOContainer.ModdedDishes.Count : GDOContainer.ModdedSettings.Count,
                Localisation[data.IsDish ? "IHQ:ProgressDishPage" : "IHQ:ProgressSettingsPage"]));
        }

        private void UpdateText(string text)
        {
            if (Label != null)
                Label.text = text;
        }

        private bool IsInitialized = false;
        private void Setup()
        {
            if (BaseDish != null)
            {
                var dishes = GDOContainer.ModdedDishes.Values.ToList();
                for (int i = 0; i < dishes.Count; i++)
                {
                    var page = (int)Mathf.Floor(i / CycleModdedViewProgress.MaxDishes);
                    LogDebug($"[APPLIANCE] [PROGRESS] Selecting dish {i} under page {page}");
                    if (page + 1 > DishPages.Count)
                    {
                        var addedPage = new GameObject($"Dish Page - {page + 1}");
                        addedPage.transform.SetParent(gameObject.transform, false);
                        addedPage.transform.localPosition = new(-1.1f, 0.505f, 0f);
                        addedPage.transform.localRotation = Quaternion.identity;
                        addedPage.SetActive(false);
                        DishPages.Add(addedPage);
                        LogDebug($"[APPLIANCE] [PROGRESS] Creating Dish Page {page}");
                    }

                    var dish = Instantiate(BaseDish);
                    dish.SetActive(true);
                    dish.transform.SetParent(DishPages[page].transform, false);
                    dish.transform.localPosition = new(2.2f / (CycleModdedViewProgress.MaxDishes - 1) * (i % CycleModdedViewProgress.MaxDishes), 0f, 0f);
                    dish.GetChild("Name").GetComponent<TextMeshPro>().text = dishes[i].Name;
                    dish.GetChild("Dish").GetComponent<MeshRenderer>().material.SetTexture(Image, PrefabSnapshot.GetSnapshot(dishes[i].IconPrefab));

                    LogDebug($"[APPLIANCE] [PROGRESS] Added Dish \"{dishes[i].Name}\" to Page {page}");
                }
            }

            if (BaseSetting != null)
            {
                var settings = GDOContainer.ModdedSettings.Values.ToList();
                for (int i = 0; i < settings.Count; i++)
                {
                    var page = (int)Mathf.Floor(i / CycleModdedViewProgress.MaxSettings);
                    LogDebug($"[APPLIANCE] [PROGRESS] Selecting setting {i} under page {page}");
                    if (page + 1 > SettingPages.Count)
                    {
                        var addedPage = new GameObject($"Setting Page - {page + 1}");
                        addedPage.transform.SetParent(gameObject.transform, false);
                        addedPage.transform.localPosition = new(-1.1f, 0.505f, 0f);
                        addedPage.transform.localRotation = Quaternion.identity;
                        addedPage.SetActive(false);
                        SettingPages.Add(addedPage);
                        LogDebug($"[APPLIANCE] [PROGRESS] Creating Setting Page {page}");
                    }

                    var setting = Instantiate(BaseSetting);
                    setting.SetActive(true);
                    setting.transform.SetParent(SettingPages[page].transform, false);
                    setting.transform.localPosition = new(2.2f / (CycleModdedViewProgress.MaxSettings - 1) * (i % CycleModdedViewProgress.MaxSettings), 0f, 0f);
                    setting.GetChild("Name").GetComponent<TextMeshPro>().text = settings[i].Name;

                    var globe = Instantiate(settings[i].Prefab);
                    globe.transform.SetParent(setting.transform.Find("Container"));
                    globe.transform.localScale = Vector3.one;
                    globe.transform.localPosition = Vector3.zero;

                    LogDebug($"[APPLIANCE] [PROGRESS] Added Setting \"{settings[i].Name}\" to Page {page}");
                }
            }
        }

        [MessagePackObject(false)]
        public struct ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)] public int Page;
            [Key(1)] public bool IsDish;

            public IUpdatableObject GetRelevantSubview(IObjectView view) => view.GetSubView<ModdedProgressView>();

            public bool IsChangedFrom(ViewData check) => check.Page != Page || check.IsDish != IsDish;
        }

        private class UpdateView : IncrementalViewSystemBase<ViewData>
        {
            private EntityQuery ProgressViews;

            protected override void Initialise()
            {
                base.Initialise();
                ProgressViews = GetEntityQuery(new ComponentType[] { typeof(CAppliance), typeof(CLinkedView), typeof(CModdedUpgradeView) });
            }

            protected override void OnUpdate()
            {
                var views = ProgressViews.ToComponentDataArray<CLinkedView>(Allocator.Temp);
                var upgrades = ProgressViews.ToComponentDataArray<CModdedUpgradeView>(Allocator.Temp);

                for (int i = 0; i < views.Length; i++)
                {
                    var view = views[i];
                    var upgrade = upgrades[i];

                    SendUpdate(view, new()
                    {
                        IsDish = upgrade.IsDish,
                        Page = upgrade.Page
                    }, MessageType.SpecificViewUpdate);
                }

                views.Dispose();
                upgrades.Dispose();
            }
        }
    }
}
