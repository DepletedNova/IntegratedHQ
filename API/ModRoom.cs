using Kitchen;
using Kitchen.Layouts;
using KitchenData;
using KitchenHQ.Franchise;
using KitchenLib.Customs;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace KitchenHQ.API
{
    public class ModRoom
    {
        // Register/Create Room
        public static ModRoom Register(Action<ModRoom, EntityCommandBuffer> Build)
        {
            var room = new ModRoom(Build);
            Rooms.Add(room);
            return room;
        }

        // Materials (unimplemented)
        public Material FloorMaterial;
        public Material WallMaterial;

        // Creates an Appliance within an ECB
        public Entity Create(int id, Vector3 location, Vector3 facing)
        {
            var pos = ModFranchise.ModRoomAnchor + new Vector3(Mathf.Clamp(location.x, -2f, 2f), 0f, Mathf.Clamp(location.z, -3f, 2f));
            var roundedPos = pos.Rounded();

            Entity entity = ECB.CreateEntity();
            ECB.AddComponent(entity, new CCreateAppliance { ID = id });
            ECB.AddComponent(entity, new CPosition(pos, quaternion.LookRotation(facing, new float3(0f, 1f, 0f))));
            ECB.AddComponent<CModRoomClears>(entity);

            System.SetPublicOccupant(roundedPos, entity);

            return entity;
        }

        public Entity Create<T>(Vector3 location, Vector3 facing) where T : CustomAppliance =>
            Create(GetCustomGameDataObject<T>().ID, location, facing);

        public Entity CreateProxy(Vector3 location, Entity target)
        {
            var proxy = Create(AssetReference.InteractionProxy, location, Vector3.forward);
            ECB.AddComponent(proxy, new CInteractionProxy { IsActive = true, Target = target });
            return proxy;
        }

        public Entity CreateItem(int id, Entity parent)
        {
            var entity = ECB.CreateEntity();
            ECB.AddComponent(entity, new CCreateItem { Holder = parent, ID = id });
            ECB.AddComponent<CModRoomClears>(entity);

            return entity;
        }

        public Entity CreateItem<T>(Entity parent) where T : CustomItem =>
            CreateItem(GetCustomGameDataObject<T>().ID, parent);

        // Non API

        #region Non-API 
        private ModRoom(Action<ModRoom, EntityCommandBuffer> Build)
        {
            this.Build = Build;
        }

        private Action<ModRoom, EntityCommandBuffer> Build;
        private EntityCommandBuffer ECB;
        private RebuildModRoom System;

        internal void BuildRoom(EntityManager EM, RebuildModRoom System)
        {
            if (Build == null)
                return;

            LogDebug("[REBUILD] [MOD ROOM] Rebuilding");

            this.System = System;
            ECB = new(Allocator.Temp);
            Build.Invoke(this, ECB);
            ECB.Playback(EM);
            ECB.Dispose();
        }

        internal static List<ModRoom> Rooms = new()
        {
            new ModRoom((room, ECB) =>
            {
                LogDebug("[REBUILD] [MOD ROOM] Building default room");

                // Decorations
                room.Create<ModRoomTable>(new(1.5f, 0f, 2f), Vector3.forward);

                // Event board
                var eventBoard = room.Create<EventBoard>(new(-1.5f, 0f, -3f), Vector3.forward);
                room.CreateProxy(new(-0.5f, 0f, -3f), eventBoard);

                // TV
                var TV = room.Create<ModTV>(new(-1f, 0f, 2f), Vector3.forward);
                room.CreateProxy(new(-2f, 0f, 2f), TV);
                room.CreateProxy(new(0f, 0f, 2f), TV);

                var tape = room.CreateItem<VHSTape>(TV);

                // VHS Writer
                room.Create<VHSWriter>(new(2f, 0f, 2f), Vector3.forward);
            })
        };
        #endregion
    }
}
