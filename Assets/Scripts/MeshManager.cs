using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Material material;
    public Light sun;
    public float specularHardness;
    public float specularStrength;
    public bool useNormalMap = true;
    public float normalStrength = 1.0f;
    void Start()
    {
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter.mesh = ProceduralMesh.Plane(10, 10);
        meshRenderer.material = material;
        transform.localScale = new Vector3(1.0f / 10.0f, 1.0f, 1.0f / 10.0f);
        // transform.Rotate(new Vector3(-90, 0, 0));

    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalVector("_LightDirection", -sun.transform.forward);
        Shader.SetGlobalFloat("_SpecularHardness", specularHardness);
        Shader.SetGlobalFloat("_SpecularStrength", specularStrength);
        Shader.SetGlobalInt("_UseNormalMap", useNormalMap ? 1 : 0);
        Shader.SetGlobalFloat("_NormalStrength", normalStrength);
    }
}
