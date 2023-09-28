using Kitchen;
using System.Reflection;
using Unity.Collections;
using Unity.Entities;

namespace KitchenHQ.Franchise
{
    [UpdateBefore(typeof(ClearExcessLayoutData))]
    public class RefreshChoices : FranchiseSystem
    {
        private EntityQuery Refreshers;
        protected override void Initialise()
        {
            base.Initialise();
            Refreshers = GetEntityQuery(new ComponentType[] { typeof(CChoiceRefresher), typeof(CTakesDuration) });
            RequireForUpdate(Refreshers);
        }

        protected override void OnUpdate()
        {
            var durations = Refreshers.ToComponentDataArray<CTakesDuration>(Allocator.Temp);
            for (int i = 0; i < durations.Length; i++)
            {
                var duration = durations[i];
                if (duration.Remaining <= 0 && duration.Active)
                {
                    LogDebug("[CYCLE] Refreshing Layout Maps & Dish Choices");

                    Set(default(HandleLayoutRequests.SLayoutRequest));

                    if (GetSingleton<HandleFoodRequests.SSelectedFood>().ID == 0)
                        Set(default(HandleFoodRequests.SRefreshFood));
                }
            }
        }
    }
}
