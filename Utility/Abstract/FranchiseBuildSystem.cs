using KitchenHQ.Extensions;
using Kitchen;
using Unity.Entities;
using KitchenHQ.Franchise;

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

    public abstract class SeasonalFranchiseBuildSystem : FranchiseBuildSystem
    {
        protected abstract ModFranchise.SeasonalLobby Seasonal { get; }

        protected override void Build()
        {
            if (ModFranchise.CurrentSeasonal() == Seasonal)
                AddDecorations();
        }

        protected abstract void AddDecorations();
    }
}
