using System.Collections.Generic;
using static GS.GS;

namespace GS
{
    public partial class GSUI
    {
        public static GSUI Header(string label, string hint = "")
        {
            return new GSUI(label, null, "Header", null, null, null, hint);
        }

        public static GSUI Spacer()
        {
            return new GSUI(null, null, "Spacer", null, null, null, null);
        }

        public static GSUI Group(string label, List<GSUI> options, string hint = "", bool header = true, bool collapsible = true, GSOptionCallback callback = null)
        {
            var data = new GSUIGroupConfig(options, header, collapsible);

            var instance = new GSUI(label, null, "Group", data, null);
            instance.callback = callback;
            return instance;
        }

        public static GSUI Selector(string label, List<string> items, string defaultValue, string key, GSOptionCallback callback = null, string hint = null)
        {
            GSUI instance = null;
            var tt = GetCallingType();
            foreach (var t in tt.GetInterfaces())

                if (t.Name == "iConfigurablePlugin" && !tt.IsAbstract && !tt.IsInterface)
                    instance = new GSUI(GetConfigurablePluginInstance(tt), key, label, "Selector", items, null, null, hint);
            if (instance == null) return null;
            instance.callback = instance.CreateDefaultCallback(callback);
            instance.postfix = instance.CreateDefaultPostfix();
            instance.comboDefault = items.IndexOf(defaultValue);
            return instance;
        }

        // Group with Checkbox and Key
        public static GSUI Group(string label, List<GSUI> options, string key, bool defaultValue, string hint = "", bool collapsible = true, GSOptionCallback callback = null)
        {
            var data = new GSUIGroupConfig(options, true, collapsible, defaultValue);
            GSUI instance = null;
            var tt = GetCallingType();
            foreach (var t in tt.GetInterfaces())

                if (t.Name == "iConfigurablePlugin" && !tt.IsAbstract && !tt.IsInterface)
                    instance = new GSUI(GetConfigurablePluginInstance(tt), key, label, "Group", data, null, null, hint);
            if (instance == null) return null;
            instance.callback = instance.CreateDefaultCallback(callback);
            instance.postfix = instance.CreateDefaultPostfix();
            return instance;
        }

        //Slider with preferences Key
        public static GSUI Slider(string label, float min, float val, float max, string key, GSOptionCallback callback = null, string hint = "")
        {
            GSUI instance = null;
            var tt = GetCallingType();
            foreach (var t in tt.GetInterfaces())
                if (t.Name == "iConfigurablePlugin" && !tt.IsAbstract && !tt.IsInterface)
                    instance = new GSUI(GetConfigurablePluginInstance(tt), key, label, "Slider", new GSSliderConfig { minValue = min, maxValue = max, defaultValue = val }, null, null, hint);

            if (instance == null) return null;
            instance.callback = instance.CreateDefaultCallback(callback);
            instance.postfix = instance.CreateDefaultPostfix();
            return instance;
        }

        //Slider with increment and preferences Key
        public static GSUI Slider(string label, float min, float val, float max, float increment, string key, GSOptionCallback callback = null, string hint = "", string negativeLabel = "")
        {
            GSUI instance = null;
            var tt = GetCallingType();
            foreach (var t in tt.GetInterfaces())
                if (t.Name == "iConfigurablePlugin" && !tt.IsAbstract && !tt.IsInterface)
                    instance = new GSUI(GetConfigurablePluginInstance(tt), key, label, "Slider", new GSSliderConfig { minValue = min, maxValue = max, defaultValue = val, negativeLabel = negativeLabel }, null, null, hint);
            if (instance == null) return null;
            var defaultCallback = instance.CreateDefaultCallback(callback);
            var CB = defaultCallback;
            if (increment != 1f) CB = CreateIncrementCallback(increment, instance, defaultCallback);
            instance.callback = CB;
            instance.postfix = instance.CreateDefaultPostfix();
            instance.increment = increment;
            return instance;
        }


        //RangeSlider with increment and preferences Key

        public static GSUI RangeSlider(string label, float min, float lowVal, float highVal, float max, float increment, string key, GSOptionCallback callback = null, GSOptionCallback callbackLow = null, GSOptionCallback callbackHigh = null, string hint = "")
        {
            GSUI instance = null;
            var tt = GetCallingType();
            foreach (var t in tt.GetInterfaces())
                if (t.Name == "iConfigurablePlugin" && !tt.IsAbstract && !tt.IsInterface)
                    instance = new GSUI(GetConfigurablePluginInstance(tt), key, label, "RangeSlider", new GSRangeSliderConfig { minValue = min, maxValue = max, defaultLowValue = lowVal, defaultHighValue = highVal, callbackLow = callbackLow, callbackHigh = callbackHigh }, null, null, hint);
            if (instance == null) return null;

            var defaultCallback = instance.CreateDefaultCallback(callback);
            var CB = defaultCallback;
            if (increment != 1f) CB = CreateIncrementCallback(increment, instance, defaultCallback);
            instance.callback = CB;
            instance.postfix = instance.CreateDefaultPostfix();
            instance.increment = increment;
            return instance;
        }

        //Checkbox with key
        public static GSUI Checkbox(string label, bool defaultValue, string key, GSOptionCallback callback = null, string hint = "")
        {
            GSUI instance = null;
            var tt = GetCallingType();
            foreach (var t in tt.GetInterfaces())
                if (t.Name == "iConfigurablePlugin" && !tt.IsAbstract && !tt.IsInterface)
                    instance = new GSUI(GetConfigurablePluginInstance(tt), key, label, "Checkbox", defaultValue, null, null, hint);
            if (instance == null) return null;

            instance.callback = instance.CreateDefaultCallback(callback);
            instance.postfix = instance.CreateDefaultPostfix();
            return instance;
        }

        //Combobox with key
        public static GSUI Combobox(string label, List<string> items, int defaultValue, string key, GSOptionCallback callback = null, string hint = "")
        {
            GSUI instance = null;
            var tt = GetCallingType();
            foreach (var t in tt.GetInterfaces())
                if (t.Name == "iConfigurablePlugin" && !tt.IsAbstract && !tt.IsInterface)
                    instance = new GSUI(GetConfigurablePluginInstance(tt), key, label, "Combobox", items, null, null, hint);
            if (instance == null) return null;
            instance.callback = instance.CreateDefaultCallback(callback);
            instance.postfix = instance.CreateDefaultPostfix();
            instance.comboDefault = defaultValue;
            return instance;
        }

        //Input with Key
        public static GSUI Input(string label, string defaultValue, string key, GSOptionCallback callback = null, string hint = "")
        {
            GSUI instance = null;
            var tt = GetCallingType();
            foreach (var t in tt.GetInterfaces())
                if (t.Name == "iConfigurablePlugin" && !tt.IsAbstract && !tt.IsInterface)
                    instance = new GSUI(GetConfigurablePluginInstance(tt), key, label, "Input", defaultValue, null, null, hint);
            if (instance == null) return null;
            instance.callback = instance.CreateDefaultCallback(callback);
            instance.postfix = instance.CreateDefaultPostfix();
            return instance;
        }

        //Button
        public static GSUI Button(string label, string caption, GSOptionCallback callback, GSOptionPostfix postfix = null, string hint = "")
        {
            var t = GetCallingType();
            var instance = new GSUI(GetConfigurablePluginInstance(t), null, label, "Button", caption, callback, null, hint);
            return instance;
        }
    }
}