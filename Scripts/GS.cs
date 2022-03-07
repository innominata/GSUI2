using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using BepInEx;
using UnityEngine;
using UnityEngine.UI;

namespace GS
{
    public partial class GS
    {
        public static string Version;
        public static bool Debug = true;
        public static string DataDir = Path.Combine(Paths.ConfigPath, "GSUI");
        public static List<iConfigurablePlugin> Plugins = new();
        public static bool Initialized = false;
        public static bool CanvasOverlay = false;
        private static readonly string AssemblyPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(GS)).Location);
        private static AssetBundle bundle;
        public static AssetBundle Bundle
        {
            get
            {
                if (bundle == null)
                {
                    var path = Path.Combine(AssemblyPath, "gsui.assetbundle");
                    var path2 = Path.Combine(AssemblyPath, "gsui.assetbundle");
                    if (File.Exists(path)) bundle = AssetBundle.LoadFromFile(path);
                    else bundle = AssetBundle.LoadFromFile(path2);
                    // foreach (var name in _bundle.GetAllAssetNames()) GS2.Warn("Bundle Contents:" + name);
                }
                if (bundle == null)
                {
                    Error("Failed to load AssetBundle!".Translate());
                    UIMessageBox.Show("Error", "Asset Bundle not found. \r\nPlease ensure your directory structure is correct.\r\n Installation instructions can be found at http://customizing.space/release. \r\nAn error log has been generated in the plugin/ErrorLog Directory".Translate(), "Return".Translate(), 0);
                    return null;
                }
                return bundle;
            }
        }
        public static void ShowMessage(string message, string title = "GSUI", string button = "OK")
        {
            UIMessageBox.Show(title.Translate(), message.Translate(), button.Translate(), 0);
        }
        public static Type GetCallingType()
        {
            return new StackTrace().GetFrame(2).GetMethod().ReflectedType;
        }

        public static iConfigurablePlugin GetConfigurablePluginInstance(Type t)
        {
            foreach (var g in Plugins)
                if (g.GetType() == t)
                    if (g is iConfigurablePlugin)
                        return g;
            return null;
        }

        public static void OnMenuLoaded()
        {
            Warn("Menu Loaded, Adding Pages");
            var testPlugin = new testPlugin() { };
            testPlugin.Init();
            var p = new Page() { plugin = testPlugin };
            
            PageManager.Pages.Add(p);
        }

        public static void ApplySettings()
        {
        }


    }
}