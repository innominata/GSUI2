using static GS.GS;
namespace GS
{
    public class testPlugin :iConfigurablePlugin
    {
        public string Name => "Test Plugin";
        public string Author => "inno";
        public string Description => "desc";
        public string Version => "Ver";
        public string GUID => "dsp.test.plugin";
        public GSOptions Options => options;
        private GSOptions options = new GSOptions();
        public static Settings Preferences = new();
        public bool Enabled
        {
            get
            {
                if (Preferences.ContainsKey("Enabled")) Warn("Contains");
                else Warn("Doesnt contain");
                return Preferences.GetBool("Enabled", true);
            }
            set
            {
                Warn($"Setting Enabled:{value}");
                Preferences.Set("Enabled", value);
            }
        }
        public void Init()
        {
            Log("Init");
            options.Add(GSUI.Button("Test", "Test2", null));
            options.Add(GSUI.Separator());options.Add(GSUI.Separator());options.Add(GSUI.Separator());options.Add(GSUI.Separator());
        }

        public void Import(Settings preferences)
        {
            Log("Import");
        }

        public Settings Export()
        {
            Log("Export");
            return Preferences;
        }

        public void OnUpdate(string key, Val val)
        {
            Log($"OnUpdate {key} {val}");
        }
    }
}