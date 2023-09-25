using KitchenHQ.Utility;
using Kitchen;
using KitchenData;
using KitchenMods;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class CreateModdedOffice : FranchiseBuildSystem<CreateOffice>, IModSystem
    {
        protected override void Build()
        {
            var office = LobbyPositionAnchors.Office;

            LogDebug("[BUILD] Office room");

            // Selection
            CreatePlanningBoard(LobbyPositionAnchors.Office);
            Create(AssetReference.SettingSelector, office + new Vector3(-2f, 0f, 0f), Vector3.forward);
            Create(FranchiseReferences.FoodSelector, office + new Vector3(2f, 0f, 0f), Vector3.forward);

            // Decoration
            Create(FranchiseReferences.Arch, office + new Vector3(3.5f, 0f, 0.5f), Vector3.forward);
        }

        // "The Plan" board
        private void CreatePlanningBoard(Vector3 location)
        {
            // First
            var e1 = EntityManager.CreateEntity(new ComponentType[]
            {
                typeof(CCreateAppliance),
                typeof(CPosition),
                typeof(SLayoutPedestal),
                typeof(SSelectedLayoutPedestal),
                typeof(CItemHolder)
            });
            EntityManager.SetComponentData(e1, new CCreateAppliance { ID = -760874610 });
            EntityManager.SetComponentData(e1, new CPosition(location + new Vector3(-1f, 0f, 0f), Quaternion.LookRotation(Vector3.forward, Vector3.up)));

            // Second
            var e2 = EntityManager.CreateEntity(new ComponentType[]
            {
                typeof(CCreateAppliance),
                typeof(CPosition),
                typeof(SSeededLayoutPedestal),
                typeof(CItemHolder),
                typeof(CPreventItemTransfer),
                typeof(CHideView)
            });
            EntityManager.SetComponentData(e2, new CCreateAppliance { ID = 1363960331 });
            EntityManager.SetComponentData(e2, new CPosition(location + new Vector3(-1f, 0f, 0f), Quaternion.LookRotation(Vector3.forward, Vector3.up)));
            
            // Third
            var e3 = EntityManager.CreateEntity(new ComponentType[]
            {
                typeof(CCreateAppliance),
                typeof(CPosition),
                typeof(SDishPedestal),
                typeof(CItemHolder)
            });
            EntityManager.SetComponentData(e3, new CCreateAppliance { ID = -1528441435 });
            EntityManager.SetComponentData(e3, new CPosition(location + new Vector3(1f, 0f, 0f), Quaternion.LookRotation(Vector3.forward, Vector3.up)));

            // Fourth
            var e4 = EntityManager.CreateEntity(new ComponentType[]
            {
                typeof(CCreateAppliance),
                typeof(CPosition),
                typeof(SFixedDishPedestal),
                typeof(CItemHolder),
                typeof(CHideView)
            });
            EntityManager.SetComponentData(e4, new CCreateAppliance { ID = -232172209 });
            EntityManager.SetComponentData(e4, new CPosition(location + new Vector3(1f, 0f, 0f), Quaternion.LookRotation(Vector3.forward, Vector3.up)));

            // Fifth
            var e5 = EntityManager.CreateEntity(new ComponentType[]
            {
                typeof(CCreateAppliance),
                typeof(CPosition)
            });
            EntityManager.SetComponentData(e5, new CCreateAppliance { ID = 359655899 });
            EntityManager.SetComponentData(e5, new CPosition(location, Quaternion.LookRotation(Vector3.forward, Vector3.up)));

        }
    }
}
