using System;
using MaterialUI;
using UnityEngine;
using UnityEngine.UIElements;

public class MaterialToggle : VisualElement
{
    public Label label;
    public Toggle toggle;

    public Action<bool> OnToggleChanged = delegate { };
    public MaterialToggle()
    {
        this.AddClass("material-toggle", "material-property");

        var left = this.CreateChild("left");
        var right = this.CreateChild("right");

        label = left.CreateChild<Label>();
        label.text = "Toggle";

        toggle = right.CreateChild<Toggle>();

        toggle.RegisterValueChangedCallback(evt => OnToggleChanged.Invoke(evt.newValue));
    }

    public void SetCurrentValue(bool newValue)
    {
        toggle.value = newValue;
    }

}
