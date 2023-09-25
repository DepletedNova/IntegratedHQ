using KitchenData;
using KitchenMods;

namespace KitchenHQ.Franchise
{
    public struct CFixedDishChoice : IAttachableProperty, IModComponent { }

    public struct CFixedFoodPedestal : IAttachableProperty, IModComponent { }
    public struct CFoodPedestal : IAttachableProperty, IModComponent
    {
        public bool Modded;
    }
}
