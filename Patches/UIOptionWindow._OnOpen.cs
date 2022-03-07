using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using static GS.GS;

namespace GS
{
    public static partial class Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIOptionWindow), "_OnOpen")]
        public static void UIOptionWindow__OnOpen(ref UIOptionWindow __instance, ref UIButton[] ___tabButtons, ref Text[] ___tabTexts)
        {
            Warn("OnOpen");
            var overlayCanvas = GameObject.Find("Overlay Canvas");
            if (overlayCanvas == null || overlayCanvas.transform.Find("Top Windows") == null)
            {
                Warn("Could not find Overlay Canvas or TopWindows");
                return;
            }
            var contentGS = GameObject.Find("Option Window/details/content-gsui");
            if (contentGS == null)
            {
                __instance.applyButton.button.onClick.AddListener(ApplySettings);
                Warn($"{___tabButtons.Length} {___tabTexts.Length}");
                PageManager.CreateSettingsPages(__instance);
                Warn($"{___tabButtons.Length} {___tabTexts.Length}");
            }

            // UIRoot.instance.optionWindow.SetTabIndex(SettingsUI.MainTabIndex, false);
            // SettingsUI.GalacticScaleTabClick();
            if (!CanvasOverlay)
            {
                overlayCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                CanvasOverlay = true;
            }
        }
    }
}