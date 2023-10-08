using Kitchen;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class CreateTapeEditor : FranchiseSystem
    {
        protected override void Initialise()
        {
            base.Initialise();
            RequireSingletonForUpdate<STapeWriter>();
            RequireSingletonForUpdate<STapeWriter.STriggerEditor>();
        }

        protected override void OnUpdate()
        {
            var appliance = GetSingletonEntity<STapeWriter.STriggerEditor>();
            var sTrigger = GetSingleton<STapeWriter.STriggerEditor>();
            if (Has<STapeWriter.SHasEditor>() || Has<STapeWriter.SEditor>() ||
                !Require(sTrigger.Interactor, out CPlayer cPlayer) ||
                !Require(appliance, out CPosition cPos))
            {
                EntityManager.RemoveComponent<STapeWriter.STriggerEditor>(appliance);
                return;
            }

            var editor = EntityManager.CreateEntity();
            Set(editor, new CPosition(cPos.Position));
            Set(editor, new CRequiresView { Type = TapeEditorCustomView, ViewMode = ViewMode.WorldToScreen });
            Set(editor, new STapeWriter.SEditor { PlayerID = cPlayer.ID, Appliance = appliance });

            Set(appliance, new STapeWriter.SHasEditor { Editor = editor, Player = sTrigger.Interactor });

            EntityManager.RemoveComponent<STapeWriter.STriggerEditor>(appliance);
        }
    }
}
