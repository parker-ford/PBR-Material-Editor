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
    public MaterialToggle diffuseMapToggle;
    public MaterialToggle normalMapToggle;
    public MaterialToggle displacementMapToggle;
    public MaterialToggle roughnessMapToggle;


    public MaterialMenu()
    {
        this.AddClass("material-menu");

        var materialEditorMenu = this.CreateChild<Foldout>("material-menu-foldout");
        materialEditorMenu.text = "Material Properties";

        var materialScrollView = materialEditorMenu.CreateChild<ScrollView>();


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
        *   Diffuse Properties
        */
        var diffuseProperties = materialScrollView.CreateChild<MaterialPropertyGroup>();
        diffuseProperties.label.text = "Diffuse Properties";

        diffuseModelDropdown = diffuseProperties.CreateChild<MaterialDropdown>();
        diffuseModelDropdown.label.text = "Diffuse Model";

        diffuseColorPicker = diffuseProperties.CreateChild<MaterialColorPickerRGB>();
        diffuseColorPicker.label.text = "Diffuse Color";

        subsurfaceSlider = diffuseProperties.CreateChild<MaterialSlider>();
        subsurfaceSlider.label.text = "Subsurface";

        /*
        *   Specular Properties
        */
        var specularProperties = materialScrollView.CreateChild<MaterialPropertyGroup>();
        specularProperties.label.text = "Specular Properties";

        normalDistributionModelDropdown = specularProperties.CreateChild<MaterialDropdown>();
        normalDistributionModelDropdown.label.text = "Normal Distribution Model";

        geometryAttenuationModelDropdown = specularProperties.CreateChild<MaterialDropdown>();
        geometryAttenuationModelDropdown.label.text = "Geometry Attenuation Model";

        roughnessSlider = specularProperties.CreateChild<MaterialSlider>();
        roughnessSlider.label.text = "Roughness";

        reflectanceSlider = specularProperties.CreateChild<MaterialSlider>();
        reflectanceSlider.label.text = "Reflectance";

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
        float displacementMapStrength
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
    }


}


