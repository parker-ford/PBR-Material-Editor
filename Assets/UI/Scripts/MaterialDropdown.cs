using MaterialUI;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using System;

public class MaterialDropdown : VisualElement
{
    public Label label;
    public DropdownField dropdown;
    public Action<int> OnChoiceChanged = delegate { };
    public MaterialDropdown()
    {
        this.AddClass("material-dropdown", "material-property");

        var left = this.CreateChild("left");
        var right = this.CreateChild("right");

        label = left.CreateChild<Label>();
        label.text = "Property Name";

        dropdown = right.CreateChild<DropdownField>();
        dropdown.RegisterValueChangedCallback(evt => OnChoiceChanged(dropdown.choices.IndexOf(evt.newValue)));
    }

    public void SetDropdownChoices(string[] choices)
    {
        dropdown.choices = choices.ToList();
    }

    public void SetCurrentValue(int choice)
    {
        dropdown.value = dropdown.choices[choice];
    }
}
