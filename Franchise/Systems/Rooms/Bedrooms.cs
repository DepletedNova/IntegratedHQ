using KitchenHQ.Utility;
using Kitchen;
using KitchenData;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    [UpdateInGroup(typeof(ModFranchiseComponentGroup))]
    public class CreateModdedBedrooms : FranchiseBuildSystem<CreateBedrooms>, IModSystem
    {
        private EntityQuery Players;

        protected override void OnInitialise()
        {
            Players = GetEntityQuery(new ComponentType[] { typeof(CPlayer), typeof(CPosition) });
        }

        protected override void Build()
        {
            var playerEntities = Players.ToEntityArray(Allocator.Temp);
            var bedrooms = LobbyPositionAnchors.Bedrooms;

            for (int i = 0; i < bedrooms.Count; i++)
            {
                int inversion = i % 2 == 0 ? 1 : -1;

                LogDebug($"[BUILD] [BEDROOM] (Index: {i}; Inverted: {inversion == -1})");

                var bed = CreateAssigned(i, AssetReference.Bed, bedrooms[i] + new Vector3(inversion * 1.5f, 0f, 1f), Vector3.forward);
                var proxy = CreateAssigned(i, AssetReference.InteractionProxy, bedrooms[i] + new Vector3(inversion * 1.5f, 0f, 0f), Vector3.forward);

                EntityManager.AddComponentData(proxy, new CInteractionProxy
                {
                    Target = bed,
                    IsActive = true
                });
                
                CreateAssigned(i, AssetReference.OutfitStation, bedrooms[i] + new Vector3(inversion * -1.5f, 0f, 1f), Vector3.forward);
                CreateAssigned(i, AssetReference.OccupationIndicator, bedrooms[i] + Vector3.right * 2.5f, Vector3.forward);

                PlaceSpawnMarker(i, bedrooms[i]);

                if (playerEntities.Length - 1 < i)
                    continue;

                EntityManager.SetComponentData(playerEntities[i], new CPosition(bedrooms[i]));
            }

            playerEntities.Dispose();
        }

        protected void PlaceSpawnMarker(int index, Vector3 location)
        {
            EntityManager.AddComponentData(EntityManager.CreateEntity(), new CPlayerSpawnLocation
            {
                Index = index,
                Location = location
            });
        }

        protected Entity CreateAssigned(int bedroom, int GDO, Vector3 location, Vector3 facing)
        {
            var appliance = Create(GDO, location, facing);
            EntityManager.AddComponentData(appliance, new CBedroomPart { Room = bedroom });
            EntityManager.AddComponentData(appliance, new COwnedByPlayer { Player = default(Entity) });
            EntityManager.AddComponentData(appliance, default(CColourByOwner));
            return appliance;
        }
    }
}
