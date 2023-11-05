using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace KitchenHQ.API
{
    public class FranchiseAppliance
    {
        /// <summary>
        /// Registers an appliance to be automatically placed in the HQ (allowing for both HQ types as well as
        /// duplicate appliance positions). Should be used in <c>BaseMod.OnInitialise()</c>
        /// </summary>
        /// <param name="LegacyFranchise">The <c>FranchiseAppliance</c> for the Legacy HQ type</param>
        /// <param name="ModFranchise">The <c>FranchiseAppliance</c> for the Modded HQ type</param>
        /// <param name="Action">An optional action to perform after the Appliance Entity is placed</param>
        public static void Register(FranchiseAppliance LegacyFranchise, FranchiseAppliance ModFranchise, Action<Entity, EntityCommandBuffer> Action = null)
        {
            SetAppliance(LegacyFranchise, Action, ref BaseAppliances, ref BaseMax);
            SetAppliance(ModFranchise, Action, ref ModAppliances, ref ModMax);
        }

        /// <summary>
        /// Registers an appliance to be automatically placed in the HQ (allowing for both HQ types as well as
        /// duplicate appliance positions). Should be used in <c>BaseMod.OnInitialise()</c>
        /// </summary>
        /// <typeparam name="T">The CustomAppliance that is to be placed</typeparam>
        /// <param name="LegacyPosition">The position of the appliance for the Legacy HQ type</param>
        /// <param name="LegacyRotation">The direction the appliance will face the Legacy HQ type</param>
        /// <param name="ModPosition">The position of the appliance for the Modded HQ type</param>
        /// <param name="ModRotation">The direction the appliance will face for the Modded HQ type</param>
        /// <param name="Action">An optional action to perform after the Appliance Entity is placed</param>
        public static void Register<T>(Vector3 LegacyPosition, Vector3 LegacyRotation, Vector3 ModPosition, Vector3 ModRotation, Action<Entity, EntityCommandBuffer> Action = null) where T : CustomAppliance =>
            Register(Create<T>(LegacyPosition, LegacyRotation), Create<T>(ModPosition, ModRotation), Action);

        /// <summary>
        /// Registers an appliance to be automatically placed in the HQ (allowing for both HQ types as well as
        /// duplicate appliance positions). Should be used in <c>BaseMod.OnInitialise()</c>
        /// </summary>
        /// <param name="id">The ID of the appliance that will be placed</param>
        /// <param name="LegacyPosition">The position of the appliance for the Legacy HQ type</param>
        /// <param name="LegacyRotation">The direction the appliance will face the Legacy HQ type</param>
        /// <param name="ModPosition">The position of the appliance for the Modded HQ type</param>
        /// <param name="ModRotation">The direction the appliance will face for the Modded HQ type</param>
        /// <param name="Action">An optional action to perform after the Appliance Entity is placed</param>
        public static void Register(int id, Vector3 LegacyPosition, Vector3 LegacyRotation, Vector3 ModPosition, Vector3 ModRotation, Action<Entity, EntityCommandBuffer> Action = null) =>
            Register(Create(id, LegacyPosition, LegacyRotation), Create(id, ModPosition, ModRotation), Action);

        /// <param name="ID">The ID of the appliance that will be placed</param>
        /// <param name="Position">The position of the appliance (Highly suggest using <c>Kitchen.LobbyPositionAnchors</c> or <c>KitchenHQ.Franchise.ModdedLobbyPositionAnchors</c> and offsetting those)</param>
        /// <param name="Rotation">The direction the appliance will face</param>
        /// <returns></returns>
        public static FranchiseAppliance Create(int ID, Vector3 Position, Vector3 Rotation)
        {
            var fa = new FranchiseAppliance();
            fa.ID = ID;
            fa.Position = Position;
            fa.Rotation = Rotation;
            return fa;
        }

        /// <typeparam name="T">The CustomAppliance that is to be placed</typeparam>
        /// <param name="Position">The position of the appliance (Highly suggest using <c>Kitchen.LobbyPositionAnchors</c> or <c>KitchenHQ.Franchise.ModdedLobbyPositionAnchors</c> and offsetting those)</param>
        /// <param name="Rotation">The direction the appliance will face</param>
        /// <returns></returns>
        public static FranchiseAppliance Create<T>(Vector3 Position, Vector3 Rotation) where T : CustomAppliance =>
            Create(GetCustomGameDataObject<T>().ID, Position, Rotation);

        // Non API

        #region Non-API
        // Appliance
        private FranchiseAppliance() { }

        public int ID;
        public Vector3 Position;
        public Vector3 Rotation;

        // Appliance Dictionaries
        internal static Dictionary<Vector3, List<(FranchiseAppliance Appliance, Action<Entity, EntityCommandBuffer> Action)>> BaseAppliances = new();
        internal static int BaseMax = 1;
        internal static Dictionary<Vector3, List<(FranchiseAppliance Appliance, Action<Entity, EntityCommandBuffer> Action)>> ModAppliances = new();
        internal static int ModMax = 1;

        // Utility
        private static void SetAppliance(FranchiseAppliance Appliance, Action<Entity, EntityCommandBuffer> Action, ref Dictionary<Vector3, List<(FranchiseAppliance, Action<Entity, EntityCommandBuffer>)>> Appliances, 
            ref int Max)
        {
            var pos = Appliance.Position.Rounded();
            if (!Appliances.ContainsKey(pos))
                Appliances.Add(pos, new());
            Appliances[pos].Add((Appliance, Action));
            Max = Mathf.Max(Appliances[pos].Count, Max);
        }
        #endregion
    }
}
