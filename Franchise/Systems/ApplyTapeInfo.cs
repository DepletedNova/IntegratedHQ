using Kitchen;
using Unity.Entities;

namespace KitchenHQ.Franchise
{
    [UpdateBefore(typeof(ViewSystemsGroup))]
    public class ApplyTapeInfo : FranchiseSystem
    {
        protected override void Initialise()
        {
            base.Initialise();
            RequireSingletonForUpdate<STape>();
            RequireSingletonForUpdate<STapePlayer>();
        }

        protected override void OnUpdate()
        {
            var playerEntity = GetSingletonEntity<STapePlayer>();
            var sPlayer = GetSingleton<STapePlayer>();
            if (!Require(playerEntity, out CItemHolder holder) || holder.HeldItem == Entity.Null || !Require(holder.HeldItem, out STape sTape))
            {
                sPlayer.Tape = default;
                sPlayer.Holding = false;
            } else
            {
                sPlayer.Tape = sTape;
                sPlayer.Holding = true;
            }

            Set(playerEntity, sPlayer);
        }

    }
}
