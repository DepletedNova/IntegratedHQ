using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class VHSTape : CustomItem
    {
        public override string UniqueNameID => "VHS Tape";

        public override ItemCategory ItemCategory => TapeItemCategory;
        public override bool IsIndisposable => true;

        public override GameObject Prefab => GetPrefab(UniqueNameID);
        public override void SetupPrefab(GameObject prefab)
        {
            prefab.ApplyMaterialToChild("Tape", "Plastic - Black", "Metal Very Dark", "Plastic - Blue");
        }

    }
}
