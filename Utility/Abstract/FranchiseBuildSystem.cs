using KitchenHQ.Extensions;
using Kitchen;
using Unity.Entities;

namespace KitchenHQ.Utility
{
    public abstract class FranchiseBuildSystem : FranchiseFirstFrameSystem
    {
        protected virtual bool OverrideBuild => false;

        protected static bool ReplaceHQ { get => !PrefManager.Get<bool>("LegacyHQEnabled"); }

        protected override void OnUpdate()
        {
            if (!ReplaceHQ && !OverrideBuild)
                return;

            Build();
        }

        protected abstract void Build();
    }

    public abstract class FranchiseBuildSystem<T> : FranchiseBuildSystem where T : GenericSystemBase
    {
        protected virtual bool OverrideReplace => false;

        protected virtual void OnInitialise() { }

        protected override void Initialise()
        {
            base.Initialise();

            OnInitialise();

            if (!ReplaceHQ && !OverrideReplace)
                return;

            LogDebug($"[SYSTEMS] Disabling system \"{typeof(T).FullName}\"");

            World.DisableSystem<T>();
        }
    }
}
