using Unity.Entities;

namespace KitchenHQ.Extensions
{
    public static class WorldExtensions
    {
        public static void DisableSystem<T>(this World world) where T : ComponentSystemBase
        {
            var system = world.GetExistingSystem<T>();
            if (system == null)
                return;
            system.Enabled = false;
        }
    }
}
