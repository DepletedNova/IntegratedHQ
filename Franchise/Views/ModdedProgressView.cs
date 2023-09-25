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


        }

        private bool IsInitialized = false;
        private void Setup()
        {
            if (BaseDish != null)
            {
                var dishes = GDOContainer.ModdedDishes.Values.ToList();
                for (int i = 0; i < dishes.Count; i++)
                {
                    var page = (int)Mathf.Floor(i / CycleUpgradeView.MaxDishes);
                    LogDebug($"[APPLIANCE] [PROGRESS] Selecting dish {i} under page {page}");
                    if (page + 1 > DishPages.Count)
                    {
                        var addedPage = new GameObject($"Dish Page - {page + 1}");
                        addedPage.transform.SetParent(gameObject.transform, false);
                        addedPage.transform.localPosition = new(-1.1f, 0.505f, 0f);
                        addedPage.transform.localRotation = Quaternion.identity;
                        //page.SetActive(false);
                        DishPages.Add(addedPage);
                        LogDebug($"[APPLIANCE] [PROGRESS] Creating Page {page}");
                    }

                    var dish = Instantiate(BaseDish);
                    dish.SetActive(true);
                    dish.transform.SetParent(DishPages[page].transform, false);
                    dish.transform.localPosition = new(2.2f / (CycleUpgradeView.MaxDishes - 1) * (i % CycleUpgradeView.MaxDishes), 0f, 0f);
                    dish.GetChild("Name").GetComponent<TextMeshPro>().text = dishes[i].Name;
                    dish.GetChild("Dish").GetComponent<MeshRenderer>().material.SetTexture(Image, PrefabSnapshot.GetSnapshot(dishes[i].IconPrefab));

                    LogDebug($"[APPLIANCE] [PROGRESS] Added Dish \"{dishes[i].Name}\" to Page {page}");
                }
            }

            if (BaseSetting != null)
            {

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
