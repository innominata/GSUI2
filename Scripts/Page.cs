using System.IO;
using System.Reflection;
using NGPT;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static GS.GS;
using Path = System.IO.Path;

namespace GS
{
    public class Page
    {
        public static RectTransform page;
        private static float anchorX;
        private static float anchorY;
        public static UnityEvent OptionsUIPostfix = new();
        private static RectTransform GSSettingsPanel;
        private static GSUIPanel SettingsPanel;
        private static readonly GSOptions options = new();
        public int pageIndex;
        public iConfigurablePlugin plugin;
        public RectTransform TabButton;
        public int tabIndex;

        public (RectTransform, Tweener) CreatePage(int _index)

        {
            Warn("Creating Page");
            pageIndex = _index;
            TabButton = Object.Instantiate(PageManager.tabButtonTemplate, PageManager.tabLine, false);
            TabButton.name = "tab-button-gsui";
            TabButton.anchoredPosition = new Vector2(TabButton.anchoredPosition.x + (pageIndex + 1) * 160, TabButton.anchoredPosition.y);
            Object.Destroy(TabButton.GetComponentInChildren<Localizer>());
            TabButton.GetComponent<Button>().onClick.RemoveAllListeners();
            TabButton.GetComponentInChildren<Text>().text = plugin.Name;
            TabButton.GetComponent<Button>().onClick.AddListener(TabClick);
            var detailsTemplate = GameObject.Find("Option Window/details/content-5").GetComponent<RectTransform>();
            page = Object.Instantiate(detailsTemplate, GameObject.Find("Option Window/details").GetComponent<RectTransform>(), false);
            page.gameObject.SetActive(true);
            page.gameObject.name = "content-gsui";

            var languageCombo = page.Find("language").GetComponent<RectTransform>();
            anchorX = languageCombo.anchoredPosition.x;
            anchorY = languageCombo.anchoredPosition.y;
            while (page.transform.childCount > 0) Object.DestroyImmediate(page.transform.GetChild(0).gameObject);
            CreateOptionsUI();
            OptionsUIPostfix.Invoke();
            return (TabButton, page.GetComponent<Tweener>());
        }

        private void CreateOptionsUI()
        {
            var gsuipath = Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(GSUI)).Location), "UnityUIExtensions.dll");
            if (!File.Exists(gsuipath))
            {
                ShowMessage("Missing UnityUIExtensions.dll".Translate(), "Error".Translate(), "Ok".Translate());
                return;
            }

            Assembly.LoadFrom(gsuipath);
            WarnJson(Bundle.GetAllAssetNames());
            var gsp = Bundle.LoadAsset<GameObject>("assets/gsuiparentpanel.prefab");
            GSSettingsPanel = Object.Instantiate(gsp, page, false).GetComponent<RectTransform>();
            GSSettingsPanel.GetComponent<ScrollRect>().scrollSensitivity = 10;
            // var sp = Bundle.LoadAsset<GameObject>("gsuiSettingsPanel");

            SettingsPanel = GSSettingsPanel.GetComponentInChildren<GSUIPanel>();
            options.AddRange(plugin.Options);
            Warn($"Adding UI Elements {plugin.Options.Count}");
            Warn(SettingsPanel?.name + "test");
            var scrollContentRect = SettingsPanel?.transform.parent.GetComponent<RectTransform>();
            Warn("Creating Settings");
            for (var i = 0; i < plugin.Options.Count; i++)
            {
                Warn($"Creating {plugin.Options[i].Label}");
                CreateUIElement(plugin.Options[i], SettingsPanel?.contents);
            }
        }

        private static void ProcessListContents(GSUIList parentList, GSUI group)
        {
            var config = (GSUIGroupConfig)group.Data;
            foreach (var option in config.options) CreateUIElement(option, parentList);
        }

        public void TabClick()
        {
            Warn($"Tab Clicked for {plugin.GUID}");
            var tabButton = TabButton.GetComponent<UIButton>();
            for (var i = 0; i < UIRoot.instance.optionWindow.tabButtons.Length; i++)
                if (tabButton == UIRoot.instance.optionWindow.tabButtons[i])
                {
                    Warn($"Found Button at index {i}");
                    tabIndex = i;
                    PageManager.SetTabIndex(i, pageIndex);
                }
        }

        public void Show()
        {
            page?.gameObject?.SetActive(true);
        }

        public void Hide()
        {
            page?.gameObject?.SetActive(false);
        }

        private static void CreateUIElement(GSUI option, GSUIList list)
        {
            Warn($"Creating UIElement {option.Label}");
            switch (option.Type)
            {
                case "Group":
                    if (option.callback == null)
                    {
                        // GS2.Log($"Adding normal list {option.Label}");
                        var newlist = list.AddList();
                        option.RectTransform = newlist.GetComponent<RectTransform>();
                        newlist.Initialize(option);
                        ProcessListContents(newlist, option);
                    }
                    else
                    {
                        // GS2.Log($"Adding toggle list {option.Label}");
                        var newlist = list.AddToggleList();
                        option.RectTransform = newlist.GetComponent<RectTransform>();
                        newlist.Initialize(option);
                        ProcessListContents(newlist, option);
                    }

                    break;
                case "Combobox":
                    var dropdown = list.AddDropdown();
                    option.RectTransform = dropdown.GetComponent<RectTransform>();
                    dropdown.initialize(option);
                    break;
                case "Selector":
                    var selector = list.AddSelector();
                    option.RectTransform = selector.GetComponent<RectTransform>();
                    selector.initialize(option);
                    break;
                case "Input":
                    var input = list.AddInput();
                    option.RectTransform = input.GetComponent<RectTransform>();
                    input.initialize(option);
                    break;
                case "Button":
                    var button = list.AddButton();
                    option.RectTransform = button.GetComponent<RectTransform>();
                    button.initialize(option);
                    break;
                case "Checkbox":
                    var toggle = list.AddToggle();
                    option.RectTransform = toggle.GetComponent<RectTransform>();
                    toggle.initialize(option);
                    break;
                case "Slider":
                    var slider = list.AddSlider();
                    option.RectTransform = slider.GetComponent<RectTransform>();
                    slider.initialize(option);
                    break;
                case "RangeSlider":
                    var rslider = list.AddRangeSlider();
                    option.RectTransform = rslider.GetComponent<RectTransform>();
                    rslider.initialize(option);
                    break;
                case "Header":
                    var header = list.AddHeader();
                    option.RectTransform = header.GetComponent<RectTransform>();
                    header.initialize(option);
                    break;
                case "Spacer":
                    var spacer = list.AddSpacer();
                    option.RectTransform = spacer;
                    break;
                case "Separator":
                    var separator = list.AddSeparator();
                    option.RectTransform = separator;
                    break;
                default:
                    Warn($"Couldn't create option {option.Label}");
                    break;
            }

            if (option.postfix != null) OptionsUIPostfix.AddListener(new UnityAction(option.postfix));
        }
    }
}