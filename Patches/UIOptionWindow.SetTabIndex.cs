using HarmonyLib;
using static GS.GS;

namespace GS
{
    public static partial class Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIOptionWindow), "SetTabIndex")]
        public static void SetTabIndex(int index, bool immediate, ref UIOptionWindow __instance)
        {
            Warn($"{index} <= {PageManager.LastVanillaTabIndex} LastIndex:{__instance.tabButtons.Length - 1}");
            if (index < PageManager.Pages[0]?.tabIndex) PageManager.HideAllPages();
        }
    }
}