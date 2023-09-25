using KitchenHQ.Utility;
using Kitchen;
using KitchenData;
using KitchenMods;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class CreateModdedContractRoom : FranchiseBuildSystem<CreateContractRoom>, IModSystem
    {
        private EntityQuery Franchises;

        protected override void OnInitialise()
        {
            Franchises = GetEntityQuery(new ComponentType[] { typeof(CFranchiseItem) });
        }

        protected override void Build()
        {
            var anchor = LobbyPositionAnchors.Contracts;

            if (Franchises.IsEmpty)
            {
                LogDebug("[BUILD] [SKIP] Contracts room (no franchises)");
                Create(AssetReference.ContractLock, anchor + new Vector3(1.5f, 0f, 3f), Vector3.forward);
                return;
            }

            LogDebug("[BUILD] Contracts room");

            CreateProjector(anchor);
            CreateCardViewer(anchor);
            CreateCardScrapper(anchor);
        }

        private void CreateProjector(Vector3 anchor)
        {
            var projector = EntityManager.CreateEntity(new ComponentType[] { typeof(CCreateAppliance), typeof(CPosition), typeof(CAvailableFranchises), typeof(RebuildKitchen.CNonKitchen), typeof(SFranchiseSelector) });
            EntityManager.SetComponentData(projector, new CCreateAppliance { ID = AssetReference.ContractProjector });
            EntityManager.SetComponentData(projector, new CPosition(anchor + new Vector3(0f, 0f, 3f)));

            var buffer = GetBuffer<CAvailableFranchises>(projector);
            buffer.Add(new() { Franchise = Entity.Null });

            var franchises = Franchises.ToEntityArray(Allocator.Temp);
            foreach (var franchise in franchises)
                buffer.Add(new() { Franchise = franchise });
        }

        private void CreateCardViewer(Vector3 anchor)
        {
            var viewer = EntityManager.CreateEntity(new ComponentType[] { typeof(CCreateAppliance), typeof(CPosition), typeof(CFranchiseCardViewer), typeof(CPreventItemTransfer), typeof(RebuildKitchen.CNonKitchen) });
            EntityManager.SetComponentData(viewer, new CCreateAppliance { ID = AssetReference.CardViewer });
            EntityManager.SetComponentData(viewer, new CPosition(anchor + new Vector3(3f, 0f, 2f)));
        }

        private void CreateCardScrapper(Vector3 anchor)
        {
            var scrapper = EntityManager.CreateEntity(new ComponentType[] { typeof(CCreateAppliance), typeof(CPosition), typeof(CFranchiseScrapper), typeof(CPreventItemTransfer), typeof(RebuildKitchen.CNonKitchen) });
            EntityManager.SetComponentData(scrapper, new CCreateAppliance { ID = AssetReference.FranchiseShredder });
            EntityManager.SetComponentData(scrapper, new CPosition(anchor + new Vector3(3f, 0f, 3f)));
        }
    }
}
