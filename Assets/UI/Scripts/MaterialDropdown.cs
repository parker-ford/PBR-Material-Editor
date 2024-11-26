using MaterialUI;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class MaterialDropdown : VisualElement
{
    public Label label;
    public DropdownField dropdown;
    public MaterialDropdown()
    {
        this.AddClass("material-dropdown");

        label = this.CreateChild<Label>();
        label.text = "Property Name";

        dropdown = this.CreateChild<DropdownField>();
        dropdown.choices = new List<string> { "options 1" };
    }
}
