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

    public event Action<Color> OnColorChanged;

    public MaterialColorPickerRGB()
    {
        this.AddClass("material-color-picker-rgb");

        currentColor = new Color(1, 1, 1);

        label = this.CreateChild<Label>();
        label.text = "Color";

        colorView = this.CreateChild<Box>();
        colorView.style.backgroundColor = currentColor;

        rSlider = this.CreateChild<MaterialSlider>();
        rSlider.label.text = "R";
        rSlider.SetCurrentValue(1);
        rSlider.OnMaterialSliderChanged += UpdateColorValueRed;

        gSlider = this.CreateChild<MaterialSlider>();
        gSlider.label.text = "G";
        gSlider.SetCurrentValue(1);
        gSlider.OnMaterialSliderChanged += UpdateColorValueGreen;

        bSlider = this.CreateChild<MaterialSlider>();
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
