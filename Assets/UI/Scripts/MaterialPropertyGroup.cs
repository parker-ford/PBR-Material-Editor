using MaterialUI;
using UnityEngine;
using UnityEngine.UIElements;

public class MaterialPropertyGroup : VisualElement
{
    public Label label;
    public MaterialPropertyGroup()
    {
        this.AddClass("material-property-group");
        var upper = this.CreateChild("upper");
        label = upper.CreateChild<Label>("header");
        label.text = "Material Property";
    }
}
