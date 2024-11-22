using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MaterialManager : MonoBehaviour
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
    }

    public Light sun;
    public Color diffuseColor;
    public Texture2D diffuseTexture;
    public Texture2D normalTexture;
    public float specularHardness = 1.0f;
    public float specularStrength = 1.0f;
    [Range(0, 1)]
    public float roughness = 0.0f;
    [Range(0, 1)]
    public float reflectance = 0.5f;
    [Range(0, 1)]
    public float subsurface = 0.5f;
    public NormalDistributionFunction ndf;
    public GeometryAttenuationFunction geo;
    public Diffuse diffuse;
    public DebugView debugView;

    void Start()
    {

    }

    void setLightValues()
    {
        Shader.SetGlobalVector("_LightDirection", -sun.transform.forward);
        Shader.SetGlobalColor("_LightColor", sun.color);
        Shader.SetGlobalFloat("_LightIntensity", sun.intensity);
        Shader.SetGlobalTexture("_DiffuseTex", diffuseTexture);
        Shader.SetGlobalTexture("_NormalTexture", normalTexture);
    }

    // Update is called once per frame
    void Update()
    {
        setLightValues();
        Shader.SetGlobalColor("_DiffuseColor", diffuseColor);
        Shader.SetGlobalFloat("_SpecularHardness", specularHardness);
        Shader.SetGlobalFloat("_SpecularStrength", specularStrength);
        Shader.SetGlobalFloat("_Roughness", roughness);
        Shader.SetGlobalInt("_NDF", (int)ndf);
        Shader.SetGlobalInt("_GEO", (int)geo);
        Shader.SetGlobalInt("_DebugView", (int)debugView);
        Shader.SetGlobalInt("_Diffuse", (int)diffuse);
        Shader.SetGlobalFloat("_Reflectance", reflectance);
        Shader.SetGlobalFloat("_Subsurface", subsurface);

    }
}
