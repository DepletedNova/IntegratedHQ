using Kitchen;
using KitchenData;
using KitchenHQ.API;
using KitchenLib.References;
using UnityEngine;

namespace KitchenHQ
{
    internal static class ExampleAPI
    {

        // API Examples (BaseMod.OnInitialise() is recommended)
        public static void ShowExamples()
        {
            // Example appliances
            // Uses absolute positions. Highly recommend the use of Kitchen.LobbyPositionAnchors
            // Accounts for both the new and legacy HQs
            FranchiseAppliance.Register(AssetReference.DangerHob, new(-1, 0, -6), Vector3.forward, new(4, 0, 0), Vector3.forward);

            // EntityCommandBuffer available for use
            FranchiseAppliance.Register(AssetReference.Counter, new(-1, 0, -6), Vector3.forward, new(4, 0, 0), Vector3.forward,
                (Entity, ECB) =>
                {
                    ECB.AddComponent<CIsOnFire>(Entity);
                });

            // Example room
            ModRoom.Register((Room, ECB) =>
            {
                // Room is a ModRoom and it contains various helper functions
                // ECB is an EntityCommandBuffer used for ECS

                // Positions are relative to the middle of the mod room
                Room.Create(AssetReference.Counter, new(-2, 0, 2), Vector3.forward);
                Room.Create(AssetReference.DangerHob, new(-1, 0, 2), Vector3.forward);

                var specialCounter = Room.Create(AssetReference.Counter, new(0, 0, 2), Vector3.forward);
                Room.CreateProxy(new(1, 0, 2), specialCounter); // Allows the easy creation of interaction proxies
                Room.CreateItem(ItemReferences.Apple, specialCounter); // Allows the creation of items

                // EntityCommandBuffer available for use
                var flamingCounter = Room.Create(AssetReference.Counter, new(2, 0, 2), Vector3.forward);
                ECB.AddComponent<CIsOnFire>(flamingCounter);

                // Any entities created for the mod room through the ECB should have KitchenHQ.Franchise.CModRoomClears
            });
        }
    }
}
