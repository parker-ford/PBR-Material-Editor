using System;
using System.Collections.Generic;
using MaterialUI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PBROverlaySelector : VisualElement
{
    public Label label;
    private VisualElement center;
    public event Action<int> OnOverlaySelection = delegate { };
    public PBROverlaySelector()
    {
        this.AddClass("pbr-overlay");

        var overlayExit = this.CreateChild<Button>("overlay-exit");
        overlayExit.clicked += () => this.style.display = DisplayStyle.None;

        var close = overlayExit.CreateChild<Button>("close");
        close.text = "x";
        close.clicked += () => this.style.display = DisplayStyle.None;

        label = overlayExit.CreateChild<Label>();
        label.text = "Container";

        center = overlayExit.CreateChild<ScrollView>("center");
    }

    // public void SetImages(List<string> paths)
    // {
    //     int index = 0;
    //     VisualElement row = null;
    //     foreach (string path in paths)
    //     {
    //         if (index % 3 == 0)
    //         {
    //             row = center.CreateChild("overlay-row");
    //         }

    //         var button = row.CreateChild<Button>("image-container");
    //         button.style.backgroundImage = Loader.LoadImage(path);
    //         int localIndex = index;
    //         button.clicked += () =>
    //         {
    //             OnOverlaySelection.Invoke(localIndex);
    //             this.style.display = DisplayStyle.None;
    //         };
    //         index++;
    //     }
    // }

    public void SetImages(List<Texture2D> images, List<string> ids)
    {
        int index = 0;
        VisualElement row = null;
        foreach (Texture2D image in images)
        {
            if (index % 3 == 0)
            {
                row = center.CreateChild("overlay-row");
            }

            var button = row.CreateChild<Button>("image-container");
            var buttonLabel = button.CreateChild<Label>("image-container-label");
            buttonLabel.text = ids[index];
            if (image)
            {
                button.style.backgroundImage = image;
            }
            int localIndex = index;
            button.clicked += () =>
            {
                OnOverlaySelection.Invoke(localIndex);
                this.style.display = DisplayStyle.None;
            };
            index++;
        }
    }
}
