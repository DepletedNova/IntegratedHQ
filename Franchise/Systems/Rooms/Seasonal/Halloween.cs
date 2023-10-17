using KitchenHQ.Utility;
using Unity.Entities;

namespace KitchenHQ.Franchise
{
    [UpdateInGroup(typeof(SeasonalModFranchiseGroup))]
    public class CreateHalloweenDecorations : SeasonalFranchiseBuildSystem
    {
        protected override ModFranchise.SeasonalLobby Seasonal => ModFranchise.SeasonalLobby.Halloween;

        protected override void AddDecorations()
        {

        }
    }
}
