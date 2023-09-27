using Kitchen;
using KitchenData;
using KitchenHQ.API;
using KitchenHQ.Franchise.Components;
using KitchenHQ.Utility;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQ.Franchise.Systems
{
    [UpdateAfter(typeof(ModFranchiseComponentGroup))]
    public class RebuildModRoom : FranchiseSystem
    {
        protected override void Initialise()
        {
            base.Initialise();
            RequireSingletonForUpdate<SModRoom>();
            RequireSingletonForUpdate<SRebuildModRoom>();
        }

        protected override void OnUpdate()
        {
            Clear<SRebuildModRoom>();

            var index = GetSingleton<SModRoom>().Index;
            if (index >= ModRoom.Rooms.Count)
            {
                LogError("[ModRoom] Index greater than Room Count");
                SetSingleton(new SModRoom());
                return;
            }

            var room = ModRoom.Rooms[index];
            room.BuildRoom(EntityManager, this);
        }

        public void SetPublicOccupant(Vector3 position, Entity e, OccupancyLayer layer = OccupancyLayer.Default) =>
            SetOccupant(position, e, layer);

        public Entity GetPublicOccupant(Vector3 position, OccupancyLayer layer = OccupancyLayer.Default) =>
            GetOccupant(position, layer);
    }
}
