using System;
using MaterialUI;
using UnityEngine;
using UnityEngine.UIElements;

public class MaterialColorPickerRGB : VisualElement
{
    public Label label;
    public Box colorView;
    public MaterialSlider rSlider;
    public MaterialSlider bSlider;
    public MaterialSlider gSlider;

    private Color currentColor;

    public event Action<Color> OnColorChanged = delegate { };

    public MaterialColorPickerRGB()
    {
        this.AddClass("material-color-picker-rgb");

        currentColor = new Color(1, 1, 1);

        var upper = this.CreateChild("upper");
        var lower = this.CreateChild("lower");

        label = upper.CreateChild<Label>();
        label.text = "Color";

        colorView = upper.CreateChild<Box>();
        colorView.style.backgroundColor = currentColor;

        rSlider = lower.CreateChild<MaterialSlider>();
        rSlider.label.text = "R";
        rSlider.SetCurrentValue(1);
        rSlider.OnMaterialSliderChanged += UpdateColorValueRed;

        gSlider = lower.CreateChild<MaterialSlider>();
        gSlider.label.text = "G";
        gSlider.SetCurrentValue(1);
        gSlider.OnMaterialSliderChanged += UpdateColorValueGreen;

        bSlider = lower.CreateChild<MaterialSlider>();
        bSlider.label.text = "B";
        bSlider.SetCurrentValue(1);
        bSlider.OnMaterialSliderChanged += UpdateColorValueBlue;
    }

    ~MaterialColorPickerRGB()
    {
        rSlider.OnMaterialSliderChanged -= UpdateColorValueRed;
        bSlider.OnMaterialSliderChanged -= UpdateColorValueBlue;
        gSlider.OnMaterialSliderChanged -= UpdateColorValueGreen;
    }

    void UpdateColorValueRed(float newValue)
    {
        currentColor.r = newValue;
        UpdateColorView();
    }

    void UpdateColorValueGreen(float newValue)
    {
        currentColor.g = newValue;
        UpdateColorView();
    }

    void UpdateColorValueBlue(float newValue)
    {
        currentColor.b = newValue;
        UpdateColorView();
    }

    void UpdateColorView()
    {
        colorView.style.backgroundColor = currentColor;
        OnColorChanged.Invoke(currentColor);
    }


}
