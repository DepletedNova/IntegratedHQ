﻿using Kitchen;
using UnityEngine;

namespace KitchenHQ.Franchise
{
    public class CreateTapeEditor : FranchiseSystem
    {
        protected override void Initialise()
        {
            base.Initialise();
            RequireSingletonForUpdate<STapeWriter>();
            RequireSingletonForUpdate<STapeWriter.STriggerTapeEditor>();
        }

        protected override void OnUpdate()
        {
            var appliance = GetSingletonEntity<STapeWriter.STriggerTapeEditor>();
            var sTrigger = GetSingleton<STapeWriter.STriggerTapeEditor>();
            if (Has<STapeWriter.SHasTapeEditor>() || Has<STapeWriter.STapeEditor>() ||
                !Require(sTrigger.Interactor, out CPlayer cPlayer) ||
                !Require(appliance, out CPosition cPos))
            {
                EntityManager.RemoveComponent<STapeWriter.STriggerTapeEditor>(appliance);
                return;
            }

            var editor = EntityManager.CreateEntity();
            Set(editor, new CPosition(cPos.Position));
            Set(editor, new CRequiresView { Type = TapeEditorCustomView, ViewMode = ViewMode.WorldToScreen });
            Set(editor, new STapeWriter.STapeEditor { PlayerID = cPlayer.ID, Appliance = appliance });

            Set(appliance, new STapeWriter.SHasTapeEditor { Editor = editor, Player = sTrigger.Interactor });

            EntityManager.RemoveComponent<STapeWriter.STriggerTapeEditor>(appliance);
        }
    }
}
