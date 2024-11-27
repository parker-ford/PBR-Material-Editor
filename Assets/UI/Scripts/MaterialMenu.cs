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


    public MaterialMenu()
    {
        this.AddClass("material-menu");

        var materialEditorMenu = this.CreateChild<Foldout>("material-menu-foldout");
        materialEditorMenu.text = "Material Properties";

        var materialScrollView = materialEditorMenu.CreateChild<ScrollView>();


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
    }

    public void SetValues(
        float reflectance,
        float roughness,
        float subsurface,
        Color diffuseColor,
        int ndf,
        int geometry,
        int diffuse,
        int debug
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
    }


}


