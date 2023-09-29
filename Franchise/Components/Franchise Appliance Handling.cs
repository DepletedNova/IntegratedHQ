using KitchenData;
using KitchenMods;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    // Mod Room
    public struct CModRoomClears : IAttachableProperty, IModComponent { }

    public struct SModRoom : IModComponent
    {
        public int Index;
    }

    public struct SRebuildModRoom : IModComponent { }

    // Appliances
    public struct CFranchiseAppliance : IModComponent
    {
        public int Total;
        public Entity PairedAppliance;
    }

    public struct SFranchiseApplianceIndex : IModComponent
    {
        public int Index;
    }

    public struct SRebuildFranchiseAppliances : IModComponent { }
}