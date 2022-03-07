using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GS.GS;

namespace GS
{
    public delegate void GSOptionCallback(Val o);

    public delegate void GSOptionPostfix();

    public partial class GSUI
    {
        private static readonly Dictionary<int, Color> colors = new();

        public static List<string> Settables = new()
        {
            "Slider",
            "RangeSlider",
            "Checkbox",
            "Combobox",
            "Inputfield"
        };

        private readonly string key;
        private readonly iConfigurablePlugin plugin;
        public GSOptionCallback callback;
        private int comboDefault = -1;
        public float increment = 1f;
        public RectTransform RectTransform;

        private GSUI()
        {
        }

        public GSUI(string label, string key, string type, object data, GSOptionCallback callback, GSOptionPostfix postfix = null, string tip = "")
        {
            var tt = GetCallingType();
            foreach (var t in tt.GetInterfaces())
                if (t.Name == "iConfigurablePlugin" && !tt.IsAbstract && !tt.IsInterface)
                    plugin = GetConfigurablePluginInstance(tt);
            Label = label;
            Type = type;
            Data = data;
            this.callback = callback;
            if (postfix == null)
                this.postfix = delegate { };
            else
                this.postfix = postfix;
            Hint = tip;
        }

        public GSUI(iConfigurablePlugin plugin, string key, string label, string type, object data, GSOptionCallback callback, GSOptionPostfix postfix = null, string hint = "")
        {
            this.plugin = plugin;
            this.key = key;
            Label = label;
            Type = type;
            Data = data;
            this.callback = callback;
            if (postfix == null)
                this.postfix = delegate { };
            else
                this.postfix = postfix;
            Hint = hint;
        }

        public string Label { get; }
        public string Hint { get; private set; }
        public string Type { get; }
        public object Data { get; private set; }


        private iConfigurablePlugin Plugin
        {
            get
            {
                if (plugin != null) return plugin;
                Error($"GSUI ${Label} Tried accessing Plugin instance when Generator = null." + GetCaller() + GetCaller(1));
                return null;
            }
        }

        private Slider slider => RectTransform.GetComponentInChildren<Slider>();


        public Val DefaultValue
        {
            get
            {
                switch (Type)
                {
                    case "Group":
                        var gcfg = Data is GSUIGroupConfig ? (GSUIGroupConfig)Data : new GSUIGroupConfig(null, true, true);
                        return gcfg.defaultValue;
                    case "Slider":
                        var cfg = Data is GSSliderConfig ? (GSSliderConfig)Data : new GSSliderConfig(-1, -1, -1);
                        return cfg.defaultValue;
                    case "RangeSlider":
                        var rcfg = Data is GSRangeSliderConfig ? (GSRangeSliderConfig)Data : new GSRangeSliderConfig(-1, -1, -1, -1);
                        return rcfg.defaultValue;
                    case "Checkbox":
                        var bresult = GetBool(Data);
                        if (bresult.succeeded) return bresult.value;
                        Warn($"No default value found for Checkbox {Label}");
                        return false;
                    case "Button":
                        Error("Trying to get default value for button {label}");
                        return null;
                    case "Input":
                        return Data.ToString();
                    case "Combobox":
                        if (comboDefault >= 0) return comboDefault;
                        Warn($"No default value found for Combobox {Label}");
                        return false;
                    case "Selector":
                        var list = Data as List<string>;
                        if (comboDefault >= 0)
                            if (list.Count > comboDefault)
                                return list[comboDefault];
                        Warn($"No default value found for Combobox {Label}");
                        return false;
                }

                Error($"Failed to return default value for {Type} {Label}");
                return null;
            }
        }

        public bool Disabled { get; private set; }

        public GSOptionPostfix postfix { get; private set; }

        private (bool succeeded, float value) GetFloat(object o)
        {
            if (o is float) return (true, (float)o);
            var success = float.TryParse(o.ToString(), out var result);
            return (success, result);
        }

        private (bool succeeded, int value) GetInt(object o)
        {
            if (o is int) return (true, (int)o);
            var success = int.TryParse(o.ToString(), out var result);
            return (success, result);
        }

        private (bool succeeded, bool value) GetBool(object o)
        {
            if (o is bool) return (true, (bool)o);
            var success = bool.TryParse(o.ToString(), out var result);
            return (success, result);
        }

        public void Reset()
        {
            Set(DefaultValue);
        }

        public void Show()
        {
            RectTransform?.gameObject.SetActive(true);
        }

        public void Hide()
        {
            RectTransform?.gameObject.SetActive(false);
        }

        public bool Disable()
        {
            if (Disabled)
            {
                Warn("Trying to disable UI Element that is already disabled");
                return false;
            }

            var c = RectTransform.GetComponentsInChildren<Image>();
            foreach (var i in c)
            {
                var id = i.GetInstanceID();
                if (colors.ContainsKey(id)) colors[id] = i.color;
                else colors.Add(id, i.color);
                i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a / 2);
            }

