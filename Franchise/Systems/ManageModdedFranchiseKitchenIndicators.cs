using Kitchen;
using KitchenData;
using KitchenMods;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    [UpdateBefore(typeof(RebuildKitchen))]
    internal class ManageModdedFranchiseKitchenIndicators : IndicatorManager, IModSystem
    {
        private EntityQuery RebuildRequests;

        protected override void Initialise()
        {
            base.Initialise();
            RebuildRequests = GetEntityQuery(new ComponentType[] { typeof(RebuildKitchen.CRebuildKitchen) });
        }

        protected override ViewType ViewType => ViewType.KitchenTutorialInfo;

        protected override EntityQuery GetSearchQuery() => GetEntityQuery(new ComponentType[] { typeof(CPosition), typeof(CModdedKitchenTutorialPrompt) });

        protected override bool ShouldHaveIndicator(Entity candidate) => RebuildRequests.IsEmpty && Has<CBeingLookedAt>(candidate);

        protected override Entity CreateIndicator(Entity source)
        {
            var recipeIndex = EntityManager.GetComponentData<CModdedKitchenTutorialPrompt>(source).Index;
            int dishID = TryGetSingleton(out RebuildKitchen.SCurrentKitchen cCurrentKitchen) ? cCurrentKitchen.Dish : AssetReference.DishSteak;
            if (recipeIndex != 0 && dishID != 0 && Data.TryGet(cCurrentKitchen.Dish, out Dish dish, true))
            {
                if (recipeIndex > dish.AlsoAddRecipes.Count)
                    EntityManager.SetComponentData(source, new CModdedKitchenTutorialPrompt());
                else
                    dishID = dish.AlsoAddRecipes[recipeIndex - 1].ID;
            }

            var sourcePos = EntityManager.GetComponentData<CPosition>(source);
            Entity indicator = base.CreateIndicator(source);
            EntityManager.AddComponentData(indicator, new CPosition(sourcePos.Position - Vector3.forward));
            EntityManager.AddComponentData(indicator, new CFranchiseKitchenIndicator { Dish = dishID });
            return indicator;
        }
    }
}
