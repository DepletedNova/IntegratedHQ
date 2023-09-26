using KitchenHQ.Utility;
using KitchenData;
using KitchenMods;

namespace KitchenHQ.Franchise
{
    public struct CModdedUpgradeView : IApplianceProperty, IModComponent
    {
        public int Page;

        public bool IsDish;
        public bool IsSetting { get => !IsDish; set => IsDish = !value; }
    }

    public struct CSubscribedModsView : IApplianceProperty, IModComponent
    {
        public int ModCount;
    }

    public struct CCreatedModsView : IApplianceProperty, IModComponent
    {
        public ulong UserID;
    }
}