            switch (Type)
            {
                case "List":
                    RectTransform.GetComponent<GSUIList>().interactable = false;
                    Disabled = true;
                    return true;
                case "Checkbox":
                    RectTransform.GetComponent<Toggle>().interactable = false;
                    Disabled = true;
                    return true;
                case "Combobox":
                    RectTransform.GetComponentInChildren<Dropdown>().interactable = false;
                    Disabled = true;
                    return true;
                case "Button":
                    RectTransform.GetComponentInChildren<Button>().interactable = false;
                    Disabled = true;
                    return true;
                case "Input":
                    RectTransform.GetComponentInChildren<InputField>().interactable = false;
                    Disabled = true;
                    return true;
                case "Slider":
                    RectTransform.GetComponentInChildren<Slider>().interactable = false;
                    Disabled = true;
                    return true;
                case "RangeSlider":
                    RectTransform.GetComponentInChildren<GSUIRangeSlider>().interactable = false;
                    Disabled = true;
                    return true;
            }

            return false;
        }

        public bool Enable()
        {
            if (!Disabled)
            {
                Warn("Trying to enable UI Element that is already enabled");
                return false;
            }

            var c = RectTransform.GetComponentsInChildren<Image>();
            foreach (var i in c)
            {
                var id = i.GetInstanceID();
                i.color = colors[id];
            }

            switch (Type)
            {
                case "List":
                    RectTransform.GetComponent<GSUIList>().interactable = true;
                    Disabled = false;
                    return true;
                case "Checkbox":
                    RectTransform.GetComponent<Toggle>().interactable = true;
                    Disabled = false;
                    return true;
                case "Combobox":
                    RectTransform.GetComponentInChildren<Dropdown>().interactable = true;
                    Disabled = false;
                    return true;
                case "Button":
                    RectTransform.GetComponentInChildren<Button>().interactable = true;
                    Disabled = false;
                    return true;
                case "Input":
                    RectTransform.GetComponentInChildren<InputField>().interactable = true;
                    Disabled = false;
                    return true;
                case "Slider":
                    RectTransform.GetComponentInChildren<Slider>().interactable = true;
                    Disabled = false;
                    return true;
                case "RangeSlider":
                    RectTransform.GetComponentInChildren<GSUIRangeSlider>().interactable = true;
                    Disabled = false;
                    return true;
            }

            return false;
        }

        public void SetHint(string hint)
        {
            Hint = hint;
        }


        private static GSOptionCallback CreateIncrementCallback(float increment, GSUI instance, GSOptionCallback existingCallback)
        {
            return o =>
            {
                if (o.ToString().Split(':').Length > 1)
                {
                    FloatPair val = o;
                    val.low = val.low - val.low % increment;
                    val.high = val.high - val.high % increment;
                    if (val.high > ((GSRangeSliderConfig)instance.Data).maxValue) val.high = ((GSRangeSliderConfig)instance.Data).maxValue;
                    instance.Set(val);
                    existingCallback(val);
                }
                else
                {
                    var value = 0.1f;
                    if (!float.TryParse(o.ToString(), out value))
                    {
                        Error($"Failed to parse increment {o} for slider {instance.Label}");
                    }
                    else
                    {
                        var cfg = (GSSliderConfig)instance.Data;
                        if (value >= cfg.maxValue - increment / 2)
                        {
                            var iMax = cfg.maxValue;
                            instance.Set(iMax);
                            existingCallback(iMax);
                        }
                        else
                        {
                            existingCallback(value);
                        }
                    }
                }
            };
        }

        private GSOptionCallback CreateDefaultCallback(GSOptionCallback callback = null)
        {
            return o =>
            {
                plugin.OnUpdate(key, o);
                if (callback is GSOptionCallback) callback(o);
            };
        }

        private GSOptionPostfix CreateDefaultPostfix()
        {
            return () =>
            {
                if (plugin != null)
                {
                    var value = plugin.Export().Get(key);
                    if (value == null) value = DefaultValue;

                    if (value != null) Set(value);
                    else Log($"Caution: Preference value for {Label} not found.");
                }
            };
        }

        public static GSUI Separator()
        {
            return new GSUI(null, null, null, "Separator", null, null, null, null);
        }
    }

    public class GSUIGroupConfig
    {
        public GSOptionCallback callback = null;
        public bool collapsible = true;
        public bool defaultValue;
        public bool header = true;
        public List<GSUI> options = new();

        public GSUIGroupConfig(List<GSUI> options, bool header, bool collapsible, bool defaultValue = false)
        {
            this.options = options;
            this.header = header;
            this.collapsible = collapsible;
            this.defaultValue = defaultValue;
        }

        public GSUIGroupConfig()
        {
        }
    }

    public struct GSSliderConfig
    {
        public float minValue;
        public float maxValue;
        public float defaultValue;
        public string negativeLabel;

        public GSSliderConfig(float minValue, float value, float maxValue, string negativeLabel = "")
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            defaultValue = value;
            this.negativeLabel = negativeLabel;
        }
    }

    public struct GSRangeSliderConfig
    {
        public GSOptionCallback callbackLow;
        public GSOptionCallback callbackHigh;
        public float minValue;
        public float maxValue;
        public float defaultLowValue;
        public float defaultHighValue;
        public FloatPair defaultValue => new(defaultLowValue, defaultHighValue);

        public GSRangeSliderConfig(float minValue, float lowValue, float highValue, float maxValue, GSOptionCallback callbackLow = null, GSOptionCallback callbackHigh = null)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            defaultLowValue = lowValue;
            defaultHighValue = highValue;
            this.callbackHigh = callbackHigh;
            this.callbackLow = callbackLow;
        }
    }
}