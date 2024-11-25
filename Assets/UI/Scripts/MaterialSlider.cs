using MaterialUI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MaterialSlider : VisualElement
{
    public Label label;
    public Slider slider;
    public FloatField floatField;
    private float currentValue;
    public MaterialSlider()
    {
        this.AddClass("material-slider");

        label = this.CreateChild<Label>();
        label.text = "Property name";

        slider = this.CreateChild<Slider>();
        slider.lowValue = 0;
        slider.highValue = 1;
        slider.RegisterValueChangedCallback(OnSliderChanged);

        floatField = this.CreateChild<FloatField>();
        floatField.maxLength = 4;
        floatField.isDelayed = true;
        floatField.RegisterValueChangedCallback(OnFloatFieldChanged);

    }

    private void OnSliderChanged(ChangeEvent<float> evt)
    {
        currentValue = evt.newValue;

        floatField.UnregisterValueChangedCallback(OnFloatFieldChanged);
        floatField.value = currentValue;
        floatField.RegisterValueChangedCallback(OnFloatFieldChanged);
    }

    private void OnFloatFieldChanged(ChangeEvent<float> evt)
    {
        currentValue = evt.newValue;

        slider.UnregisterValueChangedCallback(OnSliderChanged);
        slider.value = Mathf.Clamp(currentValue, 0.0f, 1.0f);
        slider.RegisterValueChangedCallback(OnSliderChanged);
    }

}
