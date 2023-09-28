using KitchenHQ.Utility;
using Kitchen;
using KitchenData;
using KitchenMods;
using System;
using System.Globalization;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    [UpdateInGroup(typeof(ModFranchiseComponentGroup))]
    internal class CreateModdedSeededRuns : FranchiseBuildSystem<CreateSeededRuns>, IModSystem
    {
        protected override void Build()
        {
            var office = LobbyPositionAnchors.Office;

            if (!Require(out SPlayerLevel sLevel) || sLevel.Level < 5)
            {
                LogDebug("[BUILD] [SKIP] Seeded maps (low level)");
                return;
            }

            LogDebug("[BUILD] Seeded maps");

            // Seeded runs
            EntityManager.AddComponent<CSeededRunInfo>(Create(AssetReference.SeededRunIndicator, office + new Vector3(0f, 0f, -5f), Vector3.forward));

            // Timed
            Create(FranchiseReferences.TimedBoardVisual, office + new Vector3(-5f, 0f, 0f), Vector3.forward);
            CreateSeededRun(GetSeed(true, AssetReference.FixedRunLayout), Create(AssetReference.FixedRunPedestal, office + new Vector3(-5.5f, 0f, 0f), Vector3.forward), AssetReference.FixedRunSetting);
            CreateSeededRun(GetSeed(false, AssetReference.FixedRunLayout), Create(AssetReference.FixedRunPedestal, office + new Vector3(-4.5f, 0f, 0f), Vector3.forward), AssetReference.FixedRunSetting);

            // Thematic
            if (!Data.TryGet(AssetReference.ThematicSetting, out RestaurantSetting _))
            {
                LogDebug("[BUILD] [SKIP] Thematic maps (none applicable)");
                return;
            }

            LogDebug("[BUILD] Thematic maps");

            Create(FranchiseReferences.ThematicBoardVisual, office + new Vector3(5.5f, 0f, 0f), Vector3.forward);
            CreateSeededRun(GetSeed(true, new int[] { AssetReference.ThematicLayout }), Create(AssetReference.FixedRunPedestal, office + new Vector3(5.5f, 0f, 0f), Vector3.forward), new int[] { AssetReference.ThematicSetting });
        }

        private LayoutSeed GetSeed(bool daily, int[] layout_ids)
        {
            var calendar = new GregorianCalendar();
            var dayOfYear = calendar.GetDayOfYear(DateTime.UtcNow);
            var weekOfYear = calendar.GetWeekOfYear(DateTime.UtcNow, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            var seed = calendar.GetYear(DateTime.UtcNow) * (daily ? 1000 : 100) + (daily ? dayOfYear : weekOfYear);
            LayoutSeed layoutSeed = new(seed, layout_ids);
            return layoutSeed;
        }

        private void CreateSeededRun(LayoutSeed seed, Entity pedestal, int[] settings)
        {
            var random = new System.Random(seed.FixedSeed.IntValue);

            var clearedSettings = settings.ToList();
            clearedSettings.RemoveAll(p => p == 0);
            var settingID = clearedSettings[random.Next(0, clearedSettings.Count)];

            if (!Data.TryGet(settingID, out RestaurantSetting setting, true))
                return;

            var map = seed.GenerateMap(EntityManager, settingID);
            EntityManager.AddComponentData<CItemHolder>(pedestal, map);
            EntityManager.SetComponentData<CHeldBy>(map, pedestal);
            EntityManager.AddComponentData<CHome>(map, pedestal);
            if (setting.FixedDish != null)
            {
                EntityManager.AddComponentData(map, new CSettingDish { DishID = setting.FixedDish.ID });
            }
        }

    }
}
