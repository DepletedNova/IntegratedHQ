using KitchenHQ.Utility;
using Kitchen;
using KitchenData;
using KitchenMods;
using UnityEngine;
using Unity.Entities;

namespace KitchenHQ.Franchise
{
    [UpdateInGroup(typeof(ModFranchiseComponentGroup))]
    public class CreateModdedTutorialTrigger : FranchiseBuildSystem<CreateTutorialTrigger>, IModSystem
    {
        protected override void Build()
        {
            LogDebug("[BUILD] Tutorial trigger");
            Create(Data.Get<Appliance>(AssetReference.TutorialTrigger), new Vector3(7f, 0f, 8f), Vector3.left);
        }
    }
}
