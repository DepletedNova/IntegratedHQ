using Kitchen;
using Kitchen.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace KitchenHQ.Franchise.Menus
{
    public class AddTapeTagsMenu : TapeEditorSubmenu
    {
        public AddTapeTagsMenu(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }

        private Dictionary<string, bool> Tags = new();

        private readonly List<string> TagsList = new List<string>()
        {
            "Appliance",
            "Audio",
            "Cards",
            "Cosmetics",
            "Dishes",
            "Graphics",
            "Layouts",
            "Modifiers",
            "Mod Tools",
            "Multiplayer",
            "Tweaks"
        };

        private List<ButtonElement> Buttons;

        public override void Setup(int player_id)
        {
            Buttons = new();

            AddLabel("Add Tags (4 max)");

            New<SpacerElement>().Size = 0.05f;

            var tape = RequestTape();

            InfoBoxElement tapeInfo = ModuleDirectory.Add<InfoBoxElement>(Container);
            tapeInfo.SetSize(DefaultElementSize.x, tapeInfo.BoundingBox.size.y);
            tapeInfo.SetLabel(tape.FormatValues());
            tapeInfo.SetStyle(Style);

            var currentTags = tape.Tags;
            Tags = new();
            for (int i = 0; i < TagsList.Count; i++)
            {
                var tag = TagsList[i];
                Tags[tag] = currentTags.Contains(tag);

                var button = ModuleDirectory.Add<ButtonElement>(Container, Vector2.zero);
                Buttons.Add(button);
                button.SetSize(DefaultElementSize.x, DefaultElementSize.y);
                button.SetStyle(Style);

                button.SetLabel(FormatTag(tag));
                button.OnActivate += delegate
                {
                    Tags[tag] = !Tags[tag];
                    button.SetLabel(FormatTag(tag));
                    tapeInfo.SetLabel(CreateUpdatedTape().FormatValues());

                    for (int i = 0; i < TagsList.Count; i++)
                    {
                        var tag = TagsList[i];
                        var button = Buttons[i];
                        button.SetSelectable(Tags[tag] || Tags.Values.Count((x) => x) < 4);
                    }

                    TriggerRedraw();
                };

                button.SetSelectable(Tags[tag] || Tags.Values.Count((x) => x) < 4);

                ModuleList.AddModule(button);
            }

            New<SpacerElement>();

            AddButton("Cancel", (int i) => RequestAction(TapeMenuAction.Back), 0, 0.75f);
            AddButton("Remove Tags", (int i) =>
            {
                var tape = RequestTape();

                tape.Tags = new();
                tape.Type = (TapeTypes)((tape.Type | TapeTypes.Tagged) - TapeTypes.Tagged);

                RequestUpdate(tape);
                RequestAction(TapeMenuAction.Back);
            }, 0, 0.75f).SetSelectable(currentTags.Count > 0);
            AddButton("Set Tags", (int i) =>
            {
                RequestUpdate(CreateUpdatedTape());
                RequestAction(TapeMenuAction.Back);
            }, 0, 0.75f);

            New<SpacerElement>().Size = 0.05f;
            ModuleList.AddModule(tapeInfo);
        }

        private TapeValues CreateUpdatedTape()
        {
            var Tape = RequestTape();
            Tape.Tags = new();
            foreach (var pair in Tags)
                if (pair.Value)
                    Tape.Tags.Add(pair.Key);

            Tape.Type |= TapeTypes.Tagged;
            if (Tape.Tags.Count < 1)
                Tape.Type -= TapeTypes.Tagged;

            return Tape;
        }

        private string FormatTag(string tag) => $"[{(Tags[tag] ? "<color=#A8FF1E>+</color>" : "<color=#D1171D>x</color>")}]   {tag}";
    }
}
