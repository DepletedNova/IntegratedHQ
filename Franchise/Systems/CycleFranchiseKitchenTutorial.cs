using Kitchen;
using KitchenData;
using KitchenLib.Utils;
using KitchenMods;
using Unity.Entities;

namespace KitchenHQ.Franchise
{
    internal class CycleFranchiseKitchenTutorial : ItemInteractionSystem, IModSystem
    {
        private EntityQuery RebuildRequests;

        protected override void Initialise()
        {
            base.Initialise();
            RebuildRequests = GetEntityQuery(new ComponentType[] { typeof(RebuildKitchen.CRebuildKitchen) });
        }

        protected override bool AllowActOrGrab => true;

        protected override bool IsPossible(ref InteractionData data) =>
            RebuildRequests.IsEmpty && Has<CModdedKitchenTutorialPrompt>(data.Attempt.Target) &&
            Data.TryGet(data.Context.Get<RebuildKitchen.SCurrentKitchen>().Dish, out Dish dish) && !dish.AlsoAddRecipes.IsNullOrEmpty();

        protected override void Perform(ref InteractionData data)
        {
            var dish = Data.Get<Dish>(data.Context.Get<RebuildKitchen.SCurrentKitchen>().Dish);
            var index = (data.Context.Get<CModdedKitchenTutorialPrompt>(data.Attempt.Target).Index + 1) % (dish.AlsoAddRecipes.Count + 1);
            data.Context.Set(data.Attempt.Target, new CModdedKitchenTutorialPrompt
            {
                Index = index
            });

            LogDebug($"[APPLIANCE] [RECIPE] Swapping to page: {index}");
        }
    }
}
