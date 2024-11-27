using MaterialUI;
using UnityEngine;
using UnityEngine.UIElements;

public class PBROverlaySelector : VisualElement
{
    public Label label;
    public PBROverlaySelector()
    {
        this.AddClass("pbr-overlay");

        var close = this.CreateChild<Button>("close");
        close.style.width = 100;
        close.style.height = 100;
        close.style.backgroundColor = new Color(1, 1, 1);
        close.text = "X";
        close.style.color = new Color(0, 0, 0);
        close.clicked += () => this.style.display = DisplayStyle.None;

        label = this.CreateChild<Label>();
        label.text = "Container";

        var center = this.CreateChild("center");

        int numImages = 7;

        for (int i = 0; i < numImages; i++)
        {
            var imageContainer = center.CreateChild("image-container");
            var image = imageContainer.CreateChild<Image>();
        }
    }
}
