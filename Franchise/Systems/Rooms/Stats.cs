using Kitchen;
using KitchenData;
using KitchenHQ.Utility;
using KitchenLib.Utils;
using KitchenMods;
using Steamworks;
using Steamworks.Ugc;
using System.Linq;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    [UpdateInGroup(typeof(ModFranchiseGroup))]
    public class CreateModdedStatsRoom : FranchiseBuildSystem<CreateStatsRoom>, IModSystem
    {
        protected override void Build()
        {
            var stats = LobbyPositionAnchors.Stats;

            LogDebug("[BUILD] Stats room");

            // Base
            Create(AssetReference.ExperienceView, stats + new Vector3(0f, 0f, -2f), Vector3.forward);
            Create(AssetReference.AchievementView, stats + new Vector3(0f, 0f, 0f), Vector3.forward);
            Create(AssetReference.UnlockTrackView, stats + new Vector3(2f, 0f, 0f), Vector3.forward);

            CreateSpeedrunBoard();

            // Modded
            LogDebug("[BUILD] Subscribed mods");
            var hasCount = WebUtility.AwaitTask(Task.Run(() => WebUtility.GetItemCountFromQuery(Query.Items.WhereUserSubscribed())), out int count);
            EntityManager.AddComponentData(
                Create(FranchiseReferences.SubscribedModsView, stats + new Vector3(0f, 0f, -4f), Vector3.forward),
                new CSubscribedModsView { ModCount = hasCount ? count : ModPreload.Mods.Count });

            CreateModdedUpgrades();
            CreateDevModView();
        }

        private void CreateDevModView()
        {
            LogDebug("[BUILD] Checking to build Developer Mod View");
            var devCount = WebUtility.AwaitTask(Task.Run(() => WebUtility.GetItemCountFromQuery(Query.Items.WhereUserPublished())), out int count);
            if (devCount && count > 0)
            {
                LogDebug("[BUILD] Building Developer Mod View");
                EntityManager.AddComponentData(
                    Create(FranchiseReferences.CreatedModsView, LobbyPositionAnchors.Stats + new Vector3(-2f, 0f, -4f), Vector3.forward),
                    new CCreatedModsView { UserID = SteamClient.SteamId });
            }
            else LogDebug("[BUILD] Skipping Developer Mod View");
        }

        private void CreateModdedUpgrades()
        {
            LogDebug("[BUILD] Custom upgrade view");

            var pos = LobbyPositionAnchors.Stats + new Vector3(2f, 0f, -4f);

            var table = Create(FranchiseReferences.ModdedUpgradesView, pos, Vector3.forward);
            EntityManager.AddComponentData(table, new CModdedUpgradeView
            {
                IsSetting = !GDOContainer.ModdedSettings.IsNullOrEmpty() && GDOContainer.ModdedDishes.IsNullOrEmpty()
            });

            EntityManager.AddComponentData(Create(AssetReference.InteractionProxy, pos + Vector3.right, Vector3.forward), new CInteractionProxy
                { Target = table, IsActive = true });
            EntityManager.AddComponentData(Create(AssetReference.InteractionProxy, pos - Vector3.right, Vector3.forward), new CInteractionProxy
                { Target = table, IsActive = true });
        }

        private void CreateSpeedrunBoard()
        {
            if (!Require(out SPlayerLevel sLevel) || sLevel.Level < 5)
            {
                LogDebug("[BUILD] [SKIP] Speedrun map (low level)");
                return;
            }

            LogDebug("[BUILD] Speedrun map");

            var current = SpeedrunHelpers.CurrentLeaderboardYearAndWeek();
            var year = current.Item1;
            var week = current.Item2;

            LayoutSeed layoutSeed = new(year * 200 + week, null);
            FixedSeedContext seedContext = new(layoutSeed.FixedSeed, 8853129);

            int setting_id;
            using (seedContext.UseSubcontext(0))
            {
                setting_id = Kitchen.RandomExtensions.Random(AssetReference.FixedRunSetting);
            }
            int id;
            using (seedContext.UseSubcontext(1))
            {
                id = Kitchen.RandomExtensions.Random((from x in GameData.Main.Get<Dish>()
                      where x.IsSpeedrunDish
                      select x).ToList()).ID;
            }

            var stats = LobbyPositionAnchors.Stats;

            EntityManager.AddComponent<CSpeedrunBoard>(Create(AssetReference.SpeedrunBoardVisual, stats + new Vector3(-2f, 0f, 0f), Vector3.forward));
            var pedestal = Create(AssetReference.FixedRunPedestal, stats + new Vector3(-2.5f, 0f, 0f), Vector3.forward);

            var map = layoutSeed.GenerateMap(EntityManager, setting_id);

            Set(map, new CSpeedrun
            {
                Seed = layoutSeed.FixedSeed,
                Year = year,
                Week = week,
                DishID = id
            });
            Set<CItemHolder>(pedestal, map);
            Set<CHeldBy>(map, pedestal);
            Set<CHome>(map, pedestal);

            seedContext.Dispose();
        }
    }
}
