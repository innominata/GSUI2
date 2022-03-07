using System.Collections;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace GS
{
    [BepInPlugin("dsp.gsui2", "GSUI 2 Plug-In", "2.0")]
    public class Bootstrap : BaseUnityPlugin
    {
        public new static ManualLogSource Logger;
        public static Queue buffer = new();

        internal void Awake()
        {
            
            InitializeLogger();
            Debug("Awake");
            GS.Warn("Awake");
            ApplyHarmonyPatches();
        }

        private void InitializeLogger()
        {
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            GS.Version = $"{v.Major}.{v.Minor}.{v.Build}";
            Logger = new ManualLogSource("GSUI2");
            BepInEx.Logging.Logger.Sources.Add(Logger);
            Debug("Initialized Logger");
        }

        private void ApplyHarmonyPatches()
        {
            Debug("Patching");
            var _ = new Harmony("dsp.gsui2");
            Harmony.CreateAndPatchAll(typeof(Patches));
        }

        public static void Debug(object data, LogLevel logLevel, bool isActive)
        {
            if (isActive && Logger != null)
            {
                while (buffer.Count > 0)
                {
                    var o = buffer.Dequeue();
                    var l = ((object data, LogLevel loglevel, bool isActive))o;
                    if (l.isActive) Logger.Log(l.loglevel, "Q:" + l.data);
                }

                Logger.Log(logLevel, data);
            }
            else
            {
                buffer.Enqueue((data, logLevel, true));
            }
        }

        public static void Debug(object data)
        {
            Debug(data, LogLevel.Message, true);
        }
    }
}