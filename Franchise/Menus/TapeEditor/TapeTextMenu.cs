using Kitchen;
using Kitchen.Modules;
using UnityEngine;

namespace KitchenHQ.Franchise.Menus
{
    public class TapeTextMenu : TapeEditorSubmenu
    {
        public TapeTextMenu(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }

        public override void Setup(int player_id)
        {
            AddButton("Adding text...", (int i) => RequestAction(TapeMenuAction.Back));
        }
    }
}
