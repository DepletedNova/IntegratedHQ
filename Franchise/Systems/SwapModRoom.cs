using Kitchen;
using KitchenHQ.API;
using System.Reflection;
using Unity.Collections;
using Unity.Entities;

namespace KitchenHQ.Franchise
{
    public class SwapModRoom : FranchiseSystem
    {
        private EntityQuery Swappers;
        protected override void Initialise()
        {
            base.Initialise();
            Swappers = GetEntityQuery(new ComponentType[] { typeof(CModRoomSwapper), typeof(CTakesDuration) });
            RequireForUpdate(Swappers);
            RequireSingletonForUpdate<SModRoom>();
        }

        protected override void OnUpdate()
        {
            var durations = Swappers.ToComponentDataArray<CTakesDuration>(Allocator.Temp);
            for (int i = 0; i < durations.Length; i++)
            {
                var duration = durations[i];
                if (duration.Remaining <= 0 && duration.Active)
                {
                    var modRoom = GetSingleton<SModRoom>();
                    modRoom.Index = (modRoom.Index + 1) % ModRoom.Rooms.Count;

                    LogDebug("[CYCLE] Cycling Mod Room to index: " + modRoom.Index.ToString());

                    Set(modRoom);
                    Set<SRebuildModRoom>();
                }
            }
        }
    }
}
