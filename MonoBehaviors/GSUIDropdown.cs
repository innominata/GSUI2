using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static GS.GS;

namespace GS
{
    public class GSUIDropdown : MonoBehaviour
    {
        public Dropdown _dropdown;
        public Text _labelText;
        public Text _hintText;
        public GSOptionCallback OnChange;

        public List<string> Items
        {
            get => _dropdown.options.Select(option => option.text).ToList();
            set => _dropdown.options = value.Select(s => new Dropdown.OptionData { text = s }).ToList();
        }

        public string Hint
        {
            get => _hintText.text;
            set => _hintText.text = value;
        }

        public string Label
        {
            get => _labelText.text;
            set => _labelText.text = value;
        }

        public int Value
        {
            get
            {
                if (Items.Count < 1 || Items.Count >= _dropdown.value)
                {
                    Warn($"Index out of bounds: {Label} {_dropdown.value} {Items.Count}");
                    return -1;
                }

                return _dropdown.value;
            }
            set =>
                // GS2.Warn("Setting Value to " + value + "/" +Items.Count) ; 
                _dropdown.value = value;
        }

        public void OnValueChange(int value)
        {
            if (value < 0 || value >= Items.Count)
            {
                Warn($"Index out of bounds: {Label} {value}");
                return;
            }

            OnChange?.Invoke(value);
        }

        public void initialize(GSUI options)
        {
            Items = options.Data as List<string>;
            OnChange = options.callback;
            Label = options.Label;
            Hint = options.Hint;
        }
    }
}