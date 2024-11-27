using MaterialUI;
using UnityEngine;
using UnityEngine.UIElements;

public class PBROverlaySelector : VisualElement
{
    public PBROverlaySelector()
    {
        this.AddClass("pbr-overlay");

        var center = this.CreateChild("center");

        int numImages = 7;

        for (int i = 0; i < numImages; i++)
        {
            var imageContainer = center.CreateChild("image-container");
            var image = imageContainer.CreateChild<Image>();
        }
    }
}
