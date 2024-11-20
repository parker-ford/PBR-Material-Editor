using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MaterialManager : MonoBehaviour
{
    public Light sun;
    public Color diffuseColor;
    public float specularHardness = 1.0f;
    public float specularStrength = 1.0f;

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
    }
}
