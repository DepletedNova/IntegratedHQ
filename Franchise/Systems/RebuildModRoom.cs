using Kitchen;
using KitchenData;
using KitchenHQ.API;
using KitchenHQ.Utility;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    [UpdateAfter(typeof(ModFranchiseGroup))]
    public class RebuildModRoom : FranchiseSystem
    {
        private EntityQuery Clearables;

        protected override void Initialise()
        {
            base.Initialise();

            Clearables = GetEntityQuery(new ComponentType[] { typeof(CModRoomClears) });

            RequireSingletonForUpdate<SModRoom>();
            RequireSingletonForUpdate<SRebuildModRoom>();
        }

        protected override void OnUpdate()
        {
            Clear<SRebuildModRoom>();

            var index = GetSingleton<SModRoom>().Index;
            if (index >= ModRoom.Rooms.Count)
            {
                LogError("[REBUILD] [MOD ROOM] Index greater than Room Count");
                SetSingleton(new SModRoom());
                return;
            }

            LogDebug($"[REBUILD] [MOD ROOM] Removing old appliances");
            EntityManager.DestroyEntity(Clearables);

            var room = ModRoom.Rooms[index];
            room.BuildRoom(EntityManager, this);

            LogDebug($"[REBUILD] [MOD ROOM] Rebuilt under index: {index}");
        }

        public void SetPublicOccupant(Vector3 position, Entity e, OccupancyLayer layer = OccupancyLayer.Default) =>
            SetOccupant(position, e, layer);

        public Entity GetPublicOccupant(Vector3 position, OccupancyLayer layer = OccupancyLayer.Default) =>
            GetOccupant(position, layer);
    }
}
