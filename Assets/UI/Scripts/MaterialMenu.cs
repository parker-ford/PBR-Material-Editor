using MaterialUI;
using UnityEngine;
using UnityEngine.UIElements;

public class MaterialMenu : VisualElement
{
    public MaterialDropdown diffuseModelDropdown;
    public MaterialDropdown normalDistributionModelDropdown;
    public MaterialDropdown geometryAttenuationModelDropdown;
    public MaterialDropdown debugViewTypeDropdown;
    public MaterialColorPickerRGB diffuseColorPicker;
    public MaterialSlider subsurfaceSlider;
    public MaterialSlider roughnessSlider;
    public MaterialSlider reflectanceSlider;
    public MaterialSlider displacementMapStrengthSlider;
    public MaterialSlider normalMapStrengthSlider;
    public MaterialSlider sheenSlider;
    public MaterialSlider sheenTintSlider;
    public MaterialSlider anisotropicSlider;
    public MaterialSlider clearcoatSlider;
    public MaterialSlider clearcoatGlossSlider;
    public MaterialSlider metallicSlider;
    public MaterialToggle diffuseMapToggle;
    public MaterialToggle normalMapToggle;
    public MaterialToggle displacementMapToggle;
    public MaterialToggle roughnessMapToggle;
    public MaterialToggle environmentLightingToggle;
    public MaterialToggle objectRotateToggle;
    public MaterialToggle lightRotateToggle;
    public MaterialColorPickerRGB lightColorPicker;
    public MaterialSlider lightIntensitySlider;
    public MaterialColorPickerRGB backgroundColorPicker;



    public MaterialMenu()
    {
        this.AddClass("material-menu");

        var materialEditorMenu = this.CreateChild<Foldout>("material-menu-foldout");
        materialEditorMenu.text = "Material Properties";
        // materialEditorMenu.RemoveManipulator(materialEditorMenu);

        // materialEditorMenu.unregisterCallback<KeyDownEvent>(materialEditorMenu.defaultKeyboardControl);
        // materialEditorMenu.UnregisterCallback<KeyDownEvent>((Event) => Debug.Log("hello"));
        // Foldout foldout;

        var materialScrollView = materialEditorMenu.CreateChild<ScrollView>("material-scrollview");

        /*
        *   Material Models
        */
        var materialModels = materialScrollView.CreateChild<MaterialPropertyGroup>();
        materialModels.label.text = "Texture Properties";

        diffuseModelDropdown = materialModels.CreateChild<MaterialDropdown>();
        diffuseModelDropdown.label.text = "Diffuse Model";

        normalDistributionModelDropdown = materialModels.CreateChild<MaterialDropdown>();
        normalDistributionModelDropdown.label.text = "Normal Distribution Model";

        geometryAttenuationModelDropdown = materialModels.CreateChild<MaterialDropdown>();
        geometryAttenuationModelDropdown.label.text = "Geometry Attenuation Model";


        /*
        *   Specular Parameters
        */
        var specularParamters = materialScrollView.CreateChild<MaterialPropertyGroup>();
        specularParamters.label.text = "Specular Properties";

        roughnessSlider = specularParamters.CreateChild<MaterialSlider>();
        roughnessSlider.label.text = "Roughness";

        reflectanceSlider = specularParamters.CreateChild<MaterialSlider>();
        reflectanceSlider.label.text = "Reflectance";

        clearcoatSlider = specularParamters.CreateChild<MaterialSlider>();
        clearcoatSlider.label.text = "Clearcoat";

        clearcoatGlossSlider = specularParamters.CreateChild<MaterialSlider>();
        clearcoatGlossSlider.label.text = "Clearcoat Gloss";

        // anisotropicSlider = specularProperties.CreateChild<MaterialSlider>();
        // anisotropicSlider.label.text = "Anisotropic";

        /*
        *   Diffuse Parameters
        */

        var diffuseProperties = materialScrollView.CreateChild<MaterialPropertyGroup>();
        diffuseProperties.label.text = "Diffuse Properties";

        subsurfaceSlider = diffuseProperties.CreateChild<MaterialSlider>();
        subsurfaceSlider.label.text = "Subsurface";

        sheenSlider = diffuseProperties.CreateChild<MaterialSlider>();
        sheenSlider.label.text = "Sheen";

        sheenTintSlider = diffuseProperties.CreateChild<MaterialSlider>();
        sheenTintSlider.label.text = "Sheen Tint";

        metallicSlider = diffuseProperties.CreateChild<MaterialSlider>();
        metallicSlider.label.text = "Metallic";

        diffuseColorPicker = diffuseProperties.CreateChild<MaterialColorPickerRGB>();
        diffuseColorPicker.label.text = "Diffuse Color";

        /*
        *   Lighting Properties
        */
        var lightingProperties = materialScrollView.CreateChild<MaterialPropertyGroup>();
        lightingProperties.label.text = "Lighting Settings";

        environmentLightingToggle = lightingProperties.CreateChild<MaterialToggle>();
        environmentLightingToggle.label.text = "Environment Lighting";

        lightRotateToggle = lightingProperties.CreateChild<MaterialToggle>();
        lightRotateToggle.label.text = "Rotate Light";

        objectRotateToggle = lightingProperties.CreateChild<MaterialToggle>();
        objectRotateToggle.label.text = "Rotate Object";

        lightIntensitySlider = lightingProperties.CreateChild<MaterialSlider>();
        lightIntensitySlider.label.text = "Light Intensity";
        lightIntensitySlider.slider.highValue = 5;

        lightColorPicker = lightingProperties.CreateChild<MaterialColorPickerRGB>();
        lightColorPicker.label.text = "Light Color";

        backgroundColorPicker = lightingProperties.CreateChild<MaterialColorPickerRGB>();
        backgroundColorPicker.label.text = "Background Color";

        /*
        *   Texture Properties
        */
        var textureProperties = materialScrollView.CreateChild<MaterialPropertyGroup>();
        textureProperties.label.text = "Texture Properties";

        normalMapStrengthSlider = textureProperties.CreateChild<MaterialSlider>();
        normalMapStrengthSlider.label.text = "Normal Strength";

        displacementMapStrengthSlider = textureProperties.CreateChild<MaterialSlider>();
        displacementMapStrengthSlider.label.text = "Displacement Strength";


        /*
        *   Debug Views
        */
        var debugView = materialScrollView.CreateChild<MaterialPropertyGroup>();
        debugView.label.text = "Debug View";

        debugViewTypeDropdown = debugView.CreateChild<MaterialDropdown>();
        debugViewTypeDropdown.label.text = "Type";

        diffuseMapToggle = debugView.CreateChild<MaterialToggle>();
        diffuseMapToggle.label.text = "Use Diffuse Map";

        normalMapToggle = debugView.CreateChild<MaterialToggle>();
        normalMapToggle.label.text = "Use Normal Map";

        displacementMapToggle = debugView.CreateChild<MaterialToggle>();
        displacementMapToggle.label.text = "Use Displacement Map";

        roughnessMapToggle = debugView.CreateChild<MaterialToggle>();
        roughnessMapToggle.label.text = "Use Roughness Map";

    }

