using Kitchen;
using Kitchen.Modules;
using UnityEngine;

namespace KitchenHQ.Franchise.Menus
{
    public class SelectTapeSortMenu : TapeEditorSubmenu
    {
        public SelectTapeSortMenu(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }

        public override void Setup(int player_id)
        {
            AddLabel("Select Sort Type");

            New<SpacerElement>().Size = 0.05f;

            AddButton("Newest", (int i) =>
            {
                RequestUpdate(new TapeValues
                {
                    Type = TapeTypes.Newest,
                    Tags = new(),
                    Search = "",
                    User = ""
                });
                RequestSubMenu(typeof(ModifyTapeDetailsMenu));
            });
            AddButton("Trending", (int i) =>
            {
                RequestUpdate(new TapeValues
                {
                    Type = TapeTypes.Trending,
                    Tags = new(),
                    Search = "",
                    User = ""
                });
                RequestSubMenu(typeof(ModifyTapeDetailsMenu));
            });
            New<SpacerElement>();
            AddButton("Complete", (int i) => RequestAction(TapeMenuAction.Close), 0, 0.75f);

            New<SpacerElement>().Size = 0.05f;
            AddInfo(RequestTape().FormatValues());
        }
    }
}
