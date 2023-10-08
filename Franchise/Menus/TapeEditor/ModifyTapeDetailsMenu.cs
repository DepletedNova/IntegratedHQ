using Kitchen;
using Kitchen.Modules;
using UnityEngine;
using static Kitchen.TextInputView;

namespace KitchenHQ.Franchise.Menus
{
    public class ModifyTapeDetailsMenu : TapeEditorSubmenu
    {
        public ModifyTapeDetailsMenu(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }

        public override void Setup(int player_id)
        {
            AddLabel("Modify Details");

            New<SpacerElement>().Size = 0.05f;

            AddSubmenuButton("Add Tags", typeof(AddTapeTagsMenu));
            AddButton("Set Creator ID", (int i) =>
            {
                RequestTextInput("Set Creator ID", "", 32, (TextInputState state, string value) =>
                {
                    if (state == TextInputState.TextEntryComplete)
                    {
                        var tape = RequestTape();
                        tape.Type |= TapeTypes.Creator;
                        tape.User = value;
                        if (value == "")
                            tape.Type -= TapeTypes.Creator;
                        RequestUpdate(tape);
                    }
                });
                RequestSubMenu(typeof(TapeTextMenu));
            });
            AddButton("Set Search", (int i) =>
            {
                RequestTextInput("Set Search", "", 32, (TextInputState state, string value) =>
                {
                    if (state == TextInputState.TextEntryComplete)
                    {
                        var tape = RequestTape();
                        tape.Type |= TapeTypes.Search;
                        tape.Search = value;
                        if (value == "")
                            tape.Type -= TapeTypes.Search;
                        RequestUpdate(tape);
                    }
                });
                RequestSubMenu(typeof(TapeTextMenu));
            });

            New<SpacerElement>();

            AddButton("Cancel", (int i) => RequestAction(TapeMenuAction.Return), 0, 0.75f);
            AddButton("Complete", (int i) => RequestAction(TapeMenuAction.Close), 0, 0.75f);

            New<SpacerElement>().Size = 0.05f;
            AddInfo(RequestTape().FormatValues());
        }
    }
}
