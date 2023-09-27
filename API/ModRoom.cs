﻿using Kitchen;
using KitchenData;
using KitchenHQ.Franchise;
using KitchenHQ.Franchise.Components;
using KitchenHQ.Franchise.Systems;
using KitchenLib.Customs;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace KitchenHQ.API
{
    public class ModRoom
    {
        // Register Room
        public static ModRoom Register(Action<ModRoom, EntityCommandBuffer> Build)
        {
            var room = new ModRoom(Build);
            Rooms.Add(room);
            return room;
        }

        // Materials
        public Material FloorMaterial;
        public Material WallMaterial;

        // Creates an Appliance
        public Entity Create(int id, Vector3 location, Vector3 facing)
        {
            var pos = ModFranchise.ModRoomAnchor + new Vector3(Mathf.Clamp(location.x, -2f, 2f), 0f, Mathf.Clamp(location.z, -3f, 2f));
            var roundedPos = pos.Rounded();

            if (System.GetPublicOccupant(roundedPos) != Entity.Null)
                return Entity.Null;

            Entity entity = ECB.CreateEntity();
            ECB.AddComponent(entity, new CCreateAppliance { ID = id });
            ECB.AddComponent(entity, new CPosition(pos, quaternion.LookRotation(facing, new float3(0f, 1f, 0f))));
            ECB.AddComponent<CModRoomAppliance>(entity);

            System.SetPublicOccupant(roundedPos, entity);

            return entity;
        }

        public Entity Create<T>(Vector3 location, Vector3 facing) where T : CustomAppliance =>
            Create(GetCustomGameDataObject<T>().ID, location, facing);

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

            LogDebug("[BUILD] [MOD ROOM] Rebuilding");

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
                LogDebug("[BUILD] [MOD ROOM] Building default room");
                room.Create<SubscribedModsView>(Vector3.zero, Vector3.forward);
            })
        };
        #endregion
    }
}