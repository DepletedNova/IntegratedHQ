using KitchenData;
using KitchenHQ.Utility;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class ModDebugMarker : StaticFranchiseAppliance
    {
        private static readonly int PlayerColour = Shader.PropertyToID("_PlayerColour");

        public override string UniqueNameID => "Mod Marker";
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, CreateApplianceInfo("Mod Debug Marker", "", new(), new()))
        };

        public override void SetupPrefab(GameObject prefab)
        {
            var ping = prefab.ApplyMaterialToChild("ping", "Ping");
            ping.GetComponent<MeshRenderer>().material.SetColor(PlayerColour, Color.blue);
        }
    }
}
