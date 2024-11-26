using MaterialUI;
using UnityEngine;
using UnityEngine.UIElements;

public class MaterialMenu : VisualElement
{
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

        var diffuseModel = diffuseProperties.CreateChild<MaterialDropdown>();
        diffuseModel.label.text = "Diffuse Model";

        var baseColor = diffuseProperties.CreateChild<MaterialColorPickerRGB>();
        baseColor.label.text = "Base Color";

        var subsurface = diffuseProperties.CreateChild<MaterialSlider>();
        subsurface.label.text = "Subsurface";

        /*
        *   Specular Properties
        */
        var specularProperties = materialScrollView.CreateChild<MaterialPropertyGroup>();
        specularProperties.label.text = "Specular Properties";

        var normalDistributionModel = specularProperties.CreateChild<MaterialDropdown>();
        normalDistributionModel.label.text = "Normal Distribution Model";

        var geometryAttenuationModel = specularProperties.CreateChild<MaterialDropdown>();
        geometryAttenuationModel.label.text = "Geometry Attenuation Model";

        var roughness = specularProperties.CreateChild<MaterialSlider>();
        roughness.label.text = "Roughness";

        var reflectance = specularProperties.CreateChild<MaterialSlider>();
        reflectance.label.text = "Reflectance";

        /*
        *   Debug Views
        */
        var debugView = materialScrollView.CreateChild<MaterialPropertyGroup>();
        debugView.label.text = "Debug View";

        var debugViewType = debugView.CreateChild<MaterialDropdown>();
        debugViewType.label.text = "Type";
    }

}


