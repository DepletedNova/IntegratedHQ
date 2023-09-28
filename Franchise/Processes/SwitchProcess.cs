using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using System.Collections.Generic;

namespace KitchenHQ.Franchise
{
    public class SwitchProcess : CustomProcess
    {
        public override string UniqueNameID => "switcher";
        public override bool CanObfuscateProgress => false;
        public override List<(Locale, ProcessInfo)> InfoList => new()
        {
            (Locale.English, CreateProcessInfo("Switch", "<sprite name=\"switch_0\">"))
        };
    }
}
