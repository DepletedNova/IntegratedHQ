using KitchenHQ.Utility;
using Unity.Entities;

namespace KitchenHQ.Franchise
{
    [UpdateInGroup(typeof(ModFranchiseComponentGroup))]
    public class CreateModRoom : FranchiseBuildSystem
    {
        protected override void Build()
        {
            LogDebug("[BUILD] Initialising Mod Room build");

            EntityManager.CreateEntity(typeof(SModRoom));
            EntityManager.CreateEntity(typeof(SRebuildModRoom));

            // todo: build swapper

        }
    }
}
