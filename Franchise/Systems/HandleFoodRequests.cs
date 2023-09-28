using KitchenHQ.Utility;
using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenMods;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;

namespace KitchenHQ.Franchise
{
    public class HandleFoodRequests : FranchiseSystem, IModSystem
    {
        private EntityQuery Pedestals;
        private EntityQuery Maps;

        private EntityQuery FixedMaps;

        private EntityQuery DishUpgrades;

        protected override void Initialise()
        {
            base.Initialise();
            Pedestals = GetEntityQuery(new QueryHelper().All(typeof(CFoodPedestal)).None(typeof(CFixedFoodPedestal)));
            Maps = GetEntityQuery(new QueryHelper().All(typeof(CDishChoice)).None(typeof(CFixedDishChoice)));

            FixedMaps = GetEntityQuery(new ComponentType[] { typeof(CFixedDishChoice), typeof(CDishChoice), typeof(CHome) });

            DishUpgrades = GetEntityQuery(new ComponentType[] { typeof(CDishUpgrade) });

            RequireSingletonForUpdate<SSelectedFood>();
            RequireSingletonForUpdate<SRefreshFood>();
            RequireForUpdate(Pedestals);
        }

        protected override void OnUpdate()
        {
            // Get request data and then clear it
            var request = GetSingleton<SSelectedFood>();
            Clear<SRefreshFood>();

            var requestedID = request.ID;
            var ignoreFilter = request.IgnoreFilter;
            var onlyModded = request.OnlyModded;

            LogDebug($"[MAPS] New food maps requested. ID: {requestedID}; Ignore Filter: {ignoreFilter}; Modded Only: {onlyModded}");

            // Destroy non-fixed choices
            EntityManager.DestroyEntity(Maps);

            // Put fixed choices back home
            var fixedMapsArray = FixedMaps.ToEntityArray(Allocator.Temp);
            foreach (var map in fixedMapsArray)
            {
                var mapHome = EntityManager.GetComponentData<CHome>(map).Holder;

                EntityManager.SetComponentData<CHeldBy>(map, mapHome);
                EntityManager.SetComponentData<CItemHolder>(mapHome, map);
            }
            fixedMapsArray.Dispose();

            // Gather food choices
            var dishUpgrades = DishUpgrades.ToComponentDataArray<CDishUpgrade>(Allocator.Temp);
            List<int> allFoods = KitchenData.RandomExtensions.Shuffle((from x in dishUpgrades select x.DishID).ToList());
            if (!ignoreFilter && !onlyModded)
                allFoods.RemoveAll(GDOContainer.ModdedDishes.ContainsKey);
            if (onlyModded)
                allFoods.RemoveAll(f => !GDOContainer.ModdedDishes.ContainsKey(f));
            List<int> allModded = KitchenData.RandomExtensions.Shuffle(GDOContainer.ModdedDishes.Keys.ToList());
            dishUpgrades.Dispose();

            // Place maps
            var pedestals = Pedestals.ToEntityArray(Allocator.Temp);
            int counter = 0;
            int moddedCounter = 0;
            foreach (var pedestal in pedestals)
            {
                var isModded = EntityManager.GetComponentData<CFoodPedestal>(pedestal).Modded;

                // Check if no more available choices
                if (!ignoreFilter && moddedCounter >= allModded.Count ||
                    (!isModded || ignoreFilter) && counter >= allFoods.Count)
                    continue;

                // Create choice
                var choice = EntityManager.CreateEntity();
                EntityManager.AddComponentData(choice, new CCreateItem { ID = AssetReference.DishPaper });
                EntityManager.AddComponentData(choice, new CDishChoice 
                {
                    Dish = requestedID != 0 ? requestedID : 
                    (!isModded || ignoreFilter ? allFoods[counter] : allModded[moddedCounter])
                });

                // Tick counter
                if (requestedID == 0)
                {
                    if (isModded && !ignoreFilter)
                        moddedCounter++;
                    else counter++;
                }

                // Parent choice
                EntityManager.AddComponentData<CHeldBy>(choice, pedestal);
                EntityManager.AddComponentData<CHome>(choice, pedestal);
                EntityManager.SetComponentData<CItemHolder>(pedestal, choice);
            }
            pedestals.Dispose();
        }

        public struct SSelectedFood : IModComponent
        {
            public int ID;
            public bool IgnoreFilter;
            public bool OnlyModded;
        }

        public struct SRefreshFood : IModComponent { }
    }
}
