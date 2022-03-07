using System.Collections.Generic;
using HarmonyLib;
using NGPT;
using UnityEngine;
using UnityEngine.UI;
using static GS.GS;
namespace GS
{
    public class PageManager
    {
        public static int TabIndex = 5;
        public static int LastVanillaTabIndex = -1;
        public static RectTransform tabLine;
        public static RectTransform details;
        public static RectTransform tabButtonTemplate;
        public static readonly List<Page> Pages = new();

        public static void SetTabIndex(int tabIndex, int pageIndex)
        {
            Warn($"SetTabIndex({tabIndex} {GetCaller()}{GetCaller(1)}{GetCaller(2)})");
            // UIRoot.instance.optionWindow.SetTabIndex(LastVanillaTabIndex + 1 + index, false);
            // if (index <= LastVanillaTabIndex) HideAllPages();
            // else
            // {
                UIRoot.instance.optionWindow.SetTabIndex(tabIndex, false);
                for(var i=0;i<Pages.Count;i++)
                    if (i == pageIndex) Pages[pageIndex].Show();
                    else Pages[pageIndex].Hide();
            // }
        }

        public static void HideAllPages()
        {
            Warn("Hiding All Pages");
            // for (int i = 0; i < Pages.Count; i++)
            // {
            //     Pages[i].Hide();
            // }
        }
        public static void CreateSettingsPages(UIOptionWindow __instance)
        {
            Warn("Creating Settings Pages");
            Warn($"{Pages.Count}");
            tabLine = GameObject.Find("Top Windows/Option Window/tab-line").GetComponent<RectTransform>();
            var tabParent = GameObject.Find("Option Window/tab-line/tab-button-5").GetComponent<RectTransform>().parent;
            LastVanillaTabIndex = __instance.tabButtons.Length - 1;
            Warn($"Set LastVanillaTabIndex to {LastVanillaTabIndex}");
            tabButtonTemplate = tabParent.GetChild(tabParent.childCount - 1).GetComponent<RectTransform>();
            for (int i = 0; i < Pages.Count; i++)
            {
               (var b, var t) = Pages[i].CreatePage(i);
               var uibutton = b.GetComponent<UIButton>();
               var uitext = b.GetComponentInChildren<Text>();
               uibutton.gameObject.name = "ARGH";
               Warn($"{__instance.tabButtons.Length} ");
               UIButton[] newTabButtons = __instance.tabButtons.AddToArray(uibutton);
               __instance.tabButtons = newTabButtons;
               // __instance.tabButtons.AddItem(uibutton);
               Warn(__instance.tabButtons.Length.ToString());
               Text[] newTabTexts = __instance.tabTexts.AddToArray(uitext);
               __instance.tabTexts = newTabTexts;
               // __instance.tabTexts.AddItem(r.GetComponentInChildren<Text>());
               Tweener[] tabTweeners = __instance.tabTweeners;
               Tweener[] newContents = tabTweeners.AddToArray(t);
               __instance.tabTweeners = newContents;
               // RectTransform contentTemplate = tabTweeners[0].GetComponent<RectTransform>();
               // multiplayerContent = Object.Instantiate(contentTemplate, contentTemplate.parent, true);
               // multiplayerContent.name = "multiplayer-content";
            }
        }
    }
}