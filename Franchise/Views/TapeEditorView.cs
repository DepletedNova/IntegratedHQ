using Controllers;
using Kitchen;
using Kitchen.Modules;
using KitchenHQ.Franchise.Menus;
using MessagePack;
using System;
using System.Collections.Generic;
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
                string tag = "";
                foreach (var newTag in Values.Tags)
                    tag += newTag + ";";
                state = new ResponseData
                {
                    DoNotSet = DoNotSet,
                    IsComplete = true,
                    Type = (int)Values.Type,
                    Search = Values.Search,
                    User = Values.User,
                    Tag = tag
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

                Views = GetEntityQuery(new ComponentType[] { typeof(CLinkedView), typeof(STapeWriter.SEditor) });
                RequireForUpdate(Views);

                RequireSingletonForUpdate<STapeWriter.SHasEditor>();
                RequireSingletonForUpdate<STape>();
            }

            protected override void OnUpdate()
            {
                using var views = Views.ToComponentDataArray<CLinkedView>(Allocator.Temp);

                var hasEditor = GetSingleton<STapeWriter.SHasEditor>();
                if (!Require(hasEditor.Player, out CPlayer cPlayer))
                    return;

                for (int i = 0; i < views.Length; i++)
                {
                    var view = views[i];
                    SendUpdate(view, new()
                    {
                        PlayerID = cPlayer.ID
                    });

                    ResponseData result = default(ResponseData);
                    if (ApplyUpdates(view.Identifier, (ResponseData data) => result = data, true))
                    {
                        var tapeEntity = GetSingletonEntity<STape>();
                        if (!result.DoNotSet)
                        {
                            Set(tapeEntity, new STape
                            {
                                Type = (TapeTypes)result.Type,
                                Search = result.Search,
                                Tag = result.Tag,
                                User = result.User
                            });
                        }

                        var editorEntity = GetSingletonEntity<STapeWriter.SEditor>();
                        var editor = GetSingleton<STapeWriter.SEditor>();
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
        [MessagePackObject]
        public struct ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(1)] public int PlayerID;

            public IUpdatableObject GetRelevantSubview(IObjectView view) => view.GetSubView<TapeEditorView>();

            public bool IsChangedFrom(ViewData check) => PlayerID != check.PlayerID;
        }

        [MessagePackObject]
        public struct ResponseData : IResponseData
        {
            [Key(0)] public bool DoNotSet;
            [Key(1)] public bool IsComplete;
            [Key(2)] public int Type;
            [Key(3)] public FixedString512 Tag;
            [Key(4)] public FixedString128 User;
            [Key(5)] public FixedString512 Search;
        }
        #endregion
    }
    public enum TapeMenuAction
    {
        Return,
        Back,
        Close
    }

    public struct TapeValues
    {
        public TapeTypes Type;
        public List<string> Tags;
        public string User;
        public string Search;

        public string FormatValues()
        {
            string tags = "";
            foreach (var tag in Tags)
                tags += $"{tag}, ";
            tags.Substring(0, Math.Max(tags.Length - 2, tags.Length));
            var result = $"<color=#DDE542>Sort</color>: {Type}\n" +
                $"<color=#DDE542>Tags</color>: {tags}\n" +
                $"<color=#DDE542>Creator</color>: \"{User}\"\n" +
                $"<color=#DDE542>Search</color>: \"{Search}\"\n";
            result = Regex.Replace(result, @"""", string.Empty);
            return result;
        }


    }
}
