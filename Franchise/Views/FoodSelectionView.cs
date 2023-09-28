using KitchenHQ.Utility;
using Kitchen;
using KitchenData;
using KitchenMods;
using MessagePack;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class FoodSelectionView : UpdatableObjectView<FoodSelectionView.ViewData>
    {
        public Transform Container;
        public TextMeshPro Text;

        private GameObject Display;
        private float Timer = 3f;

        private bool Cycle = true;
        private bool IsModded = false;

        private ViewData Data;

        protected override void UpdateData(ViewData data)
        {
            if (!data.IsChangedFrom(Data))
                return;

            Data = data;
            Cycle = Data.SelectedType < 3;
            IsModded = Data.SelectedType == 2;

            if (Cycle)
            {
                var global = GameData.Main.GlobalLocalisation;
                Text.text = global.Text[Data.SelectedType == 0 ? "IHQ:FoodDefault" : Data.SelectedType == 1 ? "IHQ:FoodAll" : "IHQ:FoodModded"];
            }

            if (Cycle)
            {
                Timer = 3f;
                UpdateRandomDisplay();
                return;
            }

            UpdateDisplay(Data.SelectedID);
        }

        public void Update()
        {
            if (GameData.Main == null)
                return;

            if (!setup)
                Setup();

            if (!Cycle)
                return;

            Timer -= Time.deltaTime;
            if (Timer > 0)
                return;

            Timer = 3f;
            UpdateRandomDisplay();
        }

        private void UpdateRandomDisplay()
        {
            UpdateDisplay(KitchenData.RandomExtensions.Random((IsModded ? GDOContainer.ModdedDishes.Keys : GDOContainer.AllDishes.Keys).ToList()));
        }

        private void UpdateDisplay(int ID)
        {
            if (Display != null) Destroy(Display);

            if (!GameData.Main.TryGet(ID, out Dish selectedDish, true))
                return;

            if (!Cycle)
                Text.text = selectedDish.Info.Get().Name;

            if (selectedDish.IconPrefab == null)
                return;

            Display = Instantiate(selectedDish.IconPrefab);
            Display.transform.SetParent(Container.transform, false);
            Display.transform.localPosition = Vector3.zero;
            Display.transform.rotation = Quaternion.Euler(-45, 0, 0);
        }

        bool setup = false;
        private void Setup()
        {
            Text.text = GameData.Main.GlobalLocalisation["IHQ:FoodDefault"];
            UpdateRandomDisplay();
            Cycle = true;

            setup = true;
        }

        [MessagePackObject(false)]
        public struct ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(1)] public int SelectedType;
            [Key(2)] public int SelectedID;

            public IUpdatableObject GetRelevantSubview(IObjectView view) => view.GetSubView<FoodSelectionView>();

            public bool IsChangedFrom(ViewData check) => SelectedType != check.SelectedType || SelectedID != check.SelectedID;
        }

        private class UpdateView : IncrementalViewSystemBase<ViewData>, IModSystem
        {
            private EntityQuery Query;
            protected override void Initialise()
            {
                Query = GetEntityQuery(new ComponentType[] { typeof(CFoodSelector), typeof(CAppliance), typeof(CLinkedView) });
            }

            protected override void OnUpdate()
            {
                var entities = Query.ToEntityArray(Allocator.Temp);
                foreach (var entity in entities)
                {
                    var cSelector = GetComponent<CFoodSelector>(entity);

                    SendUpdate(GetComponent<CLinkedView>(entity), new()
                    {
                        SelectedID = cSelector.Selection,
                        SelectedType = math.clamp(cSelector.Type, 0, 3)
                    }, MessageType.SpecificViewUpdate);
                }
                entities.Dispose();
            }
        }
    }
}
