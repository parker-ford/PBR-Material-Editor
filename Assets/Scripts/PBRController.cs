using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PBRController : MonoBehaviour
{

    public enum NormalDistributionFunction
    {
        BlinnPhong = 0,
        Beckman = 1,
        GGX = 2,
    }

    public enum GeometryAttenuationFunction
    {
        Beckman = 0,
        GGX = 1,
        SchlickGGX = 2
    }

    public enum Diffuse
    {
        Lambert = 0,
        Hammon = 1,
        Disney = 2
    }

    public enum DebugView
    {
        None = 0,
        NormalDistributionFunction = 1,
        GeometryAttenuation = 2,
        Fresnel = 3,
        Diffuse = 4,
        Specular = 5,
        DiffuseMap = 6,
        NormalMap = 7,
        DisplacementMap = 8,
        RoughnessMap = 9,
    }

    public PBRView view;
    public Shader pbrShader;
    public Light sun;
    public ModelObject defaultModelObject;
    public TextureObject defaultTextureObject;
    public NormalDistributionFunction defaultNDF;
    public GeometryAttenuationFunction defaultGeometry;
    public Diffuse defaultDiffuse;
    public DebugView defaultDebug;
    [Range(0, 1)] public float defaultReflectance;
    [Range(0, 1)] public float defaultRoughness;
    [Range(0, 1)] public float defaultSubsurface;
    public Color defaultDiffuseColor;
    public float defaultDisplacementStrength;
    public float defaultNormalMapStrength;
    public bool defaultUseDisplacementMap;

    private GameObject model;
    private Material material;
    private ModelObject modelObject;
    private TextureObject textureObject;
    private float reflectance;
    private float roughness;
    private float subsurface;
    private Color diffuseColor;
    private NormalDistributionFunction ndf;
    private GeometryAttenuationFunction geometry;
    private Diffuse diffuse;
    private DebugView debug;
    private float displacementMapStrength;
    private float normalMapStrength;
    private bool useDisplacementMap;
    private List<ModelObject> modelObjects;
    private List<TextureObject> textureObjects;


    void Start()
    {
        modelObjects = Loader.LoadScriptableObjects<ModelObject>();
        textureObjects = Loader.LoadScriptableObjects<TextureObject>();

        roughness = defaultRoughness;
        reflectance = defaultReflectance;
        subsurface = defaultSubsurface;
        diffuseColor = defaultDiffuseColor;
        modelObject = defaultModelObject;
        textureObject = defaultTextureObject;
        displacementMapStrength = defaultDisplacementStrength;
        normalMapStrength = defaultNormalMapStrength;
        useDisplacementMap = defaultUseDisplacementMap;
        material = new Material(pbrShader);
        ndf = defaultNDF;
        geometry = defaultGeometry;
        diffuse = defaultDiffuse;
        debug = defaultDebug;

        InstantiateModel();
        UpdateMaterialParameters();
        SetTextureMaps();
        InstantiateUI();

    }

    void InstantiateUI()
    {
        view.GenerateUI();

        // Setting Dropdown Choices
        view.materialMenu.diffuseModelDropdown.SetDropdownChoices(Enum.GetNames(typeof(Diffuse)));
        view.materialMenu.normalDistributionModelDropdown.SetDropdownChoices(Enum.GetNames(typeof(NormalDistributionFunction)));
        view.materialMenu.geometryAttenuationModelDropdown.SetDropdownChoices(Enum.GetNames(typeof(GeometryAttenuationFunction)));
        view.materialMenu.debugViewTypeDropdown.SetDropdownChoices(Enum.GetNames(typeof(DebugView)));

        // Setting Overlay Choices
        view.modelOverlay.SetImages(modelObjects.Select(obj => obj.GetPath()).ToList());
        view.modelOverlay.OnOverlaySelection += (index) =>
        {
            modelObject = modelObjects[index];
            InstantiateModel();
        };

        view.textureOverlay.SetImages(textureObjects.Select(obj => obj.displayImage).ToList());
        view.textureOverlay.OnOverlaySelection += (index) =>
        {
            textureObject = textureObjects[index];
            SetTextureMaps();
        };


        // Set Default UI Values
        view.materialMenu.SetValues(
            reflectance,
            roughness,
            subsurface,
            diffuseColor,
            (int)ndf,
            (int)geometry,
            (int)diffuse,
            (int)debug
        );

        // Bind Sliders
        BindSlider(view.materialMenu.displacementMapStrengthSlider, newValue => displacementMapStrength = newValue);
        BindSlider(view.materialMenu.normalMapStrengthSlider, newValue => normalMapStrength = newValue);
        BindSlider(view.materialMenu.reflectanceSlider, newValue => reflectance = newValue);
        BindSlider(view.materialMenu.roughnessSlider, newValue => roughness = newValue);
        BindSlider(view.materialMenu.subsurfaceSlider, newValue => subsurface = newValue);

        // Bind Color Pickers
        BindColorPicker(view.materialMenu.diffuseColorPicker, newColor => diffuseColor = newColor);

        // Bind Dropdown
        BindDropdown(view.materialMenu.normalDistributionModelDropdown, newValue => ndf = (NormalDistributionFunction)newValue);
        BindDropdown(view.materialMenu.geometryAttenuationModelDropdown, newValue => geometry = (GeometryAttenuationFunction)newValue);
        BindDropdown(view.materialMenu.diffuseModelDropdown, newValue => diffuse = (Diffuse)newValue);
        BindDropdown(view.materialMenu.debugViewTypeDropdown, newValue => debug = (DebugView)newValue);

        // Bind Toggle
        BindToggle(view.materialMenu.displacementMapToggle, newValue => useDisplacementMap = newValue);
    }

    void BindToggle(MaterialToggle toggle, Action<bool> action)
    {
        toggle.OnToggleChanged += newValue =>
        {
            action(newValue);
            UpdateMaterialParameters();
        };
    }

    void BindSlider(MaterialSlider slider, Action<float> action)
    {
        slider.OnMaterialSliderChanged += newValue =>
        {
            action(newValue);
            UpdateMaterialParameters();
        };
    }
    void BindColorPicker(MaterialColorPickerRGB colorPicker, Action<Color> action)
    {
        colorPicker.OnColorChanged += newColor =>
        {
            action(newColor);
            UpdateMaterialParameters();
        };
    }

    void BindDropdown(MaterialDropdown dropdown, Action<int> action)
    {
        dropdown.OnChoiceChanged += newValue =>
        {
            action(newValue);
            UpdateMaterialParameters();
        };
    }

    void InstantiateModel()
    {
        Destroy(model);
        model = new GameObject("Model");
        model.AddComponent<MeshFilter>().mesh = modelObject.GetMesh();
        modelObject.SetToTransform(model.transform);
        model.AddComponent<MeshRenderer>().material = material;
    }

    void SetTextureMaps()
    {
        if (textureObject.diffuseMap)
        {
            material.SetTexture("_DiffuseMap", textureObject.diffuseMap);
            material.SetInt("_DiffuseMapSet", 1);
        }
        else
        {
            material.SetInt("_DiffuseMapSet", 0);
        }

        if (textureObject.normalMap)
        {
            material.SetTexture("_NormalMap", textureObject.normalMap);
            material.SetInt("_NormalMapSet", 1);
        }
        else
        {
            material.SetInt("_NormalMapSet", 0);
        }

        if (textureObject.displacementMap)
        {
            material.SetTexture("_DisplacementMap", textureObject.displacementMap);
            material.SetInt("_DisplacementMapSet", 1);
        }
        else
        {
            material.SetInt("_DisplacementMapSet", 0);
        }

        if (textureObject.roughnessMap)
        {
            material.SetTexture("_RoughnessMap", textureObject.roughnessMap);
            material.SetInt("_RoughnessMapSet", 1);
        }
        else
        {
            material.SetInt("_RoughnessMapSet", 0);
        }
    }

    void UpdateMaterialParameters()
    {
        if (!material)
        {
            Debug.LogError("PBR Material is null");
            return;
        }

        material.SetFloat("_Reflectance", reflectance);
        material.SetFloat("_Roughness", roughness);
        material.SetFloat("_Subsurface", subsurface);
        material.SetColor("_DiffuseColor", diffuseColor);

        material.SetInt("_NDF", (int)ndf);
        material.SetInt("_Geometry", (int)geometry);
        material.SetInt("_Diffuse", (int)diffuse);
        material.SetInt("_DebugView", (int)debug);

        material.SetColor("_LightColor", sun.color);
        material.SetVector("_LightDirection", -sun.transform.forward);
        material.SetFloat("_LightIntensity", sun.intensity);

        material.SetFloat("_DisplacementStrength", displacementMapStrength);
        material.SetFloat("_NormalStrength", normalMapStrength);

        material.SetInt("_UseDisplacementMap", useDisplacementMap ? 1 : 0);
    }

    void OnDisable()
    {
        Destroy(material);
        Destroy(model);
    }
}
