using KitchenHQ.API;
using KitchenHQ.Utility;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    [UpdateInGroup(typeof(ModFranchiseGroup))]
    public class CreateModRoom : FranchiseBuildSystem
    {
        protected override void Build()
        {
            LogDebug("[BUILD] Initialising Mod Room build");

            Set<SModRoom>();
            Set<SRebuildModRoom>();

            if (ModRoom.Rooms.Count > 1)
                Create(FranchiseReferences.RoomSwapper, ModFranchise.ModRoomAnchor + new Vector3(-2.9f, 0, -3), Vector3.forward);
        }
    }
}
