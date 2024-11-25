using MaterialUI;
using UnityEngine;
using UnityEngine.UIElements;

public class MaterialPropertyGroup : VisualElement
{
    public Label label;
    public MaterialPropertyGroup()
    {
        label = this.CreateChild<Label>();
        label.text = "Material Property";
    }
}
