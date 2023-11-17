using Controllers;
using Kitchen;
using Kitchen.Modules;
using KitchenHQ.Franchise.Menus;
using KitchenLib.Utils;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Collections;
using Unity.Entities;

namespace KitchenHQ.Franchise
{
    public class TapeEditorView : ResponsiveObjectView<TapeEditorView.ViewData, TapeEditorView.ResponseData>, IInputConsumer
    {
        private bool IsComplete = false;
        private bool DoNotSet = false;
        private TapeValues Values;

        private int PlayerID;
        private InputLock.Lock Lock;
        private Type ActiveMenu;
        private Type PriorMenu;

        private PanelElement Panel;
        private ModuleList Modules;
        private Dictionary<Type, TapeEditorSubmenu> Menus;

        public override void Initialise()
        {
            base.Initialise();
            Values = new() { Type = TapeTypes.Newest, Tags = new(), Search = "", User = "" };
            Panel = Add<PanelElement>();
            Modules = new();
            Menus = new()
            {
                { typeof(SelectTapeSortMenu), new SelectTapeSortMenu(transform, Modules) },
                { typeof(ModifyTapeDetailsMenu), new ModifyTapeDetailsMenu(transform, Modules) },
                { typeof(AddTapeTagsMenu), new AddTapeTagsMenu(transform, Modules) },
                { typeof(TapeTextMenu), new TapeTextMenu(transform, Modules) }
            };
            foreach (var submenu in Menus.Values)
            {
                submenu.OnRequestMenu += (object _, Type t) => SetActiveMenu(t);
                submenu.OnRequestSkipStackMenu += (object _, Type t) => SetActiveMenu(t);
                submenu.OnRequestAction += OnRequestAction;
                submenu.OnPreviousMenu += (object _, Type T) => OnRequestAction(null, TapeMenuAction.Back);
                submenu.OnRequestUpdate += (object _, TapeValues values) => Values = values;
                submenu.OnRequestTape = () => Values;
            }
        }

        private void OnDestroy()
        {
            LocalInputSourceConsumers.Remove(this);
        }

        protected override void UpdateData(ViewData data)
        {
            if (InputSourceIdentifier.DefaultInputSource == null)
                return;

            if (!Players.Main.Get(data.PlayerID).IsLocalUser)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);

            PlayerID = data.PlayerID;
            Values = data.Data;

            LocalInputSourceConsumers.Register(this);
            if (Lock.Type != PlayerLockState.Unlocked)
                InputSourceIdentifier.DefaultInputSource.ReleaseLock(PlayerID, Lock);
            Lock = InputSourceIdentifier.DefaultInputSource.SetInputLock(PlayerID, PlayerLockState.NonPause);
            SetActiveMenu(typeof(SelectTapeSortMenu));
            Panel.SetColour(PlayerID);
        }

        public override bool HasStateUpdate(out IResponseData state)
        {
            state = null;
            if (IsComplete && !Values.Equals(default(TapeValues)))
            {
                state = new ResponseData
                {
                    DoNotSet = DoNotSet,
                    IsComplete = true,
                    Data = Values
                };
            }
            return IsComplete;
        }

        public InputConsumerState TakeInput(int playerID, InputState state)
        {
            if (PlayerID != playerID)
                return InputConsumerState.NotConsumed;

            if (state.MenuTrigger == ButtonState.Pressed)
            {
                IsComplete = true;
                DoNotSet = true;
                InputSourceIdentifier.DefaultInputSource.ReleaseLock(PlayerID, Lock);
                return InputConsumerState.Terminated;
            }

            if (!Modules.HandleInteraction(state) && state.MenuCancel == ButtonState.Pressed)
                LeaveCurrentMenu();

            if (!IsComplete)
                return InputConsumerState.Consumed;
            return InputConsumerState.Terminated;
        }

        public override void Remove()
        {
            IsComplete = true;
            DoNotSet = true;
            InputSourceIdentifier.DefaultInputSource.ReleaseLock(PlayerID, Lock);
            base.Remove();
        }

        #region ECS
        private class UpdateView : ResponsiveViewSystemBase<ViewData, ResponseData>
        {
            EntityQuery Views;
            protected override void Initialise()
            {
                base.Initialise();

                Views = GetEntityQuery(new ComponentType[] { typeof(CLinkedView), typeof(STapeWriter.STapeEditor) });
                RequireForUpdate(Views);

                RequireSingletonForUpdate<STapeWriter.SHasTapeEditor>();
                RequireSingletonForUpdate<STape>();
            }

            protected override void OnUpdate()
            {
                using var views = Views.ToComponentDataArray<CLinkedView>(Allocator.Temp);

                var hasEditor = GetSingleton<STapeWriter.SHasTapeEditor>();
                if (!Require(hasEditor.Player, out CPlayer cPlayer))
                    return;

                for (int i = 0; i < views.Length; i++)
                {
                    var view = views[i];
                    var tape = GetSingleton<STape>();
                    SendUpdate(view, new()
                    {
                        PlayerID = cPlayer.ID,
                        Data = (TapeValues)tape
                    });

                    ResponseData result = default;
                    if (ApplyUpdates(view.Identifier, (ResponseData data) => result = data, true))
                    {
                        var tapeEntity = GetSingletonEntity<STape>();
                        if (!result.DoNotSet)
                        {
                            LogDebug("[APPLIANCE] [Tape Editor] Setting Tape data");
                            Set(tapeEntity, (STape)result.Data);
                        }

                        var editorEntity = GetSingletonEntity<STapeWriter.STapeEditor>();
                        var editor = GetSingleton<STapeWriter.STapeEditor>();
                        editor.Completed = result.IsComplete;
                        Set(editorEntity, editor);
                    }
                }
            }
        }
        #endregion