    public void SetValues(
        float reflectance,
        float roughness,
        float subsurface,
        Color diffuseColor,
        int ndf,
        int geometry,
        int diffuse,
        int debug,
        bool useDiffuseMap,
        bool useNormalMap,
        bool useDisplacementMap,
        bool useRoughnessMap,
        float normalMapStrength,
        float displacementMapStrength,
        float sheen,
        float sheenTint,
        float clearcoat,
        float clearcoatGloss,
        float metallic,
        bool useEnvironmentLighting,
        bool rotateLight,
        bool rotateObject,
        Color lightColor,
        float lightIntensity,
        Color backgroundColor
    )
    {
        reflectanceSlider.SetCurrentValue(reflectance);
        roughnessSlider.SetCurrentValue(roughness);
        subsurfaceSlider.SetCurrentValue(subsurface);
        diffuseColorPicker.SetCurrentValue(diffuseColor);
        normalDistributionModelDropdown.SetCurrentValue(ndf);
        geometryAttenuationModelDropdown.SetCurrentValue(geometry);
        diffuseModelDropdown.SetCurrentValue(diffuse);
        debugViewTypeDropdown.SetCurrentValue(debug);
        diffuseMapToggle.SetCurrentValue(useDiffuseMap);
        normalMapToggle.SetCurrentValue(useNormalMap);
        displacementMapToggle.SetCurrentValue(useDisplacementMap);
        roughnessMapToggle.SetCurrentValue(useRoughnessMap);
        normalMapStrengthSlider.SetCurrentValue(normalMapStrength);
        displacementMapStrengthSlider.SetCurrentValue(displacementMapStrength);
        sheenSlider.SetCurrentValue(sheen);
        sheenTintSlider.SetCurrentValue(sheenTint);
        clearcoatSlider.SetCurrentValue(clearcoat);
        clearcoatGlossSlider.SetCurrentValue(clearcoatGloss);
        metallicSlider.SetCurrentValue(metallic);
        environmentLightingToggle.SetCurrentValue(useEnvironmentLighting);
        lightRotateToggle.SetCurrentValue(rotateLight);
        objectRotateToggle.SetCurrentValue(rotateObject);
        lightColorPicker.SetCurrentValue(lightColor);
        lightIntensitySlider.SetCurrentValue(lightIntensity);
        backgroundColorPicker.SetCurrentValue(backgroundColor);
    }

    public void SetAllEnabled(bool state)
    {
        foreach (var child in this.Children())
        {
            child.SetEnabled(state);
        }
    }


}


