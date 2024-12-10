using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PBRController : MonoBehaviour
{

    public enum NormalDistributionFunction
    {
        GGX = 0,
        Beckman = 1,
        BlinnPhong = 2,
        // AnisotropicGTR = 3 // TODO: Properly implement this
    }

    public enum GeometryAttenuationFunction
    {
        SchlickGGX = 0,
        GGX = 1,
        Beckman = 2,
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
        Sheen = 6,
        Clearcoat = 7,
        ClearcoatGloss = 8,
        DiffuseMap = 60,
        NormalMap = 70,
        DisplacementMap = 80,
        RoughnessMap = 90,
        Normal = 100,
        Roughness = 110
    }

    public PBRView view;
    public Shader pbrShader;
    public Light sun;
    public ModelObject defaultModelObject;
    public TextureObject defaultTextureObject;
    public EnvironmentObject defaultEnvironmentObject;
    public NormalDistributionFunction defaultNDF;
    public GeometryAttenuationFunction defaultGeometry;
    public Diffuse defaultDiffuse;
    public DebugView defaultDebug;
    [Range(0, 1)] public float defaultReflectance;
    [Range(0, 1)] public float defaultRoughness;
    [Range(0, 1)] public float defaultSubsurface;
    [Range(0, 1)] public float defaultSheen;
    [Range(0, 1)] public float defaultSheenTint;
    [Range(0, 1)] public float defaultClearcoat;
    [Range(0, 1)] public float defaultClearcoatGloss;
    [Range(0, 1)] public float defaultMetallic;
    [Range(0, 1)] public float defaultAnisotropic;
    public Color defaultDiffuseColor;
    public float defaultDisplacementStrength;
    public float defaultNormalMapStrength;
    public bool defaultUseDisplacementMap;
    public bool defaultUseDiffuseMap;
    public bool defaultUseNormalMap;
    public bool defaultUseRoughnessMap;
    public bool defaultRotateModel;
    public bool defaultRotateLight;
    public bool defaultUseEnironmentLighting;
    public Texture2D integratedBRDF;
    public float defaultLightIntensity;
    public Color defaultLightColor;
    public Color defaultBackgroundColor;

    private GameObject model;
    private Material material;
    private ModelObject modelObject;
    private TextureObject textureObject;
    private EnvironmentObject environmentObject;
    private float reflectance;
    private float roughness;
    private float subsurface;
    private float sheen;
    private float sheenTint;
    private float clearcoat;
    private float clearcoatGloss;
    private float metallic;
    private float anisotropic;
    private Color diffuseColor;
    private NormalDistributionFunction ndf;
    private GeometryAttenuationFunction geometry;
    private Diffuse diffuse;
    private DebugView debug;
    private float displacementMapStrength;
    private float normalMapStrength;
    private bool useDisplacementMap;
    private bool useDiffuseMap;
    private bool useNormalMap;
    private bool useRoughnessMap;
    private bool rotateModel;
    private bool rotateLight;
    private bool useEnvironmentLighting;
    private float lightIntensity;
    private Color lightColor;
    private Color backgroundColor;
    private List<ModelObject> modelObjects;
    private List<TextureObject> textureObjects;
    private List<EnvironmentObject> environmentObjects;


    void Start()
    {
        modelObjects = Loader.LoadScriptableObjects<ModelObject>();
        textureObjects = Loader.LoadScriptableObjects<TextureObject>();
        environmentObjects = Loader.LoadScriptableObjects<EnvironmentObject>();

        roughness = defaultRoughness;
        reflectance = defaultReflectance;
        subsurface = defaultSubsurface;
        diffuseColor = defaultDiffuseColor;
        sheen = defaultSheen;
        sheenTint = defaultSheenTint;
        clearcoat = defaultClearcoat;
        clearcoatGloss = defaultClearcoatGloss;
        metallic = defaultMetallic;
        anisotropic = defaultAnisotropic;
        modelObject = defaultModelObject;
        textureObject = defaultTextureObject;
        environmentObject = defaultEnvironmentObject;
        displacementMapStrength = defaultDisplacementStrength;
        normalMapStrength = defaultNormalMapStrength;
        useDisplacementMap = defaultUseDisplacementMap;
        useNormalMap = defaultUseNormalMap;
        useDiffuseMap = defaultUseDiffuseMap;
        useRoughnessMap = defaultUseRoughnessMap;
        rotateModel = defaultRotateModel;
        rotateLight = defaultRotateLight;
        useEnvironmentLighting = defaultUseEnironmentLighting;
        material = new Material(pbrShader);
        ndf = defaultNDF;
        geometry = defaultGeometry;
        diffuse = defaultDiffuse;
        debug = defaultDebug;
        lightIntensity = defaultLightIntensity;
        lightColor = defaultLightColor;
        backgroundColor = defaultBackgroundColor;

        InstantiateModel();
        UpdateMaterialParameters();
        SetTextureMaps();
        SetEnvironment();
        InstantiateUI();

    }

    void Update()
    {
        if (rotateModel)
        {
            model.transform.Rotate(new Vector3(0, Time.deltaTime * 4.0f, 0));
        }
        if (defaultRotateLight)
        {
            sun.transform.Rotate(new Vector3(0, Time.deltaTime * -15.0f, 0), Space.World);
        }
        material.SetVector("_LightDirection", -sun.transform.forward);
    }

    void InstantiateUI()
    {
        view.GenerateUI();

        // Setting Dropdown Choices
        view.materialMenu.diffuseModelDropdown.SetDropdownChoices(Enum.GetNames(typeof(Diffuse)));
        view.materialMenu.normalDistributionModelDropdown.SetDropdownChoices(Enum.GetNames(typeof(NormalDistributionFunction)));
        view.materialMenu.geometryAttenuationModelDropdown.SetDropdownChoices(Enum.GetNames(typeof(GeometryAttenuationFunction)));
        view.materialMenu.debugViewTypeDropdown.SetDropdownChoices(Enum.GetNames(typeof(DebugView)));

        view.modelButton.text = "Model: " + modelObject.id;
        view.textureButton.text = "Texture: " + textureObject.id;

        // Setting Overlay Choices
        view.modelOverlay.SetImages(modelObjects.Select(obj => obj.displayImage).ToList(), modelObjects.Select(obj => obj.id).ToList());
        view.modelOverlay.OnOverlaySelection += (index) =>
        {
            modelObject = modelObjects[index];
            view.modelButton.text = "Model: " + modelObject.id;
            InstantiateModel();
        };

        view.textureOverlay.SetImages(textureObjects.Select(obj => obj.displayImage).ToList(), textureObjects.Select(obj => obj.id).ToList());
        view.textureOverlay.OnOverlaySelection += (index) =>
        {
            textureObject = textureObjects[index];
            view.textureButton.text = "Texture: " + textureObject.id;
            SetTextureMaps();
        };

        view.environmentOverlay.SetImages(environmentObjects.Select(obj => obj.displayImage).ToList(), environmentObjects.Select(obj => obj.id).ToList());
        view.environmentOverlay.OnOverlaySelection += (index) =>
        {
            environmentObject = environmentObjects[index];
            view.environmentButton.text = "Environment: " + environmentObject.id;
            SetEnvironment();
        };

        // Bind Sliders
        BindSlider(view.materialMenu.displacementMapStrengthSlider, newValue => displacementMapStrength = newValue);
        BindSlider(view.materialMenu.normalMapStrengthSlider, newValue => normalMapStrength = newValue);
        BindSlider(view.materialMenu.reflectanceSlider, newValue => reflectance = newValue);
        BindSlider(view.materialMenu.roughnessSlider, newValue => roughness = newValue);
        BindSlider(view.materialMenu.subsurfaceSlider, newValue => subsurface = newValue);
        BindSlider(view.materialMenu.sheenSlider, newValue => sheen = newValue);
        BindSlider(view.materialMenu.sheenTintSlider, newValue => sheenTint = newValue);
        BindSlider(view.materialMenu.clearcoatSlider, newValue => clearcoat = newValue);
        BindSlider(view.materialMenu.clearcoatGlossSlider, newValue => clearcoatGloss = newValue);
        BindSlider(view.materialMenu.metallicSlider, newValue => metallic = newValue);
        BindSlider(view.materialMenu.lightIntensitySlider, newValue => lightIntensity = newValue);
        // BindSlider(view.materialMenu.anisotropicSlider, newValue => anisotropic = newValue);

        // Bind Color Pickers
        BindColorPicker(view.materialMenu.diffuseColorPicker, newColor => diffuseColor = newColor);
        BindColorPicker(view.materialMenu.lightColorPicker, newColor => lightColor = newColor);
        BindColorPicker(view.materialMenu.backgroundColorPicker, newColor => { backgroundColor = newColor; SetEnvironment(); });

        // Bind Dropdown
        BindDropdown(view.materialMenu.normalDistributionModelDropdown, newValue => ndf = (NormalDistributionFunction)newValue);
        BindDropdown(view.materialMenu.geometryAttenuationModelDropdown, newValue => geometry = (GeometryAttenuationFunction)newValue);
        BindDropdown(view.materialMenu.debugViewTypeDropdown, newValue => debug = (DebugView)newValue);
        BindDropdown(view.materialMenu.diffuseModelDropdown, newValue =>
        {
            diffuse = (Diffuse)newValue;
            view.materialMenu.sheenSlider.SetEnabled(diffuse == Diffuse.Disney);
            view.materialMenu.sheenTintSlider.SetEnabled(diffuse == Diffuse.Disney);
            view.materialMenu.subsurfaceSlider.SetEnabled(diffuse == Diffuse.Disney);
        });

        // Bind Toggle
        BindToggle(view.materialMenu.lightRotateToggle, newValue => rotateLight = newValue);
        BindToggle(view.materialMenu.objectRotateToggle, newValue => rotateModel = newValue);
        BindToggle(view.materialMenu.displacementMapToggle, newValue => useDisplacementMap = newValue);
        BindToggle(view.materialMenu.diffuseMapToggle, newValue => useDiffuseMap = newValue);
        BindToggle(view.materialMenu.normalMapToggle, newValue => useNormalMap = newValue);
        BindToggle(view.materialMenu.roughnessMapToggle, newValue => useRoughnessMap = newValue);
        BindToggle(view.materialMenu.environmentLightingToggle, newValue =>
        {
            useEnvironmentLighting = newValue;
            SetEnvironment();
            if (useEnvironmentLighting)
            {
                view.materialMenu.diffuseModelDropdown.SetDropdownChoices(new[] { "Lambert" });
                view.materialMenu.normalDistributionModelDropdown.SetDropdownChoices(new[] { "GGX" });
                view.materialMenu.geometryAttenuationModelDropdown.SetDropdownChoices(new[] { "SchlickGGX" });
                view.materialMenu.diffuseModelDropdown.SetCurrentValue(0);
                view.materialMenu.normalDistributionModelDropdown.SetCurrentValue(0);
                view.materialMenu.geometryAttenuationModelDropdown.SetCurrentValue(0);
            }
            else
            {
                view.materialMenu.diffuseModelDropdown.SetDropdownChoices(Enum.GetNames(typeof(Diffuse)));
                view.materialMenu.normalDistributionModelDropdown.SetDropdownChoices(Enum.GetNames(typeof(NormalDistributionFunction)));
                view.materialMenu.geometryAttenuationModelDropdown.SetDropdownChoices(Enum.GetNames(typeof(GeometryAttenuationFunction)));
                view.materialMenu.diffuseModelDropdown.SetCurrentValue((int)diffuse);
                view.materialMenu.normalDistributionModelDropdown.SetCurrentValue((int)ndf);
                view.materialMenu.geometryAttenuationModelDropdown.SetCurrentValue((int)geometry);
            }

            view.materialMenu.lightRotateToggle.SetEnabled(!useEnvironmentLighting);
            view.materialMenu.lightColorPicker.SetEnabled(!useEnvironmentLighting);
            view.materialMenu.lightIntensitySlider.SetEnabled(!useEnvironmentLighting);
            view.materialMenu.backgroundColorPicker.SetEnabled(!useEnvironmentLighting);

        });

        // Set Default UI Values
        view.materialMenu.SetValues(
            reflectance,
            roughness,
            subsurface,
            diffuseColor,
            (int)ndf,
            (int)geometry,
            (int)diffuse,
            (int)debug,
            useDiffuseMap,
            useNormalMap,
            useDisplacementMap,
            useRoughnessMap,
            normalMapStrength,
            displacementMapStrength,
            sheen,
            sheenTint,
            clearcoat,
            clearcoatGloss,
            metallic,
            useEnvironmentLighting,
            rotateLight,
            rotateModel,
            lightColor,
            lightIntensity,
            backgroundColor
        );
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
        GameObject mesh = new GameObject("mesh");
        mesh.transform.parent = model.transform;

        mesh.AddComponent<MeshFilter>().mesh = modelObject.GetMesh();
        modelObject.SetToTransform(mesh.transform);
        mesh.AddComponent<MeshRenderer>().material = material;
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

    void SetEnvironment()
    {
        if (useEnvironmentLighting)
        {
            material.SetTexture("_FilteredDiffuseMap", environmentObject.filteredDiffuseMap);
            material.SetTexture("_IntegratedBRDF", integratedBRDF);
            material.SetTexture("_FilteredSpecularMap", environmentObject.filteredSpecularMap);
            material.SetInt("_SpecularMipLevels", 6); //Hardcoded for now
            RenderSettings.skybox = environmentObject.skyboxMaterial;
        }
        else
        {
            RenderSettings.skybox = null;
            Camera.main.backgroundColor = backgroundColor;
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
        material.SetFloat("_Sheen", sheen);
        material.SetFloat("_SheenTint", sheenTint);
        material.SetFloat("_Clearcoat", clearcoat);
        material.SetFloat("_ClearcoatGloss", clearcoatGloss);
        material.SetFloat("_Metallic", metallic);
        material.SetFloat("_Anisotropic", anisotropic);
        material.SetColor("_AmbientColor", backgroundColor);

        material.SetInt("_NDF", (int)ndf);
        material.SetInt("_Geometry", (int)geometry);
        material.SetInt("_Diffuse", (int)diffuse);
        material.SetInt("_DebugView", (int)debug);

        material.SetColor("_LightColor", lightColor);
        material.SetFloat("_LightIntensity", lightIntensity);

        material.SetFloat("_DisplacementStrength", displacementMapStrength);
        material.SetFloat("_NormalStrength", normalMapStrength);

        material.SetInt("_UseDisplacementMap", useDisplacementMap ? 1 : 0);
        material.SetInt("_UseDiffuseMap", useDiffuseMap ? 1 : 0);
        material.SetInt("_UseNormalMap", useNormalMap ? 1 : 0);
        material.SetInt("_UseRoughnessMap", useRoughnessMap ? 1 : 0);
        material.SetInt("_UseEnvironmentLighting", useEnvironmentLighting ? 1 : 0);
    }

    void OnDisable()
    {
        Destroy(material);
        Destroy(model);
    }
}
