using Kitchen;
using KitchenMods;
using System.Linq;
using Unity.Collections;
using Unity.Entities;

namespace KitchenHQ.Franchise
{
    public class InteractSelectFood : ItemInteractionSystem, IModSystem
    {
        private EntityQuery DishUpgrades;

        protected override void Initialise()
        {
            base.Initialise();
            RequireSingletonForUpdate<SFranchiseMarker>();
            RequireSingletonForUpdate<HandleFoodRequests.SSelectedFood>();

            DishUpgrades = GetEntityQuery(new ComponentType[] { typeof(CDishUpgrade) });
        }

        protected override bool AllowActOrGrab => true;

        protected override bool IsPossible(ref InteractionData data)
        {
            if (Has<CFoodSelector>(data.Target) && !HasSingleton<HandleFoodRequests.SRefreshFood>())
                return true;

            return false;
        }

        protected override void Perform(ref InteractionData data)
        {
            var dishes = DishUpgrades.ToComponentDataArray<CDishUpgrade>(Allocator.Temp);
            var choices = (from x in dishes select x.DishID).ToList();
            dishes.Dispose();

            var selector = GetComponent<CFoodSelector>(data.Target);
            var selected = (selector.Type + (data.Attempt.Type == InteractionType.Act ? 1 : -1)) % (choices.Count + 3);
            if (selected < 0) selected = choices.Count + 2;

            selector.Type = selected;

            var request = new HandleFoodRequests.SSelectedFood();

            if (selected == 1)
            {
                request.IgnoreFilter = true;
            } else if (selected == 2)
            {
                request.OnlyModded = true;
                request.IgnoreFilter = true;
            } else if (selected > 2)
            {
                selector.Selection = choices[selected - 3];
                request.ID = choices[selected - 3];
                request.IgnoreFilter = true;
            }

            Set(data.Target, selector);
            Set(request);
            Set<HandleFoodRequests.SRefreshFood>();
        }
    }
}
