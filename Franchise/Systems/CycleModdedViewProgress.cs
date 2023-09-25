using Kitchen;
using KitchenHQ.Utility;
using KitchenLib.Utils;
using KitchenMods;
using Unity.Mathematics;

namespace KitchenHQ.Franchise
{
    internal class CycleModdedViewProgress : ItemInteractionSystem, IModSystem
    {
        public const int MaxDishes = 5;
        public const int MaxSettings = 5;

        protected override bool AllowActOrGrab => true;

        protected override bool IsPossible(ref InteractionData data) => Has<CModdedUpgradeView>(data.Attempt.Target) &&
            (!GDOContainer.ModdedDishes.IsNullOrEmpty() && !GDOContainer.ModdedSettings.IsNullOrEmpty() ||
            GDOContainer.ModdedDishes.Count > MaxDishes ||
            GDOContainer.ModdedSettings.Count > MaxSettings);

        protected override void Perform(ref InteractionData data)
        {
            if (!Require(data.Attempt.Target, out CModdedUpgradeView view))
                return;

            if (data.Attempt.Type == InteractionType.Act && 
                (view.IsDish && GDOContainer.ModdedDishes.Count > MaxDishes || 
                view.IsSetting && GDOContainer.ModdedSettings.Count > MaxSettings))
            {
                var pageCount = math.ceil(view.IsDish ? GDOContainer.ModdedDishes.Count / MaxDishes : GDOContainer.ModdedSettings.Count / MaxSettings) + 1;
                view.Page = (int)((view.Page + 1) % pageCount);
                Set(data.Attempt.Target, view);

                LogDebug($"[APPLIANCE] [PROGRESS] Flipping page to {view.Page}/{pageCount}");
            }
            else if (data.Attempt.Type == InteractionType.Grab && 
                !GDOContainer.ModdedDishes.IsNullOrEmpty() && !GDOContainer.ModdedSettings.IsNullOrEmpty())
            {
                data.ShouldAct = true;
                view.IsDish = !view.IsDish;
                view.Page = 0;
                Set(data.Attempt.Target, view);

                LogDebug($"[APPLIANCE] [PROGRESS] Swapping type to \"{(view.IsDish ? "Dish" : "Setting")}\"");
            }
            else
            {
                data.ShouldAct = false;

                LogDebug("[APPLIANCE] [PROGRESS] Skipping action.");
            }
        }
    }
}
