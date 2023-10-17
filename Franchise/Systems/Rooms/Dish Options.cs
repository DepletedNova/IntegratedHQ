using KitchenHQ.Utility;
using Kitchen;
using KitchenData;
using KitchenMods;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    [UpdateInGroup(typeof(ModFranchiseGroup))]
    public class CreateModdedDishOptions : FranchiseBuildSystem<CreateDishOptions>, IModSystem
    {

        private EntityQuery ExtraDishes;

        protected override void OnInitialise()
        {
            ExtraDishes = GetEntityQuery(new ComponentType[] { typeof(CUpgradeExtraDish) });
        }

        protected override void Build()
        {
            var office = LobbyPositionAnchors.Office;

            Set<HandleFoodRequests.SSelectedFood>();
            Set<HandleFoodRequests.SRefreshFood>();

            // Always available -- replace with a reroll
            /*if (AssetReference.AlwaysAvailableDish != 0 && Data.TryGet(AssetReference.AlwaysAvailableDish, out Dish alwaysDish, true))
                CreateFixedFoodSource(office + new Vector3(0f, 0f, -3f), alwaysDish);*/

            // Slot count
            int slots = ExtraDishes.CalculateEntityCount();
            if (Require(out SPlayerLevel sLevel))
            {
                LogDebug("[BUILD] [UNLOCK] Adding extra dish slots...");
                if (sLevel.Level >= 15)
                {
                    slots += 3;
                    LogDebug("[BUILD] [UNLOCK] Level 15: +3");
                }
            }

            // Base Dishes
            List<Vector3> baseDishLocations = new()
            {
                new Vector3(2f, 0f, -3f),
                new Vector3(2f, 0f, -4f),
                new Vector3(3f, 0f, -3f),
                new Vector3(3f, 0f, -4f),
                new Vector3(4f, 0f, -3f),
                new Vector3(4f, 0f, -4f),

                new Vector3(2f, 0f, -6f),
                new Vector3(3f, 0f, -6f),
                new Vector3(4f, 0f, -6f),
            };

            int totalSlots = Mathf.Min(baseDishLocations.Count, 1 + slots);
            LogDebug($"[BUILD] Dish slots ({totalSlots})");
            for (int i = 0; i < totalSlots; i++)
                CreatePedestal(office + baseDishLocations[i], false);

            // Modded Dishes
            if (GDOContainer.ModdedDishes.Count <= 0)
            {
                LogDebug("[BUILD] [SKIP] Modded dish slots (no modded dishes)");
                return;
            }

            LogDebug("[BUILD] Modded dish slots");

            var moddedDishLocations = Kitchen.RandomExtensions.Shuffle(new List<Vector3>()
            {
                new Vector3(6f, 0f, -3f),
                new Vector3(6f, 0f, -4f),
                new Vector3(6f, 0f, -5f),
            });

            Create(FranchiseReferences.ModdedFoodsText, office + new Vector3(5.5f, 0f, -4f), Vector3.right);
            for (int i = 0; i < moddedDishLocations.Count; i++)
                CreatePedestal(office + moddedDishLocations[i], true);
        }

        private Entity CreatePedestal(Vector3 location, bool isModded)
        {
            var appliance = EntityManager.CreateEntity(new ComponentType[] { typeof(CItemHolder) });
            EntityManager.AddComponentData(appliance, new CCreateAppliance { ID = AssetReference.DishPedestal });
            EntityManager.AddComponentData(appliance, new CPosition(location));
            EntityManager.AddComponentData(appliance, new CFoodPedestal { Modded = isModded });
            return appliance;
        }

        private void CreateFixedFoodSource(Vector3 location, Dish dish)
        {
            var appliance = CreatePedestal(location, false);
            EntityManager.AddComponent<CFixedFoodPedestal>(appliance);

            var bp = EntityManager.CreateEntity();
            EntityManager.AddComponentData(bp, new CCreateItem { ID = AssetReference.DishPaper });
            EntityManager.AddComponentData(bp, new CDishChoice { Dish = dish.ID });

            EntityManager.AddComponentData<CHeldBy>(bp, appliance);
            EntityManager.AddComponentData<CHome>(bp, appliance);
            EntityManager.SetComponentData<CItemHolder>(appliance, bp);
        }
    }
}
