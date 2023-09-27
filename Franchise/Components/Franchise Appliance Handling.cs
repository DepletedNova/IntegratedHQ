using KitchenData;
using KitchenMods;

namespace KitchenHQ.Franchise.Components
{
    public struct CModRoomAppliance : IAttachableProperty, IModComponent { }

    public struct SModRoom : IModComponent
    {
        public int Index;
    }

    public struct SRebuildModRoom : IModComponent { }
}