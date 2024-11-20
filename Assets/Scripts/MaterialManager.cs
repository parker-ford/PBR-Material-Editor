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

    public enum DebugView
    {
        None,
        NormalDistributionFunction,
        GeometryAttenuation,
        Fresnel
    }

    public Light sun;
    public Color diffuseColor;
    public float specularHardness = 1.0f;
    public float specularStrength = 1.0f;
    [Range(0, 1)]
    public float roughness = 0.0f;
    public NormalDistributionFunction ndf;
    public DebugView debugView;

    void Start()
    {

    }

    void setLightValues()
    {
        Shader.SetGlobalVector("_LightDirection", -sun.transform.forward);
        Shader.SetGlobalColor("_LightColor", sun.color);
        Shader.SetGlobalFloat("_LightIntensity", sun.intensity);
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
        Shader.SetGlobalInt("_DebugView", (int)debugView);
    }
}
