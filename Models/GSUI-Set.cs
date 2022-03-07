using System.Collections.Generic;
using UnityEngine.UI;
using static GS.GS;

namespace GS
{
    public partial class GSUI
    {
        public bool Set(GSSliderConfig cfg)
        {
            if (RectTransform == null) return false;
            var slider = RectTransform.GetComponent<Slider>();
            if (slider == null) return false;
            slider.minValue = cfg.minValue >= 0 ? cfg.minValue : slider.minValue;
            slider.maxValue = cfg.maxValue >= 0 ? cfg.maxValue : slider.maxValue;
            slider.value = cfg.defaultValue >= 0 ? cfg.defaultValue : slider.value;
            return true;
        }

        public bool Set(Val o)
        {
            if (RectTransform == null)
                return false;
            switch (Type)
            {
                case "Selector":
                    RectTransform.GetComponent<GSUISelector>().value = o;
                    return true;
                case "RangeSlider":
                    FloatPair ff = o;
                    RectTransform.GetComponent<GSUIRangeSlider>().LowValue = ff.low;
                    RectTransform.GetComponent<GSUIRangeSlider>().HighValue = ff.high;
                    RectTransform.GetComponent<GSUIRangeSlider>().LowValue = ff.low;
                    return true;
                case "Slider":
                    RectTransform.GetComponent<GSUISlider>()._slider.value = o;
                    return true;
                case "Input":
                    if (o == null)
                    {
                        Error($"Failed to set input {Label} as value was null");
                        return false;
                    }

                    RectTransform.GetComponentInChildren<InputField>().text = o;
                    return true;
                case "Checkbox":
                    var toggle = RectTransform.GetComponent<GSUIToggle>();
                    if (toggle is null)
                    {
                        Error($"Failed to find Toggle for {Label}");
                        return false;
                    }

                    toggle.Value = o;
                    return true;
                case "Group":
                    var gtoggle = RectTransform.GetComponent<GSUIToggleList>();
                    if (gtoggle is null)
                    {
                        Error($"Failed to find Toggle for {Label}");
                        return false;
                    }

                    gtoggle.Value = o;
                    return true;
                case "Combobox":
                    var cb = RectTransform.GetComponentInChildren<Dropdown>();
                    if (cb is null)
                    {
                        Error($"Failed to find UICombobox for {Label}");
                        return false;
                    }

                    if (o > cb.options.Count - 1)
                    {
                        Error($"Failed to set {o} for combobox '{Label}': Value > Item Count");
                        return false;
                    }

                    comboDefault = o;
                    cb.value = o;
                    return true;
            }

            return false;
        }

        public bool SetItems(List<string> items)
        {
            if (Type != "Combobox")
            {
                Warn("Trying to Set Items on non Combobox UI Element");
                return false;
            }

            Data = items;
            if (RectTransform != null) RectTransform.GetComponent<GSUIDropdown>().Items = items;
            return true;
        }
    }
}