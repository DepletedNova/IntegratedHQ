using Unity.Entities;

namespace KitchenHQ.Utility
{
    public class ModFranchiseGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(ModFranchiseGroup), OrderFirst = true)]
    public class SeasonalModFranchiseGroup : ComponentSystemGroup { }
}
