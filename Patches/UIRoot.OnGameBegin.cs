using HarmonyLib;
using static GS.GS;

namespace GS
{
    public static partial class Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIRoot), "OnGameBegin")]
        public static void OnGameBegin()
        {
            Warn("OnGameBegin");
            Initialized = true;
            OnMenuLoaded();
        }
    }
}