using KitchenData;
using KitchenLib.Customs;
using System.Collections.Generic;
using System.Linq;

namespace KitchenHQ.Utility
{
    public static class GDOContainer
    {
        public static Dictionary<int, Dish> ModdedDishes = new();
        public static Dictionary<int, Dish> AllDishes = new();
        public static Dictionary<int, RestaurantSetting> ModdedSettings = new();

        internal static void SetupDishes(GameData gameData)
        {
            foreach (var dish in gameData.Get<Dish>().Where(dish => dish.IsUnlockable && dish.Type == DishType.Base))
            {
                if (AllDishes.ContainsKey(dish.ID))
                    break;

                AllDishes.Add(dish.ID, dish);
            }

            foreach (var customGDOPair in CustomGDO.GDOs)
            {
                if (ModdedDishes.ContainsKey(customGDOPair.Key) || ModdedSettings.ContainsKey(customGDOPair.Key))
                    break;

                if (customGDOPair.Value is CustomDish dish)
                {
                    if (!dish.IsAvailableAsLobbyOption)
                        continue;

                    ModdedDishes.Add(dish.ID, dish.GameDataObject);
                } else if (customGDOPair.Value is CustomRestaurantSetting setting)
                {
                    ModdedSettings.Add(setting.ID, setting.GameDataObject);
                }
            }
        }
    }
}