        #region Menu Util
        private void OnRequestAction(object _, TapeMenuAction action)
        {
            if (action == TapeMenuAction.Return)
            {
                Values = new TapeValues
                {
                    Type = TapeTypes.Newest,
                    Tags = new(),
                    Search = "",
                    User = ""
                };

                SetActiveMenu(typeof(SelectTapeSortMenu));
                return;
            }
            if (action == TapeMenuAction.Back)
            {
                LeaveCurrentMenu(false);
                return;
            }
            if (action != TapeMenuAction.Close)
                return;
            CloseEditor();
        }

        private void SetActiveMenu(Type menu_type)
        {
            Modules.Clear();
            TapeEditorSubmenu submenu;
            if (!Menus.TryGetValue(menu_type, out submenu))
            {
                CloseEditor();
                return;
            }
            PriorMenu = ActiveMenu;
            ActiveMenu = menu_type;
            submenu.SetupWithPlayer(PlayerID);
            Panel.SetColour(PlayerID);
            Panel.SetTarget(Modules);
        }

        private void LeaveCurrentMenu(bool do_not_close = false)
        {
            if (!do_not_close && ActiveMenu == typeof(SelectTapeSortMenu))
            {
                CloseEditor();
                return;
            }
            SetActiveMenu(PriorMenu);
        }

        private void CloseEditor()
        {
            IsComplete = true;
            InputSourceIdentifier.DefaultInputSource.ReleaseLock(PlayerID, Lock);
            LocalInputSourceConsumers.Remove(this);
        }
        #endregion

        #region MessagePack Objects
        [MessagePackObject(false)]
        public struct ViewData : IViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(1)] public int PlayerID;
            [Key(2)] public TapeValues Data;

            public bool IsChangedFrom(ViewData check) => PlayerID != check.PlayerID;
        }

        [MessagePackObject(false)]
        public struct ResponseData : IResponseData
        {
            [Key(1)] public bool DoNotSet;
            [Key(2)] public bool IsComplete;
            [Key(3)] public TapeValues Data;
        }
        #endregion
    }
    public enum TapeMenuAction
    {
        Return,
        Back,
        Close
    }

    [MessagePackObject(false)]
    public struct TapeValues : IEquatable<TapeValues>
    {
        [Key(1)] public TapeTypes Type;
        [Key(2)] public List<string> Tags;
        [Key(3)] public string User;
        [Key(4)] public string Search;

        public bool Equals(TapeValues other)
        {
            if (Tags != null && other.Tags != null)
                foreach (var tag in other.Tags)
                    if (!Tags.Contains(tag))
                        return false;
            return Type == other.Type && User == other.User && Search == other.Search;
        }

        public string FormatValues()
        {
            // Tags
            string tags = "";
            Tags.RemoveAll(s => s.IsNullOrEmpty());
            for (int i = 0; i < Tags.Count; i++)
            {
                tags += Tags[i];
                if (i < Tags.Count - 1)
                {
                    tags += ", ";
                }
            }

            string result = "";
            AddValue(ref result, "Filter", Type.ToString());
            AddValue(ref result, "Tags", tags);
            AddValue(ref result, "Creator", $"\"{User}\"");
            AddValue(ref result, "Search", $"\"{Search}\"");
            return result;
        }

        private void AddValue(ref string Input, string Name, string Value)
        {
            if (Value.Trim('"') != string.Empty)
            {
                if (Input.Length > 0) Input += "\n";
                Input += $"<color=#DDE542>{Name}</color>: {Value}";
            }
        }

        public static implicit operator STape(TapeValues Values)
        {
            Values.Tags.RemoveAll(s => s.IsNullOrEmpty());
            string tag = "";
            foreach (var newTag in Values.Tags)
                tag += newTag + ";";

            var tape = new STape() { Type = (int)Values.Type };

            if (!tag.IsNullOrEmpty())
                tape.Tags = tag;
            if (!Values.Search.IsNullOrEmpty())
                tape.Search = Values.Search;
            if (!Values.User.IsNullOrEmpty())
                tape.User = Values.User;

            return tape;
        }

        public static explicit operator TapeValues(STape Values)
        {
            TapeValues tape = new()
            {
                Type = (TapeTypes)Values.Type,
                Tags = new(),
                Search = "",
                User = ""
            };

            if (Values.Tags != default(FixedString512) && !Values.Tags.IsEmpty)
                tape.Tags = Values.Tags.ConvertToString().Split(';').ToList();
            if (Values.Search != default(FixedString512) && !Values.Search.IsEmpty)
                tape.Search = Values.Search.ConvertToString();
            if (Values.User != default(FixedString128) && !Values.User.IsEmpty)
                tape.User = Values.User.ConvertToString();

            return tape;
        }

    }
}
