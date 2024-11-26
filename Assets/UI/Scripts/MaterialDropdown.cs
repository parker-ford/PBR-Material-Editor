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

        var left = this.CreateChild("left");
        var right = this.CreateChild("right");

        label = left.CreateChild<Label>();
        label.text = "Property Name";

        dropdown = right.CreateChild<DropdownField>();
        dropdown.choices = new List<string> { "options 1", "options 2", "options 3" };
    }
}
