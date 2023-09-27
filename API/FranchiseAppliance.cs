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
        public static void Register(FranchiseAppliance LegacyFranchise, FranchiseAppliance ModFranchise, Action<Entity, EntityManager> Action = null)
        {
            SetAppliance(LegacyFranchise, Action, ref BaseAppliances);
            SetAppliance(ModFranchise, Action, ref ModAppliances);
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
        public static void Register<T>(Vector3 LegacyPosition, Vector3 LegacyRotation, Vector3 ModPosition, Vector3 ModRotation, Action<Entity, EntityManager> Action = null) where T : CustomAppliance =>
            Register(Create<T>(LegacyPosition, LegacyRotation), Create<T>(ModPosition, ModRotation), Action);

        /// <param name="ID">The ID of the appliance that will be placed</param>
        /// <param name="Position">The position of the appliance (Highly suggest using <c>Kitchen.LobbyPositionAnchors</c> and offsetting those)</param>
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
        /// <param name="Position">The position of the appliance (Highly suggest using <c>Kitchen.LobbyPositionAnchors</c> and offsetting those)</param>
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
        internal static Dictionary<Vector3, List<(FranchiseAppliance Appliance, Action<Entity, EntityManager> Action)>> BaseAppliances = new();
        internal static Dictionary<Vector3, List<(FranchiseAppliance Appliance, Action<Entity, EntityManager> Action)>> ModAppliances = new();

        // Utility
        private static void SetAppliance(FranchiseAppliance Appliance, Action<Entity, EntityManager> Action, ref Dictionary<Vector3, List<(FranchiseAppliance, Action<Entity, EntityManager>)>> Appliances)
        {
            var pos = Appliance.Position.Rounded();
            if (!Appliances.ContainsKey(pos))
                Appliances.Add(pos, new());
            Appliances[pos].Add((Appliance, Action));
        }
        #endregion
    }
}
