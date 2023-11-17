using Kitchen;
using KitchenData;
using KitchenHQ.Utility;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    [UpdateInGroup(typeof(SeasonalModFranchiseGroup))]
    public class CreateHalloweenDecorations : SeasonalFranchiseBuildSystem
    {
        protected override ModFranchise.SeasonalLobby Seasonal => ModFranchise.SeasonalLobby.Halloween;

        protected override void AddDecorations()
        {
            // Garland
            Create(GetCastedGDO<Appliance, HalloweenGarland>(), new Vector3(2.5f, 0f, -2.5f), Vector3.forward);

            // Cobwebs
            var cobweb = GetCastedGDO<Appliance, Cobweb>();
            Create(cobweb, new Vector3(1f, 0f, 3f), Vector3.forward);
            Create(cobweb, ModdedLobbyPositionAnchors.Workshop + new Vector3(0f, 0f, 11f), Vector3.left);
            Create(cobweb, ModdedLobbyPositionAnchors.Garage + new Vector3(5f, 0f, 5f), Vector3.forward);

            // Pumpkins
            var pumpkin1 = GetCastedGDO<Appliance, Pumpkin1>();
            var pumpkin2 = GetCastedGDO<Appliance, Pumpkin2>();
            Create(pumpkin1, ModdedLobbyPositionAnchors.Office + new Vector3(6f, 0f, 0f), Vector3.forward);
            Create(pumpkin2, ModdedLobbyPositionAnchors.Office + new Vector3(-6f, 0f, 0f), Vector3.forward);
            Create(pumpkin2, ModdedLobbyPositionAnchors.Workshop + new Vector3(5f, 0f, 8f), Vector3.right);

            // todo: more decorations; I want plenty of decently high quality decorations to make the HQ just a bit more lively and pretty. This should go for ALL seasonal HQ updates par MAYBE April Fools
        }
    }
}
