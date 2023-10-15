using Kitchen;

namespace KitchenHQ.Franchise.Systems
{
    public class ActivateTelevision : ItemInteractionSystem
    {
        protected override bool RequirePress => true;

        private CPlayer player;
        protected override bool IsPossible(ref InteractionData data) =>
            Has<STelevision>(data.Target) && !Has<STelevision.STriggerInteraction>(data.Target) &&
            Require(out STapePlayer sPlayer) && !sPlayer.Tape.Equals(default) && 
            Require(data.Interactor, out player);

        protected override void Perform(ref InteractionData data)
        {
            LogDebug("[APPLIANCE] [TV] Interacted with television");
            Set(data.Target, new STelevision.STriggerInteraction { PlayerID = player.ID });
        }
    }
}
