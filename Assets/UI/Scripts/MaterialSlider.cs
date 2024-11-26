using MaterialUI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class MaterialSlider : VisualElement
{
    public Label label;
    public Slider slider;
    public FloatField floatField;
    private float currentValue;

    public event Action<float> OnMaterialSliderChanged = delegate { };

    public MaterialSlider()
    {
        this.AddClass("material-slider", "material-property");

        var left = this.CreateChild("left");
        var right = this.CreateChild("right");

        label = left.CreateChild<Label>();
        label.text = "Property name";

        slider = right.CreateChild<Slider>();
        slider.lowValue = 0;
        slider.highValue = 1;
        slider.RegisterValueChangedCallback(OnSliderChanged);

        floatField = right.CreateChild<FloatField>();
        floatField.maxLength = 4;
        floatField.isDelayed = true;
        floatField.RegisterValueChangedCallback(OnFloatFieldChanged);

    }

    private void OnSliderChanged(ChangeEvent<float> evt)
    {
        currentValue = evt.newValue;
        OnMaterialSliderChanged.Invoke(currentValue);

        floatField.UnregisterValueChangedCallback(OnFloatFieldChanged);
        floatField.value = currentValue;
        floatField.RegisterValueChangedCallback(OnFloatFieldChanged);
    }

    private void OnFloatFieldChanged(ChangeEvent<float> evt)
    {
        currentValue = evt.newValue;
        OnMaterialSliderChanged.Invoke(currentValue);

        slider.UnregisterValueChangedCallback(OnSliderChanged);
        slider.value = Mathf.Clamp(currentValue, 0.0f, 1.0f);
        slider.RegisterValueChangedCallback(OnSliderChanged);
    }

    public float GetCurrentValue()
    {
        return currentValue;
    }

    public void SetCurrentValue(float newValue)
    {
        currentValue = newValue;

        slider.UnregisterValueChangedCallback(OnSliderChanged);
        floatField.UnregisterValueChangedCallback(OnFloatFieldChanged);

        slider.value = Mathf.Clamp(newValue, 0.0f, 1.0f);
        floatField.value = newValue;

        slider.RegisterValueChangedCallback(OnSliderChanged);
        floatField.RegisterValueChangedCallback(OnFloatFieldChanged);
    }

}
