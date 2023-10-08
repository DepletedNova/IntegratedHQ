using Kitchen;
using Kitchen.Modules;
using System;
using UnityEngine;

namespace KitchenHQ.Franchise.Menus
{
    public abstract class TapeEditorSubmenu : Menu<TapeMenuAction>
    {
        protected TapeEditorSubmenu(Transform container, ModuleList module_list) : base(container, module_list)
        {
            DefaultElementSize = new Vector2(2.5f, 0.35f);
            module_list.Padding = 0.05f;
        }

        public event EventHandler<TapeValues> OnRequestUpdate = delegate
        {
        };

        public Func<TapeValues> OnRequestTape;

        protected TapeValues RequestTape() => OnRequestTape.Invoke();

        protected void RequestUpdate(TapeValues value) => OnRequestUpdate.Invoke(null, value);

        public void SetupWithPlayer(int ID)
        {
            PlayerID = ID;
            Setup(ID);
        }

        protected int PlayerID;
    }
}
