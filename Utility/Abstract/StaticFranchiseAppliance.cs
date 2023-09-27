using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenHQ.Utility
{
    public abstract class StaticFranchiseAppliance : CustomAppliance
    {
        public override bool IsNonInteractive => true;
        public override bool AutoGenerateNavMeshObject => false;
        public override List<IApplianceProperty> Properties => new()
        {
            new CImmovable(),
            new CStatic(),
            new CFixedRotation(),
            new CAllowPlacingOver()
        };

        public override GameObject Prefab => GetPrefab(UniqueNameID);
    }
}
