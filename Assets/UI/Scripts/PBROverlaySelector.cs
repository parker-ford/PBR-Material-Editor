using System.Collections.Generic;
using MaterialUI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PBROverlaySelector : VisualElement
{
    public Label label;

    private VisualElement center;
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

        center = this.CreateChild("center");
    }

    public void SetImages(List<string> paths)
    {
        foreach (string path in paths)
        {
            Debug.Log(path);
            var button = center.CreateChild<Button>("image-container");
            button.style.backgroundImage = Loader.LoadImage(path);
        }
    }
}
